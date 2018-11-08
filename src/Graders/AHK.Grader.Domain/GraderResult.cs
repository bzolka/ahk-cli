using System.Collections.Generic;

namespace AHK.Grader
{
    public class GraderResult
    {
        public readonly bool GradingSuccessful;
        public readonly int Grade;
        public readonly IReadOnlyList<string> FailedTestNames;

        public GraderResult(int grade, IReadOnlyList<string> failedTestNames)
        {
            this.GradingSuccessful = true;
            this.Grade = grade;
            this.FailedTestNames = failedTestNames;
        }

        private GraderResult()
        {
            this.GradingSuccessful = false;
        }

        public static readonly GraderResult NoResult = new GraderResult();
    }
}
