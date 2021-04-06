using System;

namespace Ahk.TaskRunner
{
    public abstract class RunnerTask
    {
        protected RunnerTask(string submissionSource, string studentId, TimeSpan evaluationTimeout)
        {
            this.SubmissionSource = submissionSource;
            this.StudentId = studentId;
            this.EvaluationTimeout = evaluationTimeout;

        }

        public string SubmissionSource { get; }
        public string StudentId { get; }
        public TimeSpan EvaluationTimeout { get; }

    }
}
