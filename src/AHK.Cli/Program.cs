using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AHK
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var appConfig = getAppConfig(args);

            Console.WriteLine("Reading tasks...");

            var (tasksToEvaluate, taskSolutionProvider) = EvaluationTaskReaderFromDisk.ReadFrom(appConfig.AssignmentsDir);

            var loggerFactory = new LoggerFactory()
                        .AddConsole(LogLevel.Warning, true);

            var es = new Evaluation.EvaluationService(tasksToEvaluate,
                        new StaticDockerRunnerFactory(loggerFactory),
                        taskSolutionProvider,
                        new Evaluation.FilesystemResultArtifactHandler(appConfig.ResultsDir),
                        loggerFactory.CreateLogger<Evaluation.EvaluationService>(),
                        appConfig.MaxTaskRuntime);

            Console.WriteLine("Running evaluation...");

            var execStatistics = await es.ProcessAllQueue();

            Console.WriteLine("Finished.");
            if (execStatistics.HasFailed())
            {
                Console.WriteLine("Some evaluations FAILED (indicates program or configuration error)");
                Console.WriteLine($"Solution tested: {execStatistics.AllTasks} / {execStatistics.ExecutedSuccessfully} executed without issues, {execStatistics.FailedExecution} FAILED to execute");
            }
            else
            {
                Console.WriteLine($"Executed {execStatistics.ExecutedSuccessfully} evaluations");
            }
        }

        private static AppConfig getAppConfig(string[] commandLineArgs)
        {
            var configRoot = new ConfigurationBuilder()
                                    .AddJsonFile(@"AppConfig.json", optional: true, reloadOnChange: false)
                                    .AddEnvironmentVariables("AHK_")
                                    .AddCommandLine(commandLineArgs, getCommandLineSwitchMappings())
                                    .Build();
            var appConfig = new AppConfig();
            configRoot.Bind(appConfig);
            return appConfig;
        }

        private static IDictionary<string, string> getCommandLineSwitchMappings()
            => new Dictionary<string, string>()
            {
                { "-m", "AssignmentsDir" },
                { "-e", "ResultsDir" }
            };

        private class StaticDockerRunnerFactory : TaskRunner.ITaskRunnerFactory
        {
            private ILoggerFactory loggerFactory;

            public StaticDockerRunnerFactory(ILoggerFactory loggerFactory)
                => this.loggerFactory = loggerFactory;

            public TaskRunner.ITaskRunner CreateRunner(TaskRunner.RunnerTask task)
                => new TaskRunner.DockerRunner(task, loggerFactory.CreateLogger<TaskRunner.DockerRunner>());
        }
    }
}
