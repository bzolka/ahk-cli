using System;
using System.Threading.Tasks;
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

            using (var xlsxWriter = new ExcelResultsWriter.XlsxResultsWriter(runConfig.ResultsXlsxFileName))
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
            using (logger.BeginScope("Evaluating task {TaskId}; StudentId {StudentId}", task.TaskId, task.StudentId))
            using (var evaluationStatScope = evaluationStatisticsRecorder.OnExecutionStarted())
            {
                logger.LogInformation("Input path {solutionPath}; artifacts {artifactPath}", task.SolutionPath, task.ResultArtifactPath);
                logger.LogInformation("Starting evaluation");

                try
                {
                    using (var taskRunner = new TaskRunner.DockerRunner(createDockerTask(task), logger))
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
                                        sanitizeContainerConsoleOutput(runnerResult.ConsoleOutput),
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
                                if (!graderResult.GradingSuccessful)
                                {
                                    evaluationStatScope.OnExecutionFailed();
                                    logger.LogError(runnerResult.Exception, "Grader failed");
                                }
                                else
                                {
                                    resultsWriter.Write(task.StudentId, graderResult);
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

        private TaskRunner.DockerRunnerTask createDockerTask(ExecutionTask task)
            => new TaskRunner.DockerRunnerTask(task.TaskId,
                                               task.DockerImageName,
                                               task.SolutionPath, task.DockerSolutionDirectoryInContainer,
                                               task.ResultArtifactPath, task.DockerResultPathInContainer,
                                               TimeSpanHelper.Smaller(task.DockerTimeout, maxTimeout),
                                               task.DockerImageParams);

        private string sanitizeContainerConsoleOutput(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            var builder = new System.Text.StringBuilder(text.Length);

            using (var sr = new System.IO.StringReader(text))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    // skip lines that are ment for the ConsoleMessagesGrader
                    if (line.StartsWith(@"###ahk", StringComparison.OrdinalIgnoreCase))
                        continue;

                    builder.AppendLine(line);
                }
            }

            return builder.ToString();
        }
    }
}
