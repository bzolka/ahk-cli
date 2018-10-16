using System;

namespace AHK.Grader
{
    public abstract class GraderTask
    {
        public readonly Guid TaskId;
        public readonly string StudentId;

        public GraderTask(Guid taskId, string studentId)
        {
            this.TaskId = taskId;
            this.StudentId = studentId ?? throw new ArgumentNullException(nameof(studentId));
        }
    }
}
