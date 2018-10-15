using System;

namespace AHK.Evaluation
{
    public class EvaluationTask
    {
        public readonly Guid TaskId = Guid.NewGuid();
        public readonly string StudentId;
        public readonly string SolutionPath;
        public readonly string ResultArtifactPath;
        public readonly EvaluationConfig EvaluationConfig;

        public EvaluationTask(string studentId, string solutionPath, string resultArtifactPath, EvaluationConfig evaluationConfig)
        {
            this.StudentId = studentId ?? throw new ArgumentNullException(nameof(studentId));
            this.SolutionPath = solutionPath ?? throw new ArgumentNullException(nameof(solutionPath));
            this.ResultArtifactPath = resultArtifactPath ?? throw new ArgumentNullException(nameof(resultArtifactPath));
            this.EvaluationConfig = evaluationConfig ?? throw new ArgumentNullException(nameof(evaluationConfig));
        }
    }
}
