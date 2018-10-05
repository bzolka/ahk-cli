using System;

namespace AHK.Evaluation
{
    public class EvaluationTask
    {
        public readonly Guid Id = Guid.NewGuid();
        public readonly string StudentId;
        public readonly string SolutionFullPath;
        public readonly EvaluationConfig EvaluationConfig;

        public EvaluationTask(string studentId, string solutionFullPath, EvaluationConfig evaluationConfig)
        {
            this.StudentId = studentId;
            this.SolutionFullPath = solutionFullPath;
            this.EvaluationConfig = evaluationConfig;
        }
    }
}
