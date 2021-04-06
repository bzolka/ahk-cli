namespace Ahk.Grader
{
    public class ConsoleMessagesGraderTask : GraderTask
    {
        public ConsoleMessagesGraderTask(string submissionSource, string studentId, string consoleLog, string validationCode)
            : base(submissionSource, studentId)
        {
            this.ConsoleLog = consoleLog;
            this.ValidationCode = validationCode;
        }

        public string ConsoleLog { get; }
        public string ValidationCode { get; }
    }
}
