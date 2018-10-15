using System;
using System.Threading.Tasks;
using AHK.Evaluation.Grader;
using Microsoft.Extensions.Logging;

namespace AHK
{
    internal static class RunCommand
    {
        public static async Task<int> Execute(string assignmentsDir, string resultsDir, AppConfig appConfig, ILoggerFactory loggerFactory)
        {
            Console.WriteLine("Reading tasks...");

            var tasksToEvaluate = EvaluationTaskReaderFromDisk.ReadFrom(assignmentsDir, resultsDir);

            var es = new Evaluation.EvaluationService(tasksToEvaluate,
                        new DockerRunnerFactory(loggerFactory),
                        new TrxGraderFactory(),
                        loggerFactory.CreateLogger<Evaluation.EvaluationService>(),
                        appConfig.MaxTaskRuntime);

            Console.WriteLine("Running evaluation...");

            var execStatistics = await es.ProcessAllQueue();

            Console.WriteLine("Finished.");
            if (execStatistics.HasFailed())
            {
                Console.WriteLine("Some evaluations FAILED (indicates program or configuration error)");
                Console.WriteLine($"Solution tested: {execStatistics.AllTasks} / {execStatistics.ExecutedSuccessfully} executed without issues, {execStatistics.FailedExecution} FAILED to execute");
                return -1;
            }
            else
            {
                Console.WriteLine($"Executed {execStatistics.ExecutedSuccessfully} evaluations");
                return 0;
            }
        }

        private class DockerRunnerFactory : TaskRunner.ITaskRunnerFactory
        {
            private ILoggerFactory loggerFactory;

            public DockerRunnerFactory(ILoggerFactory loggerFactory)
                => this.loggerFactory = loggerFactory;

            public Task Cleanup(ILogger logger) => TaskRunner.DockerCleanup.Cleanup(logger);

            public TaskRunner.ITaskRunner CreateRunner(TaskRunner.RunnerTask task)
                => new TaskRunner.DockerRunner(task, loggerFactory.CreateLogger<TaskRunner.DockerRunner>());
        }
    }
}
