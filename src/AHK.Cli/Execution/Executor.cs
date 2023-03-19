using System;
using System.Threading.Tasks;
using AHK.TaskRunner;
using Microsoft.Extensions.Logging;

namespace AHK.Execution
{
    public class Executor
    {
        private readonly ILogger logger;
        private readonly TimeSpan maxTimeout;

        public Executor(ILogger logger, TimeSpan? maxTimeout = null)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.maxTimeout = maxTimeout ?? TimeSpan.FromMinutes(8);
        }

        public async Task<ExecutionStatistics> Execute(RunConfig runConfig, IProgress<int> progress = null)
        {
            if (runConfig == null)
                throw new ArgumentNullException(nameof(runConfig));

            using (var xlsxWriter = new ExcelResultsWriter.XlsxResultsWriter(runConfig.XlsxResultsWriterConfig))
            {
                var evaluationResult = new ExecutionStatisticsRecorder();
                int finishedCount = 0;
                foreach (var t in runConfig.Tasks)
                {
                    await executeTask(t, evaluationResult, xlsxWriter);

                    ++finishedCount;
                    progress?.Report((int)((double)finishedCount / runConfig.Tasks.Count * 100));
                }
                return evaluationResult.GetEvaluationStatistics();
            }
        }

        private async Task executeTask(ExecutionTask task, ExecutionStatisticsRecorder evaluationStatisticsRecorder, ExcelResultsWriter.XlsxResultsWriter resultsWriter)
        {
            using (logger.BeginScope("Evaluating task {TaskId}; Student {StudentName} {StudentNeptun}", task.TaskId, task.StudentName, task.StudentNeptun))
            using (var evaluationStatScope = evaluationStatisticsRecorder.OnExecutionStarted())
            {
                logger.LogInformation("Input path {solutionPath}; artifacts {artifactPath}", task.SolutionPath, task.ResultArtifactPath);
                logger.LogInformation("Starting evaluation");

                try
                {
                    using (var taskRunner = createTaskRunner(task, logger))
                    {
                        logger.LogTrace("Starting runner");
                        var runnerResult = await taskRunner.Run();
                        logger.LogTrace("Finished runner");

                        if (!string.IsNullOrEmpty(runnerResult.ConsoleOutput))
                        {
                            if (!string.IsNullOrEmpty(task.ResultArtifactPath))
                            {
                                var outputFilePath = System.IO.Path.Combine(task.ResultArtifactPath, "_console-log.txt");
                                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(outputFilePath));
                                System.IO.File.WriteAllText(
                                        outputFilePath,
                                        sanitizeContainerConsoleOutput(runnerResult.ConsoleOutput, task.EvaluationTask),
                                        System.Text.Encoding.UTF8);

                                logger.LogTrace("Container stdout saved to artifact directory");
                            }
                            else
                            {
                                logger.LogWarning("Container stdout lost; no artifact directory");
                            }
                        }

                        if (runnerResult.HadError())
                        {
                            evaluationStatScope.OnExecutionFailed();
                            logger.LogError(runnerResult.Exception, "Evaluation task failed");
                        }
                        else
                        {
                            if (task.EvaluationTask != null)
                            {
                                var graderResult = await task.EvaluationTask.ExecuteGrader(task, runnerResult, logger);
                                if (!graderResult.HasResult)
                                {
                                    evaluationStatScope.OnExecutionFailed();
                                    logger.LogError(runnerResult.Exception, "Grading yield not results");
                                }
                                else
                                {
                                    resultsWriter.Write(task.StudentName, task.StudentNeptun, graderResult);
                                    evaluationStatScope.OnExecutionCompleted();
                                }
                            }
                            else
                            {
                                evaluationStatScope.OnExecutionCompleted();
                            }
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

        private ITaskRunner createTaskRunner(ExecutionTask task, ILogger logger)
        {
            switch (task)
            {
                case DockerExecutionTask dockerTask:
                    return new TaskRunner.DockerRunner(createDockerTask(dockerTask), logger);
                case LocalCmdExecutionTask cmdTask:
                    return new TaskRunner.LocalCmdRunner(createLocalCmdTask(cmdTask), logger);
                default:
                    throw new ArgumentException("task is not a recognized ExecutionTask", nameof(task));
            }
        }

        private TaskRunner.DockerRunnerTask createDockerTask(DockerExecutionTask task)
            => new TaskRunner.DockerRunnerTask(task.TaskId,
                                               task.DockerImageName,
                                               task.SolutionPath, task.DockerSolutionDirectoryInContainer,
                                               task.ResultArtifactPath, task.DockerResultPathInContainer,
                                               TimeSpanHelper.Smaller(task.DockerTimeout, maxTimeout),
                                               task.DockerImageParams);

        private TaskRunner.LocalCmdRunnerTask createLocalCmdTask(LocalCmdExecutionTask task)
         => new TaskRunner.LocalCmdRunnerTask(task.TaskId,
                                            task.SolutionPath, task.ResultArtifactPath,
                                            TimeSpanHelper.Smaller(task.CommandTimeout, maxTimeout),
                                            task.Command, task.EnvironmentVariables);// TODO-BZ: DockerTimeout is not a menaingful name here


        private string sanitizeContainerConsoleOutput(string text, Evaluation.EvaluationTask evaluationTask)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            if (evaluationTask is Evaluation.ConsoleMessagesEvaluationTask consoleEval)
            {
                var builder = new System.Text.StringBuilder(text.Length);

                using (var sr = new System.IO.StringReader(text))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        // skip lines that are ment for the ConsoleMessagesGrader
                        if (line.StartsWith(@"###ahk", StringComparison.OrdinalIgnoreCase))
                            continue;

                        // remove the validation code from any line; just as a safety measure
                        line = line.Replace(consoleEval.ValidationCode, "{***}");

                        builder.AppendLine(line);
                    }
                }

                return builder.ToString();
            }
            else
            {
                return text;
            }
        }
    }
}
