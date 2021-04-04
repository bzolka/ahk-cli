using System;

namespace Ahk.TaskRunner
{
    public class RunnerResult
    {
        public readonly string ConsoleOutput;
        public readonly Exception Exception;

        public RunnerResult(string consoleOutput)
        {
            this.ConsoleOutput = consoleOutput;
        }

        public RunnerResult(string consoleOutput, Exception ex)
            : this(consoleOutput)
        {
            this.Exception = ex;
        }

        public bool HadError() => Exception != null;

        public static RunnerResult Failed(string consoleOutput, Exception ex) => new RunnerResult(consoleOutput, ex);
        public static RunnerResult Timeout(string consoleOutput) => new RunnerResult(consoleOutput, new TimeoutException());
        public static RunnerResult Success(string consoleOutput) => new RunnerResult(consoleOutput);
    }
}
