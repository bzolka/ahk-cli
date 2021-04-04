namespace Ahk.Grader
{
    /// <summary>
    /// GraderResult 1 ---- * ExerciseResult 1 ---- * TestResult
    /// </summary>
    public class TestResult
    {
        public const string DefaultTestName = "N/A";

        public readonly string TestName;
        public readonly GradingOutcomes GradingOutcome;
        public readonly int ResultPoints;
        public readonly string Description;

        public TestResult(string testName, int resultPoints, GradingOutcomes gradingOutcome, string description)
        {
            this.TestName = string.IsNullOrEmpty(testName) ? DefaultTestName : testName;
            this.ResultPoints = resultPoints;
            this.GradingOutcome = gradingOutcome;
            this.Description = description ?? string.Empty;
        }
    }
}
