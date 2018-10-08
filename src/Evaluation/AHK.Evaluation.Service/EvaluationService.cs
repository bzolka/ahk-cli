using System;
using System.Threading;
using System.Threading.Tasks;
using AHK.TaskRunner;

namespace AHK.Evaluation
{
    public class EvaluationService
    {
        private readonly IEvaluatorInputQueue inputQueue;
        private readonly ITaskRunnerFactory taskRunnerFactory;
        private readonly ITaskSolutionProvider taskSolutionProvider;
        private readonly IResultArtifactHandler resultArtifactsHandler;
        private readonly TimeSpan evaluationTimeout;

        public EvaluationService(IEvaluatorInputQueue inputQueue, ITaskRunnerFactory taskRunnerFactory,
                                 ITaskSolutionProvider taskSolutionProvider, IResultArtifactHandler resultArtifactsHandler,
                                 TimeSpan? evaluationTimeout = null)
        {
            this.inputQueue = inputQueue;
            this.taskRunnerFactory = taskRunnerFactory;
            this.taskSolutionProvider = taskSolutionProvider;
            this.resultArtifactsHandler = resultArtifactsHandler;
            this.evaluationTimeout = evaluationTimeout ?? TimeSpan.FromMinutes(8);
        }

        public async Task RunContinuously(CancellationToken stop)
        {
            try
            {
                while (!stop.IsCancellationRequested)
                {
                    var evaluationTask = inputQueue.Dequeue(stop);
                    await processTask(evaluationTask);
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }

        public async Task ProcessAllQueue()
        {
            while (inputQueue.TryDequeue(out EvaluationTask evaluationTask))
            {
                await processTask(evaluationTask);
            }
        }

        private async Task<EvaluationResult> processTask(EvaluationTask evaluationTask)
        {
            try
            {
                if (shouldRun(evaluationTask))
                {
                    var solutionPath = taskSolutionProvider.GetSolutionPath(evaluationTask.StudentId);
                    var runnerTask = createRunnerTask(evaluationTask, solutionPath, resultArtifactsHandler.GetPathFor(evaluationTask.StudentId));
                    using (var taskRunner = taskRunnerFactory.CreateRunner(runnerTask))
                    {
                        var runnerResult = await taskRunner.Run();
                        return createEvaluationResultFrom(runnerResult);
                    }
                }
                else
                {
                    // TO-DO
                    return EvaluationResult.NotExecuted();
                }
            }
            catch (Exception ex)
            {
                // TO-DO
                return EvaluationResult.Failed(ex);
            }
        }

        private RunnerTask createRunnerTask(EvaluationTask evaluationTask, string solutionFolder, string resultFolder)
            => new RunnerTask(evaluationTask.EvaluationConfig.ImageName,
                              solutionFolder, evaluationTask.EvaluationConfig.SolutionDirectoryInContainer,
                              evaluationTask.EvaluationConfig.ResultInContainer, resultFolder,
                              TimeSpanHelper.Smaller(evaluationTask.EvaluationConfig.EvaluationTimeout, evaluationTimeout));

        private EvaluationResult createEvaluationResultFrom(RunnerResult runnerResult)
        {
            // TO-DO
            return new EvaluationResult();
        }

        private bool shouldRun(EvaluationTask task) => true;
    }
}
