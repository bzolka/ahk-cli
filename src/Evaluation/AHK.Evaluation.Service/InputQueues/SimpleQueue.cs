using System.Collections.Concurrent;
using System.Threading;

namespace AHK.Evaluation.InputQueues
{
    internal class SimpleQueue : IEvaluatorInputQueue
    {
        private readonly BlockingCollection<EvaluationTask> queue = new BlockingCollection<EvaluationTask>(new ConcurrentQueue<EvaluationTask>());

        public void Enqueue(EvaluationTask task) => queue.Add(task);
        public bool HasAny() => queue.Count > 0;
        public EvaluationTask Dequeue(CancellationToken cancellationToken) => queue.Take(cancellationToken);
        public bool TryDequeue(out EvaluationTask task) => queue.TryTake(out task);
    }
}
