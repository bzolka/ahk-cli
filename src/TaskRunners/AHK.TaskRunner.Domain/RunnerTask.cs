using System;

namespace AHK.TaskRunner
{
    public abstract class RunnerTask
    {
        public readonly Guid TaskId;
        public readonly TimeSpan EvaluationTimeout;

        public RunnerTask(Guid taskId, TimeSpan evaluationTimeout)
        {
            this.TaskId = taskId;
            this.EvaluationTimeout = evaluationTimeout;
        }
    }
}
