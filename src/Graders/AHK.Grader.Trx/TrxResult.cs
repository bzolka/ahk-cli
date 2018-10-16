using System.Collections.Generic;

namespace AHK.Grader
{
    public class TrxResult
    {
        public readonly int Total;
        public readonly int Passed;
        public readonly IReadOnlyList<string> Failed;

        public TrxResult(int total, int passed, IReadOnlyList<string> failedNames)
        {
            this.Total = total;
            this.Passed = passed;
            this.Failed = failedNames ?? new string[0];
        }
    }
}
