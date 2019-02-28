using System;

namespace AHK.Grader
{
    public class TrxGraderTask : GraderTask
    {
        public readonly string TrxFilePath;

        public TrxGraderTask(Guid taskId, string studentName, string studentNeptun, string trxFilePath)
            : base(taskId, studentName, studentNeptun)
        {
            this.TrxFilePath = trxFilePath ?? throw new ArgumentNullException(nameof(trxFilePath));
        }
    }
}
