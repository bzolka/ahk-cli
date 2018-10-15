using System;
using System.Threading;
using System.Threading.Tasks;
using AHK.TaskRunner;
using Microsoft.Extensions.Logging;

namespace AHK.Evaluation
{
    public class EvaluationService
    {
        private readonly IEvaluatorInputQueue inputQueue;
        private readonly ITaskRunnerFactory taskRunnerFactory;
        private readonly IGraderFactory graderFactory;
        private readonly ILogger<EvaluationService> logger;
        private readonly TimeSpan evaluationTimeout;

        public EvaluationService(IEvaluatorInputQueue inputQueue, ITaskRunnerFactory taskRunnerFactory, IGraderFactory graderFactory,
                                 ILogger<EvaluationService> logger,
                                 TimeSpan? evaluationTimeout = null)
        {
            this.inputQueue = inputQueue ?? throw new ArgumentNullException(nameof(inputQueue));
            this.taskRunnerFactory = taskRunnerFactory ?? throw new ArgumentNullException(nameof(taskRunnerFactory));
            this.graderFactory = graderFactory ?? throw new ArgumentNullException(nameof(graderFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.evaluationTimeout = evaluationTimeout ?? TimeSpan.FromMinutes(8);
        }

        public async Task RunContinuously(CancellationToken stop)
        {
            logger.LogInformation("Queue processing started");
            try
            {
                var evaluationResult = new EvaluationStatisticsRecorder();
                while (!stop.IsCancellationRequested)
                {
                    var evaluationTask = inputQueue.Dequeue(stop);
                    await processTask(evaluationTask, evaluationResult);
                }
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation($"Queue processing cancelled");
                return;
            }
            finally
            {
                logger.LogInformation($"Queue processing terminated");
            }
        }

        public async Task<EvaluationStatistics> ProcessAllQueue()
        {
            var evaluationResult = new EvaluationStatisticsRecorder();
            while (inputQueue.TryDequeue(out EvaluationTask evaluationTask))
            {
                await processTask(evaluationTask, evaluationResult);
            }
            return evaluationResult.GetEvaluationStatistics();
        }

        private async Task processTask(EvaluationTask evaluationTask, EvaluationStatisticsRecorder evaluationStatisticsRecorder)
        {
            using (logger.BeginScope("Evaluating task {TaskId}; StudentId {StudentId}", evaluationTask.TaskId, evaluationTask.StudentId))
            using (var evaluationStatScope = evaluationStatisticsRecorder.OnExecutionStarted())
            {
                logger.LogInformation("Input path {solutionPath}; artifacts {artifactPath}", evaluationTask.SolutionPath, evaluationTask.ResultArtifactPath);
                logger.LogInformation("Starting evaluation");

                try
                {
                    var runnerTask = createRunnerTask(evaluationTask);

                    using (var taskRunner = taskRunnerFactory.CreateRunner(runnerTask))
                    using (var grader = graderFactory.CreateGrader(evaluationTask))
                    {
                        logger.LogTrace("Starting runner");
                        var runnerResult = await taskRunner.Run();
                        logger.LogTrace("Finished runner");

                        if (runnerResult.HadError())
                        {
                            await grader.StoreFailedExecution(runnerResult.Exception.Message);
                            evaluationStatScope.OnExecutionFailed();
                            logger.LogError(runnerResult.Exception, "Evaluation task failed");
                        }
                        else
                        {
                            await grader.GradeResult(evaluationTask.ResultArtifactPath);
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

        private RunnerTask createRunnerTask(EvaluationTask evaluationTask)
            => new RunnerTask(evaluationTask.TaskId,
                              evaluationTask.EvaluationConfig.ImageName,
                              evaluationTask.SolutionPath, evaluationTask.EvaluationConfig.SolutionInContainer,
                              evaluationTask.EvaluationConfig.ResultInContainer, evaluationTask.ResultArtifactPath,
                              TimeSpanHelper.Smaller(evaluationTask.EvaluationConfig.EvaluationTimeout, evaluationTimeout));
    }
}
