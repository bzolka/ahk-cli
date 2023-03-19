namespace AHK.Grader
{
    /// <summary>
    /// GraderResult 1 ---- * ExerciseResult 1 ---- * TestResult
    /// </summary>
    public class TestResult
    {
        public const string DefaultTestName = "N/A";

        public readonly string TestName;
        public readonly GradingOutcomes GradingOutcome;
        // BZ (based on code): this is always 0 or 1 for Trx parser, can only have larger than 1 value for the consolse parser.
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
