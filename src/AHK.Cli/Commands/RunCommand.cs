using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AHK
{
    internal static class RunCommand
    {
        public static async Task<int> Execute(string assignmentsDir, string resultsDir, AppConfig appConfig, ILoggerFactory loggerFactory)
        {
            Console.WriteLine("Reading tasks...");

            var (tasksToEvaluate, taskSolutionProvider) = EvaluationTaskReaderFromDisk.ReadFrom(assignmentsDir);

            var es = new Evaluation.EvaluationService(tasksToEvaluate,
                        new StaticDockerRunnerFactory(loggerFactory),
                        taskSolutionProvider,
                        new Evaluation.FilesystemResultArtifactHandler(resultsDir),
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

        private class StaticDockerRunnerFactory : TaskRunner.ITaskRunnerFactory
        {
            private ILoggerFactory loggerFactory;

            public StaticDockerRunnerFactory(ILoggerFactory loggerFactory)
                => this.loggerFactory = loggerFactory;

            public Task Cleanup(ILogger logger) => TaskRunner.DockerCleanup.Cleanup(logger);

            public TaskRunner.ITaskRunner CreateRunner(TaskRunner.RunnerTask task)
                => new TaskRunner.DockerRunner(task, loggerFactory.CreateLogger<TaskRunner.DockerRunner>());
        }
    }
}
