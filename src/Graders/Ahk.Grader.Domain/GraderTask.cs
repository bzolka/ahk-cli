namespace Ahk.Grader
{
    public abstract class GraderTask
    {
        protected GraderTask(string submissionSource, string studentId)
        {
            this.SubmissionSource = submissionSource;
            this.StudentId = studentId;
        }

        public string SubmissionSource { get; }
        public string StudentId { get; }

    }
}
