using System;
using System.Collections.Generic;
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

        public async Task<ExecutionStatistics> Execute(IReadOnlyList<ExecutionTask> tasks, IProgress<int> progress = null)
        {
            var evaluationResult = new ExecutionStatisticsRecorder();
            int finishedCount = 0;
            foreach (var t in tasks)
            {
                await executeTask(t, evaluationResult);

                ++finishedCount;
                progress?.Report((int)((double)finishedCount / tasks.Count));
            }
            return evaluationResult.GetEvaluationStatistics();
        }

        private async Task executeTask(ExecutionTask task, ExecutionStatisticsRecorder evaluationStatisticsRecorder)
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

                        using (var grader = new Grader.TrxGrader(createTrxTask(task), logger))
                        {
                            if (runnerResult.HadError())
                            {
                                await grader.GradeFailedExecution(runnerResult.Exception.Message);
                                evaluationStatScope.OnExecutionFailed();
                                logger.LogError(runnerResult.Exception, "Evaluation task failed");
                            }
                            else
                            {
                                await grader.GradeResult();
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
                                               TimeSpanHelper.Smaller(task.DockerTimeout, maxTimeout));

        private Grader.TrxGraderTask createTrxTask(ExecutionTask task)
            => new Grader.TrxGraderTask(task.TaskId, task.StudentId, System.IO.Path.Combine(task.ResultArtifactPath, task.TrxFileName), task.TrxOutputFile);
    }
}
