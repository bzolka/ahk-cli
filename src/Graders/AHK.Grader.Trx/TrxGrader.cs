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

        public async Task GradeResult()
        {
            if (!System.IO.File.Exists(task.TrxFilePath))
            {
                logger.LogInformation($"No Trx file for task {task.TaskId} student {task.StudentId} at '{task.TrxFilePath}'");
                await handleNoResult();
            }
            else
            {
                logger.LogTrace($"Found Trx file for task {task.TaskId} student {task.StudentId} at '{task.TrxFilePath}'");
                await processTrxFile(task.TrxFilePath);
            }
        }

        public Task GradeFailedExecution(string errorMessage)
            => writeLine($"{task.StudentId};futasi hiba: {errorMessage}");

        private async Task processTrxFile(string trxFilePath)
        {
            var trxResult = await TrxReader.Read(trxFilePath);
            await writeResult(trxResult.Total, trxResult.Passed);
        }

        private Task handleNoResult()
            => writeResult(0, 0);

        private async Task writeResult(int allTestsExecuted, int testsPassed)
        {
            if (!System.IO.File.Exists(task.OutputFilePath))
                await writeCsvFileHeader();
            await writeLine(formatResult(allTestsExecuted, testsPassed));
        }

        private string formatResult(int allTestsExecuted, int testsPassed)
            => $"{task.StudentId};{allTestsExecuted};{testsPassed}";

        private Task writeCsvFileHeader()
            => writeLine("HallgatoAzonosito;OsszesTeszt;SikeresTeszt");

        private Task writeLine(string line)
            => System.IO.File.AppendAllTextAsync(task.OutputFilePath, line.Replace(Environment.NewLine, " ") + Environment.NewLine, System.Text.Encoding.UTF8);

        public void Dispose() { }
    }
}
