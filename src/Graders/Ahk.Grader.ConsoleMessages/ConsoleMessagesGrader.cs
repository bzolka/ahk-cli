using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Ahk.Grader
{
    public class ConsoleMessagesGrader : IGrader
    {
        private readonly ConsoleMessagesGraderTask task;
        private readonly ILogger logger;

        public ConsoleMessagesGrader(ConsoleMessagesGraderTask task, ILogger logger)
        {
            this.task = task;
            this.logger = logger;
        }

        public async Task<GraderResult> GradeResult()
        {
            if (string.IsNullOrEmpty(task.ConsoleLog))
            {
                logger.LogInformation($"Empty console log for submission {task.SubmissionSource} student {task.StudentId}");
                return GraderResult.NoResult;
            }
            else
            {
                var reader = new ConsoleMessages.ConsoleMessagesReader(task.ValidationCode, logger);
                var result = await reader.Grade(task.ConsoleLog);

                if (!result.HasResult)
                    logger.LogInformation($"No result parsed from console log for source {task.SubmissionSource} student {task.StudentId}");

                return result;
            }
        }

        public void Dispose() { }
    }
}
