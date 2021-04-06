namespace Ahk.Grader
{
    public class TrxGraderTask : GraderTask
    {
        public TrxGraderTask(string submissionSource, string studentId, string trxFilePath)
            : base(submissionSource, studentId)
        {
            this.TrxFilePath = trxFilePath;
        }

        public string TrxFilePath { get; }
    }
}
