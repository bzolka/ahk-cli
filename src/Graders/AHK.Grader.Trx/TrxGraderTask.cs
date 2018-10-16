using System;

namespace AHK.Grader
{
    public class TrxGraderTask : GraderTask
    {
        public readonly string TrxFilePath;
        public readonly string OutputFilePath;

        public TrxGraderTask(Guid taskId, string studentId, string trxFilePath, string outputFilePath)
            : base(taskId, studentId)
        {
            this.TrxFilePath = trxFilePath ?? throw new ArgumentNullException(nameof(trxFilePath));
            this.OutputFilePath = outputFilePath ?? throw new ArgumentNullException(nameof(outputFilePath));
        }
    }
}
