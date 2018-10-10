using System;

namespace AHK.Evaluation.Grader
{
    public class TrxGraderOptions
    {
        public readonly string TrxFileName;
        public readonly string OutputFile;

        public TrxGraderOptions(string trxFileName, string outputFileName)
        {
            this.TrxFileName = trxFileName ?? throw new ArgumentNullException(nameof(trxFileName));
            this.OutputFile = outputFileName ?? throw new ArgumentNullException(nameof(outputFileName));
        }
    }
}
