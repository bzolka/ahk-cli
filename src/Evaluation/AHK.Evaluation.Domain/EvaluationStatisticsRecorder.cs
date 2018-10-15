namespace AHK.Evaluation
{
    public partial class EvaluationStatisticsRecorder
    {
        public int AllTasks { get; private set; }
        public int ExecutedSuccessfully { get; private set; }
        public int FailedExecution { get; private set; }

        public EvaluationStatistics GetEvaluationStatistics()
            => new EvaluationStatistics(AllTasks, ExecutedSuccessfully, FailedExecution);

        public EvaluationStatisticsRecorderScope OnExecutionStarted()
        {
            ++AllTasks;
            return new EvaluationStatisticsRecorderScope(this);
        }
    }
}
