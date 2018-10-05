namespace AHK.Evaluation
{
    public static class EvaluatorInputQueueFactory
    {
        public static IEvaluatorInputQueue Create() => new InputQueues.SimpleQueue();
    }
}
