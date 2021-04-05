using System.Collections.Generic;

namespace Ahk.Grader
{
    /// <summary>
    /// GraderResult 1 ---- * ExerciseResult 1 ---- * TestResult
    /// </summary>
    public class GraderResult
    {
        public readonly bool HasResult;
        public readonly IReadOnlyList<ExerciseResult>? Exercises;

        public static readonly GraderResult NoResult = new GraderResult();

        public GraderResult(IReadOnlyList<ExerciseResult> exercises)
        {
            this.HasResult = exercises?.Count > 0;
            this.Exercises = exercises ?? new List<ExerciseResult>();
        }

        private GraderResult()
        {
            this.HasResult = false;
        }

        public GradingOutcomes GradingOutcome => this.HasResult ? this.Exercises!.AccumulateOutcome() : GradingOutcomes.FailedToGrade;
    }
}
