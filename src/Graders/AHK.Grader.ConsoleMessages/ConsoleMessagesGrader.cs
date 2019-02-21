using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AHK.Grader
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
                logger.LogInformation($"Empty console log for task {task.TaskId} student {task.StudentId}");
                return GraderResult.NoResult;
            }
            else
            {
                var reader = new ConsoleMessagesReader(task.ValidationCode, logger);
                var result = await reader.Read(task.ConsoleLog);
                return new GraderResult(result.Passed, result.FailedTestNames);
            }
        }

        public void Dispose() { }
    }
}
