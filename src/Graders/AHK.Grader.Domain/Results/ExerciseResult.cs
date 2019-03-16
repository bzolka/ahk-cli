using System.Collections.Generic;
using System.Linq;

namespace AHK.Grader
{
    /// <summary>
    /// GraderResult 1 ---- * ExerciseResult 1 ---- * TestResult
    /// </summary>
    public class ExerciseResult
    {
        public const string DefaultExerciseName = @"N/A";

        public readonly string ExerciseName;
        public readonly IReadOnlyList<TestResult> TestsResults;

        public ExerciseResult(string exerciseName, IReadOnlyList<TestResult> testResults)
        {
            this.ExerciseName = exerciseName ?? DefaultExerciseName;
            this.TestsResults = testResults ?? new List<TestResult>();
        }

        public int SumResultPoints => this.TestsResults.Sum(tr => tr.ResultPoints);
        public GradingOutcomes GradingOutcome => this.TestsResults.AccumulateOutcome();
    }
}
