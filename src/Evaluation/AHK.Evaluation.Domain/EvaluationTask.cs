using System;

namespace AHK.Evaluation
{
    public class EvaluationTask
    {
        public readonly Guid TaskId = Guid.NewGuid();
        public readonly string StudentId;
        public readonly EvaluationConfig EvaluationConfig;

        public EvaluationTask(string studentId, EvaluationConfig evaluationConfig)
        {
            this.StudentId = studentId ?? throw new ArgumentNullException(nameof(studentId));
            this.EvaluationConfig = evaluationConfig ?? throw new ArgumentNullException(nameof(evaluationConfig));
        }
    }
}
