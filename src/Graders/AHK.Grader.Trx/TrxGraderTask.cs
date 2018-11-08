using System;

namespace AHK.Grader
{
    public class TrxGraderTask : GraderTask
    {
        public readonly string TrxFilePath;

        public TrxGraderTask(Guid taskId, string studentId, string trxFilePath)
            : base(taskId, studentId)
        {
            this.TrxFilePath = trxFilePath ?? throw new ArgumentNullException(nameof(trxFilePath));
        }
    }
}
