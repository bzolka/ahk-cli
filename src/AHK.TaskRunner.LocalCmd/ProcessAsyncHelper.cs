using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AHK.TaskRunner
{
    // Based on https://gist.github.com/georg-jung/3a8703946075d56423e418ea76212745
    // (another variant is available here https://gist.github.com/Indigo744/b5f3bd50df4b179651c876416bf70d0a)
    // This could also be used: https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.process.exited?view=netcore-2.1
    public static class ProcessAsyncHelper
    {
        public static async Task<ProcessResult> RunProcessAsync(string command, string arguments, int timeout, Dictionary<string, string> envVariables = null)
        {
            var result = new ProcessResult();

            using (var process = new Process())
            {
                process.StartInfo.FileName = command;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.UseShellExecute = false;
                //process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;

                if (envVariables != null)
                    foreach (var e in envVariables)
                        process.StartInfo.EnvironmentVariables[e.Key] = e.Value;

                var outputAndError = new StringBuilder();

                var outputBuilder = new StringBuilder();
                var outputCloseEvent = new TaskCompletionSource<bool>();

                process.OutputDataReceived += (s, e) =>
                {
                    if (e.Data == null)
                    {
                        outputCloseEvent.SetResult(true);
                    }
                    else
                    {
                        outputBuilder.Append(e.Data);
                        outputAndError.Append(e.Data);
                    }
                };

                var errorBuilder = new StringBuilder();
                var errorCloseEvent = new TaskCompletionSource<bool>();

                process.ErrorDataReceived += (s, e) =>
                {
                    if (e.Data == null)
                    {
                        errorCloseEvent.SetResult(true);
                    }
                    else
                    {
                        errorBuilder.Append(e.Data);
                        outputAndError.Append(e.Data);
                    }
                };

                var isStarted = process.Start();
                if (!isStarted)
                {
                    result.ExitCode = process.ExitCode;
                    return result;
                }

                // Reads the output stream first and then waits because deadlocks are possible
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // Creates task to wait for process exit using timeout
                var waitForExit = WaitForExitAsync(process, timeout);

                // Create task to wait for process exit and closing all output streams
                var processTask = Task.WhenAll(waitForExit, outputCloseEvent.Task, errorCloseEvent.Task);

                // Waits process completion and then checks it was not completed by timeout
                if (await Task.WhenAny(Task.Delay(timeout), processTask) == processTask && waitForExit.Result)
                {
                    result.ExitCode = process.ExitCode;
                    result.Output = outputBuilder.ToString();
                    result.Error = errorBuilder.ToString();
                    result.OutputAndError = outputAndError.ToString();
                }
                else
                {
                    try
                    {
                        // Kill hung process
                        process.Kill();
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }

            return result;
        }


        private static Task<bool> WaitForExitAsync(Process process, int timeout)
        {
            return Task.Run(() => process.WaitForExit(timeout));
        }


        public struct ProcessResult
        {
            public int? ExitCode;
            public string Output;
            public string Error;
            public string OutputAndError;
        }
    }
}
