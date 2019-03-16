using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AHK.Grader
{
    public class TrxGrader : IGrader
    {
        private readonly TrxGraderTask task;
        private readonly ILogger logger;

        public TrxGrader(TrxGraderTask task, ILogger logger)
        {
            this.task = task ?? throw new ArgumentNullException(nameof(task));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<GraderResult> GradeResult()
        {
            if (!System.IO.File.Exists(task.TrxFilePath))
            {
                logger.LogInformation($"No Trx file for task {task.TaskId} student {task.StudentNeptun} at '{task.TrxFilePath}'");
                return GraderResult.NoResult;
            }
            else
            {
                logger.LogTrace($"Found Trx file for task {task.TaskId} student {task.StudentNeptun} at '{task.TrxFilePath}'");

                var trxReader = new Trx.TrxReader(logger);
                var result = await trxReader.Parse(task.TrxFilePath);

                if (!result.HasResult)
                    logger.LogInformation($"No tests parsed from Trx for task {task.TaskId} student {task.StudentNeptun} at '{task.TrxFilePath}'");

                return result;
            }
        }

        public void Dispose() { }
    }
}
