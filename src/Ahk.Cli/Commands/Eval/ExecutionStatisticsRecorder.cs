namespace Ahk.Commands.Eval
{
    public partial class ExecutionStatisticsRecorder
    {
        public int AllTasks { get; private set; }
        public int ExecutedSuccessfully { get; private set; }
        public int FailedExecution { get; private set; }

        public bool HasFailed() => FailedExecution > 0;

        public ExecutionStatisticsRecorderScope OnExecutionStarted()
        {
            ++AllTasks;
            return new ExecutionStatisticsRecorderScope(this);
        }
    }
}
