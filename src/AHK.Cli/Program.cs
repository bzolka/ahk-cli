using Microsoft.Extensions.Configuration;

namespace AHK
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var appConfig = getAppConfig(args);

            var inputQueue = Evaluation.EvaluatorInputQueueFactory.Create();
            foreach (var evalTask in EvaluationTaskReaderFromDisk.ReadFrom(appConfig.AssignmentsDir))
                inputQueue.Enqueue(evalTask);

            await new Evaluation.EvaluationService(inputQueue, new StaticDockerRunnerFactory())
                                    .ProcessAllQueue();
        }

        private static AppConfig getAppConfig(string[] commandLineArgs)
        {
            var configRoot = new ConfigurationBuilder()
                                    .AddJsonFile(@"AppConfig.json", optional: true, reloadOnChange: false)
                                    .AddEnvironmentVariables("AHK_")
                                    .AddCommandLine(commandLineArgs)
                                    .Build();
            var appConfig = new AppConfig();
            configRoot.Bind(appConfig);
            return appConfig;
        }

        private class StaticDockerRunnerFactory : TaskRunner.ITaskRunnerFactory
        {
            public TaskRunner.ITaskRunner CreateRunner(TaskRunner.RunnerTask task) => new TaskRunner.DockerRunner(task);
        }
    }
}
