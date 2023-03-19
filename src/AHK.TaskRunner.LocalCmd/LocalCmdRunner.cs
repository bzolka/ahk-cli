using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;


namespace AHK.TaskRunner
{
    public class LocalCmdRunner : ITaskRunner
    {
        private readonly LocalCmdRunnerTask task;
        private readonly ILogger logger;
        private readonly string commandToExecute;
        private readonly string commandArguments;
        private readonly Dictionary<string, string> environmentVariables;

        public LocalCmdRunner(LocalCmdRunnerTask task, ILogger logger)
        {
            this.task = task ?? throw new ArgumentNullException(nameof(task));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            logger.LogTrace("Created LocalCmd runner for {TaskId}", task.TaskId);
            (commandToExecute, commandArguments) = getCommandStringAndArguments_ReplacePlaceholders();
            environmentVariables = getEnvironmentVariablesReplacePlaceHolders();
        }

        public async Task<RunnerResult> Run()
        {
            // Note: 
            // We need to mess with environment variables because overriding runsettings parameters does not work yet as expected
            // see https://github.com/Microsoft/vstest/issues/862. Expected to be released with SDK 16.7
            // When we have that at hand pass AppId in command line arguments the dotnet test instead of using environment variable
            try
            {
                //using (var timeout = new CancellationTokenSource(task.EvaluationTimeout))
                //{
                    // TODO-BZ: Ha timeout van, az most nem derül ki itt!!!
                    string consoleLog = string.Empty;
                    try
                    {
                        // var envVariables = new Dictionary<string, string>() { { "AppId", Path.Combine(task.SolutionPath, "Feladatok\\WinFormExpl\\bin\\Debug\\netcoreapp3.1\\ProjectFileForTest.exe")} };
                        var processResult = await ProcessAsyncHelper.RunProcessAsync(commandToExecute, commandArguments,
                            (int)task.EvaluationTimeout.TotalMilliseconds, environmentVariables); 
                        consoleLog = processResult.OutputAndError;
                    }
                    finally
                    {
                        // Nothing to clean up, cancellation/timeout already killed the process
                    }

                    logger.LogInformation("LocalCmd runner finished");
                    return RunnerResult.Success(consoleLog);
                //}
            }
            catch (OperationCanceledException) // todo-bz
            {
                logger.LogWarning("LocalCmd runner timeout");
                return RunnerResult.Timeout(string.Empty);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "LocalCmd runner failed");
                return RunnerResult.Failed(string.Empty, ex);
            }
        }

        public void Dispose()
        {
            
        }

        private (string, string) getCommandStringAndArguments_ReplacePlaceholders()
        {
            // TOIMPROVE: prepare for quoted command
            // TOIMPROVE: add support for command line argumenst with spaces

            string cmd = task.Command;
            string cmdArgs = string.Empty;

            cmd = cmd.Trim();
            int commandEndIndex = cmd.IndexOf(' ');
            if (commandEndIndex != -1 && commandEndIndex < cmd.Length-1)
            {
                cmdArgs = cmd.Substring(commandEndIndex + 1);
                cmd = cmd.Substring(0, commandEndIndex);
            }

            // Replace placeholders
            cmdArgs = cmdArgs
                .Replace("{OutDirectory}", task.ResultArtifactPath)
                .Replace("{SolutionDirectory}", task.SolutionPath);
            return (cmd, cmdArgs);
        }

        private Dictionary<string, string>  getEnvironmentVariablesReplacePlaceHolders()
        {
            if (task.EnvironmentVariables == null)
                return null;

            return task.EnvironmentVariables.ToDictionary(e =>e.Key,
                e => e.Value
                    .Replace("{OutDirectory}", task.ResultArtifactPath)
                    .Replace("{SolutionDirectory}", task.SolutionPath)
                );
        }

    }
}
