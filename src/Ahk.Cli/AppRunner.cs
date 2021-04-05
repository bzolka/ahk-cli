using System.Threading.Tasks;
using Ahk.Execution;
using Microsoft.Extensions.Logging;

namespace Ahk
{
    internal static class AppRunner
    {
        public static async Task<ExecutionStatistics> Go(string assignmentsDir, string executionConfigFile, string resultsDir, AppConfig appConfig, ConsoleProgress.IConsoleProgress progress, ILogger logger)
        {
            var progressLoad = progress.BeginTask("Loading submissions");
            var runConfig = JobsLoader.Load(assignmentsDir, executionConfigFile, resultsDir, logger);
            progressLoad.SetProgress(1, 1);

            var progressExec = progress.BeginTask("Running evaluation");
            var executor = new Executor(logger, appConfig.MaxTaskRuntime);
            return await executor.Execute(runConfig, progressExec.SetProgress);
        }
    }
}
