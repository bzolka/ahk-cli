using System;

namespace AHK.TaskRunner
{
    public class RunnerResult
    {
        public readonly Exception Exception;

        public RunnerResult(Exception ex) => this.Exception = ex;
        public RunnerResult() { }

        public bool HadError() => Exception != null;

        public static RunnerResult Failed(Exception ex) => new RunnerResult(ex);
        public static RunnerResult Timeout() => new RunnerResult(new TimeoutException());
        public static RunnerResult Success() => new RunnerResult();
    }
}
