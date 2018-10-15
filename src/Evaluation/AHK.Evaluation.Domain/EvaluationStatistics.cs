namespace AHK.Evaluation
{
    public class EvaluationStatistics
    {
        public readonly int AllTasks;
        public readonly int ExecutedSuccessfully;
        public readonly int FailedExecution;

        public EvaluationStatistics(int allTasks, int executedSuccessfully, int failedExecution)
        {
            AllTasks = allTasks;
            ExecutedSuccessfully = executedSuccessfully;
            FailedExecution = failedExecution;
        }

        public bool HasFailed() => FailedExecution > 0;
    }
}
