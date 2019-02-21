using System.Collections.Generic;

namespace AHK.Grader
{
    public class ConsoleMessagesTestResult
    {
        public readonly int Total;
        public readonly int Passed;
        public readonly IReadOnlyList<string> FailedTestNames;

        public ConsoleMessagesTestResult(int total, int passed, IReadOnlyList<string> failedNames)
        {
            this.Total = total;
            this.Passed = passed;
            this.FailedTestNames = failedNames ?? new string[0];
        }
    }
}
