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
                logger.LogInformation($"No Trx file for task {task.TaskId} student {task.StudentId} at '{task.TrxFilePath}'");
                return GraderResult.NoResult;
            }
            else
            {
                logger.LogTrace($"Found Trx file for task {task.TaskId} student {task.StudentId} at '{task.TrxFilePath}'");
                var trxResult = await TrxReader.Read(task.TrxFilePath);
                return new GraderResult(trxResult.Passed, trxResult.FailedTestNames);
            }
        }

        public void Dispose() { }
    }
}
