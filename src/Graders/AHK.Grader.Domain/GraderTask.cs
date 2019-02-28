using System;

namespace AHK.Grader
{
    public abstract class GraderTask
    {
        public readonly Guid TaskId;
        public readonly string StudentName;
        public readonly string StudentNeptun;

        public GraderTask(Guid taskId, string studentName, string studentNeptun)
        {
            this.TaskId = taskId;
            this.StudentName = studentName ?? throw new ArgumentNullException(nameof(studentName));
            this.StudentNeptun = studentNeptun ?? throw new ArgumentNullException(nameof(studentNeptun));
        }
    }
}
