using System.Threading;

namespace AHK.Evaluation
{
    public interface IEvaluatorInputQueue
    {
        void Enqueue(EvaluationTask task);

        bool HasAny();
        bool TryDequeue(out EvaluationTask task);
        EvaluationTask Dequeue(CancellationToken cancellationToken);
    }
}
