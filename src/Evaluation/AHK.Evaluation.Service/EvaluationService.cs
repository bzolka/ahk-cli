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

        public EvaluationService(IEvaluatorInputQueue inputQueue, ITaskRunnerFactory taskRunnerFactory)
        {
            this.inputQueue = inputQueue;
            this.taskRunnerFactory = taskRunnerFactory;
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
                    var runnerTask = createRunnerTask(evaluationTask);
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

        private RunnerTask createRunnerTask(EvaluationTask evaluationTask)
            => new RunnerTask(evaluationTask.EvaluationConfig.ImageName,
                evaluationTask.SolutionFullPath, evaluationTask.EvaluationConfig.SolutionDirectoryInContainer,
                evaluationTask.EvaluationConfig.ResultInContainer, getPathForResults(evaluationTask),
                evaluationTask.EvaluationConfig.EvaluationTimeout);

        private static string getPathForResults(EvaluationTask evaluationTask)
            => System.IO.Path.Combine(System.IO.Path.GetDirectoryName(evaluationTask.SolutionFullPath),
                                      $"__Eredmenyek__{DateTime.Now.ToString("s").Replace(':', '-')}",
                                      System.IO.Path.GetFileName(evaluationTask.SolutionFullPath));

        private EvaluationResult createEvaluationResultFrom(RunnerResult runnerResult)
        {
            // TO-DO
            return new EvaluationResult();
        }

        private bool shouldRun(EvaluationTask task) => true;
    }
}
