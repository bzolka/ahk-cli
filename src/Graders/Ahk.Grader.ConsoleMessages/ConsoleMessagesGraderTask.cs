using System;

namespace Ahk.Grader
{
    public class ConsoleMessagesGraderTask : GraderTask
    {
        public readonly string ConsoleLog;
        public readonly string ValidationCode;

        public ConsoleMessagesGraderTask(Guid taskId, string studentName, string studentNeptun, string consoleLog, string validationCode)
            : base(taskId, studentName, studentNeptun)
        {
            this.ConsoleLog = consoleLog ?? throw new ArgumentNullException(nameof(consoleLog));
            this.ValidationCode = validationCode ?? throw new ArgumentNullException(nameof(validationCode));
        }
    }
}
