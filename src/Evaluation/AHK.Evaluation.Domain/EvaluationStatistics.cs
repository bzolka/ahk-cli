using System;

namespace AHK.Evaluation
{
    public class EvaluationStatistics
    {
        public int AllTasks { get; private set; }
        public int ExecutedSuccessfully { get; private set; }
        public int FailedExecution { get; private set; }

        public void OnExecutionFailed() => ++FailedExecution;
        public void OnExecutionCompleted() => ++ExecutedSuccessfully;
        public void OnExecutionStarted() => ++AllTasks;

        public bool HasFailed() => FailedExecution > 0;
    }
}
