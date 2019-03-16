using System.Collections.Generic;
using System.Linq;

namespace AHK.Grader
{
    public static class GradingOutcomeExtensions
    {
        public static GradingOutcomes AccumulateOutcome(this IEnumerable<GradingOutcomes> values)
        {
            if (values.All(v => v == GradingOutcomes.Graded))
                return GradingOutcomes.Graded;

            if (values.Any(v => v == GradingOutcomes.FailedToGrade))
                return GradingOutcomes.FailedToGrade;

            return GradingOutcomes.Inconclusive;
        }

        public static GradingOutcomes AccumulateOutcome(this IEnumerable<TestResult> testResults)
            => testResults.Select(tr => tr.GradingOutcome).AccumulateOutcome();

        public static GradingOutcomes AccumulateOutcome(this IEnumerable<ExerciseResult> exResults)
            => exResults.Select(tr => tr.GradingOutcome).AccumulateOutcome();
    }
}
