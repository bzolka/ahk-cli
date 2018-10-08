using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace AHK
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var appConfig = getAppConfig(args);

            var (tasksToEvaluate, taskSolutionProvider) = EvaluationTaskReaderFromDisk.ReadFrom(appConfig.AssignmentsDir);

            await new Evaluation.EvaluationService(tasksToEvaluate, new StaticDockerRunnerFactory(), taskSolutionProvider, new Evaluation.FilesystemResultArtifactHandler(appConfig.ResultsDir))
                                    .ProcessAllQueue();
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
            public TaskRunner.ITaskRunner CreateRunner(TaskRunner.RunnerTask task) => new TaskRunner.DockerRunner(task);
        }
    }
}
