using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Ahk.Grader
{
    public class TrxGrader : IGrader
    {
        private readonly TrxGraderTask task;
        private readonly ILogger logger;

        public TrxGrader(TrxGraderTask task, ILogger logger)
        {
            this.task = task;
            this.logger = logger;
        }

        public async Task<GraderResult> GradeResult()
        {
            if (!System.IO.File.Exists(task.TrxFilePath))
            {
                logger.LogInformation($"No trx file found for submission {task.SubmissionSource} student {task.StudentId} at '{task.TrxFilePath}'");
                return GraderResult.NoResult;
            }
            else
            {
                logger.LogTrace($"Found trx file for submission {task.SubmissionSource} student {task.StudentId} at '{task.TrxFilePath}'");

                var trxReader = new Trx.TrxReader(logger);
                var result = await trxReader.Parse(task.TrxFilePath);

                if (!result.HasResult)
                    logger.LogInformation($"No tests parsed from Trx for submission {task.SubmissionSource} student {task.StudentId} at '{task.TrxFilePath}'");

                return result;
            }
        }

        public void Dispose() { }
    }
}
