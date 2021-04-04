namespace Ahk.Execution
{
    public class ExecutionStatistics
    {
        public readonly int AllTasks;
        public readonly int ExecutedSuccessfully;
        public readonly int FailedExecution;

        public ExecutionStatistics(int allTasks, int executedSuccessfully, int failedExecution)
        {
            AllTasks = allTasks;
            ExecutedSuccessfully = executedSuccessfully;
            FailedExecution = failedExecution;
        }

        public bool HasFailed() => FailedExecution > 0;
    }
}
