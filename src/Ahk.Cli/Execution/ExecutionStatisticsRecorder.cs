namespace Ahk.Execution
{
    public partial class ExecutionStatisticsRecorder
    {
        public int AllTasks { get; private set; }
        public int ExecutedSuccessfully { get; private set; }
        public int FailedExecution { get; private set; }

        public ExecutionStatistics GetEvaluationStatistics()
            => new ExecutionStatistics(AllTasks, ExecutedSuccessfully, FailedExecution);

        public ExecutionStatisticsRecorderScope OnExecutionStarted()
        {
            ++AllTasks;
            return new ExecutionStatisticsRecorderScope(this);
        }
    }
}
