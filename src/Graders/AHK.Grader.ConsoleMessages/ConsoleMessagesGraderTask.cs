using System;

namespace AHK.Grader
{
    public class ConsoleMessagesGraderTask : GraderTask
    {
        public readonly string ConsoleLog;
        public readonly string ValidationCode;

        public ConsoleMessagesGraderTask(Guid taskId, string studentId, string consoleLog, string validationCode)
            : base(taskId, studentId)
        {
            this.ConsoleLog = consoleLog ?? throw new ArgumentNullException(nameof(consoleLog));
            this.ValidationCode = validationCode ?? throw new ArgumentNullException(nameof(validationCode));
        }
    }
}
