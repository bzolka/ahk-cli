using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ahk.Grader;
using Ahk.TaskRunner;
using CliFx.Attributes;
using CliFx.Exceptions;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace Ahk.Commands.Eval
{
    public abstract class EvaluateCommandBase : CommandBase
    {
        protected readonly ILogger logger;
        private readonly ExecutionStatisticsRecorder evaluationStatistics = new ExecutionStatisticsRecorder();

        public EvaluateCommandBase(ILogger logger)
            => this.logger = logger;

        [CommandOption("submissions", 's', Description = "A directory containing the submissions to evaluate. The current directory if not specified.")]
        public string? SubmissionsDirArg { get; set; }

        [CommandOption("output", 'o', Description = "A directory to use as output; the results of the evaluation are placed here. A new directory in the current directory if not specified.")]
        public string? OutputDirArg { get; set; }

        [CommandOption("studentid", 'd', Description = "A file name expected in the root of every submission folder/zip containing the identifier of the student.")]
        public string? StudentIdTextFileName { get; set; } = @"neptun.txt";


        protected override async Task executeCommandCore()
        {
            await AnsiConsole.Progress()
                  .Columns(new ProgressColumn[]
                  {
                              new TaskDescriptionColumn(),
                              new ProgressBarColumn(),
                              new PercentageColumn(),
                              new RemainingTimeColumn(),
                  })
                  .StartAsync(execCore);

            if (evaluationStatistics!.HasFailed())
                AnsiConsole.MarkupLine($"[{Color.Yellow}]Some submissions were not evaluated:[/] [{Color.Green}]{evaluationStatistics.ExecutedSuccessfully} completed[/] and [{Color.Red}]{evaluationStatistics.FailedExecution} FAILED[/].");
            else
                AnsiConsole.MarkupLine($"[{Color.Green}]Completed the evaluation of {evaluationStatistics.ExecutedSuccessfully} submissions[/]");
        }

        private async Task execCore(ProgressContext ctx)
        {
            var submissionsDirEffective = PathHelper.GetSubmissionsDir(SubmissionsDirArg);
            var outputDirEffective = PathHelper.GetOutputDir(OutputDirArg);

            var submissions = listPossibleSubmissions(submissionsDirEffective);
            if (!submissions.Any())
                throw new CommandException($"Submission directory '{submissionsDirEffective}' has no subdirectories or zip files");

            var progressExec = ctx.AddTask("Running evaluation");
            progressExec.MaxValue = submissions.Count;

            var resultsXlsxFileName = Path.Combine(outputDirEffective, "results.xlsx");
            using (var xlsxWriter = new ExcelResultsWriter.XlsxResultsWriter(resultsXlsxFileName))
            {
                foreach (var submissionPath in submissions)
                {
                    await executeTask(submissionPath, outputDirEffective, xlsxWriter);
                    progressExec.Increment(1);
                }
            }
        }

        private static IReadOnlyCollection<string> listPossibleSubmissions(string submissionsDir) =>
            Directory.EnumerateDirectories(submissionsDir).Union(Directory.EnumerateFiles(submissionsDir, "*.zip")).ToList();

        private async Task executeTask(string submissionPath, string outputDirEffective, ExcelResultsWriter.XlsxResultsWriter resultsWriter)
        {
            var studentId = getStudentId(submissionPath);
            var artifactPath = Path.Combine(outputDirEffective, studentId);
            using (var evaluationStatScope = evaluationStatistics.OnExecutionStarted())
            {
                logger.LogInformation("Input path {solutionPath}; artifacts {artifactPath}", submissionPath, artifactPath);
                logger.LogInformation("Starting evaluation");

                try
                {
                    using var taskRunner = CreateRunner(submissionPath, studentId, artifactPath);
                    var runnerResult = await taskRunner.Run();

                    if (!string.IsNullOrEmpty(runnerResult.ConsoleOutput))
                    {
                        var outputFilePath = Path.Combine(artifactPath, "_console-log.txt");
                        Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));
                        await File.WriteAllTextAsync(outputFilePath, sanitizeContainerConsoleOutput(runnerResult.ConsoleOutput), System.Text.Encoding.UTF8);
                    }

                    if (runnerResult.HadError())
                    {
                        evaluationStatScope.OnExecutionFailed();
                        logger.LogError(runnerResult.Exception, "Evaluation task failed");
                    }
                    else
                    {
                        using var grader = CreateGrader(submissionPath, studentId, artifactPath, runnerResult);
                        var graderResult = await grader.GradeResult();
                        if (!graderResult.HasResult)
                        {
                            evaluationStatScope.OnExecutionFailed();
                            logger.LogError(runnerResult.Exception, "Grading yield not results");
                        }
                        else
                        {
                            resultsWriter.Write(studentId, graderResult);
                            evaluationStatScope.OnExecutionCompleted();
                        }
                    }
                }
                catch (Exception ex)
                {
                    evaluationStatScope.OnExecutionFailed();
                    logger.LogError(ex, "Evaluation task failed");
                }
                finally
                {
                    logger.LogInformation("Terminating evaluation task");
                }
            }
        }

        protected virtual string sanitizeContainerConsoleOutput(string text) => text;

        protected abstract ITaskRunner CreateRunner(string submissionSource, string studentId, string artifactPath);
        protected abstract IGrader CreateGrader(string submissionSource, string studentId, string artifactPath, RunnerResult runnerResult);

        private string getStudentId(string submissionPath)
        {
            if (!string.IsNullOrEmpty(StudentIdTextFileName))
                return StudentIdParser.GetStudentId(submissionPath, StudentIdTextFileName);

            return Path.GetFileNameWithoutExtension(submissionPath);
        }

    }
}
