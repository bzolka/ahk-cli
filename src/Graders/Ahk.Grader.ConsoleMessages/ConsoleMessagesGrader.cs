using System;
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
            this.task = task ?? throw new ArgumentNullException(nameof(task));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<GraderResult> GradeResult()
        {
            if (string.IsNullOrEmpty(task.ConsoleLog))
            {
                logger.LogInformation($"Empty console log for task {task.TaskId} student {task.StudentNeptun}");
                return GraderResult.NoResult;
            }
            else
            {
                var reader = new ConsoleMessages.ConsoleMessagesReader(task.ValidationCode, logger);
                var result = await reader.Grade(task.ConsoleLog);

                if (!result.HasResult)
                    logger.LogInformation($"No result parsed from console log for task {task.TaskId} student {task.StudentNeptun}");

                return result;
            }
        }

        public void Dispose() { }
    }
}
