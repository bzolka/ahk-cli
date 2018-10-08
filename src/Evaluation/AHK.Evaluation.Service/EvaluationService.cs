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
        private readonly ITaskSolutionProvider taskSolutionProvider;
        private readonly IResultArtifactHandler resultArtifactsHandler;
        private readonly ILogger<EvaluationService> logger;
        private readonly TimeSpan evaluationTimeout;

        public EvaluationService(IEvaluatorInputQueue inputQueue, ITaskRunnerFactory taskRunnerFactory,
                                 ITaskSolutionProvider taskSolutionProvider, IResultArtifactHandler resultArtifactsHandler,
                                 ILogger<EvaluationService> logger,
                                 TimeSpan? evaluationTimeout = null)
        {
            this.inputQueue = inputQueue ?? throw new ArgumentNullException(nameof(inputQueue));
            this.taskRunnerFactory = taskRunnerFactory ?? throw new ArgumentNullException(nameof(taskRunnerFactory));
            this.taskSolutionProvider = taskSolutionProvider ?? throw new ArgumentNullException(nameof(taskSolutionProvider));
            this.resultArtifactsHandler = resultArtifactsHandler ?? throw new ArgumentNullException(nameof(resultArtifactsHandler));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.evaluationTimeout = evaluationTimeout ?? TimeSpan.FromMinutes(8);
        }

        public async Task RunContinuously(CancellationToken stop)
        {
            logger.LogInformation("Queue processing started");
            try
            {
                var evaluationResult = new EvaluationStatistics();
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
            var evaluationResult = new EvaluationStatistics();
            while (inputQueue.TryDequeue(out EvaluationTask evaluationTask))
            {
                await processTask(evaluationTask, evaluationResult);
            }
            return evaluationResult;
        }

        private async Task processTask(EvaluationTask evaluationTask, EvaluationStatistics resultAggregator)
        {
            using (logger.BeginScope("Evaluating task {TaskId}; StudentId {StudentId}", evaluationTask.TaskId, evaluationTask.StudentId))
            {
                logger.LogInformation("Starting evaluation");
                try
                {
                    resultAggregator.OnExecutionStarted();

                    var solutionPath = taskSolutionProvider.GetSolutionPath(evaluationTask.StudentId);
                    var artifactPath = resultArtifactsHandler.GetPathFor(evaluationTask.StudentId);
                    var runnerTask = createRunnerTask(evaluationTask, solutionPath, artifactPath);

                    logger.LogInformation("InputPath {solutionPath}; artifacts {artifactPath}", solutionPath, artifactPath);

                    using (var taskRunner = taskRunnerFactory.CreateRunner(runnerTask))
                    {
                        logger.LogTrace("Starting runner");
                        var runnerResult = await taskRunner.Run();
                        logger.LogTrace("Finished runner");

                        if (runnerResult.HadError())
                        {
                            resultAggregator.OnExecutionFailed();
                            logger.LogError(runnerResult.Exception, "Evaluation task failed");
                        }
                        else
                        {
                            resultAggregator.OnExecutionCompleted();
                        }
                    }
                }
                catch (Exception ex)
                {
                    resultAggregator.OnExecutionFailed();
                    logger.LogError(ex, "Evaluation task failed");
                }
                finally
                {
                    logger.LogInformation("Terminating evaluation task");
                }
            }
        }

        private RunnerTask createRunnerTask(EvaluationTask evaluationTask, string solutionFolder, string resultFolder)
            => new RunnerTask(evaluationTask.TaskId,
                              evaluationTask.EvaluationConfig.ImageName,
                              solutionFolder, evaluationTask.EvaluationConfig.SolutionDirectoryInContainer,
                              evaluationTask.EvaluationConfig.ResultInContainer, resultFolder,
                              TimeSpanHelper.Smaller(evaluationTask.EvaluationConfig.EvaluationTimeout, evaluationTimeout));
    }
}
