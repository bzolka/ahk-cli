using System;
using System.Threading.Tasks;

namespace AHK.Evaluation.Grader
{
    public class TrxGrader : IGrader
    {
        private readonly EvaluationTask evaluationTask;
        private readonly TrxGraderOptions options;

        public TrxGrader(EvaluationTask evaluationTask, TrxGraderOptions options)
        {
            this.evaluationTask = evaluationTask ?? throw new ArgumentNullException(nameof(evaluationTask));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task GradeResult(string directory)
        {
            var trxFilePath = System.IO.Path.Combine(directory, options.TrxFileName);
            if (!System.IO.File.Exists(trxFilePath))
                await handleNoResult();
            else
                await processTrxFile(trxFilePath);
        }

        public Task StoreFailedExecution(string errorMessage)
            => writeLine($"{evaluationTask.StudentId};{DateTime.Now};futasi hiba: {errorMessage}");

        private async Task processTrxFile(string trxFilePath)
        {
            var trxResult = await TrxReader.Read(trxFilePath);
            await writeResult(trxResult.Total, trxResult.Passed);
        }

        private Task handleNoResult()
            => writeResult(0, 0);

        private async Task writeResult(int allTestsExecuted, int testsPassed)
        {
            if (!System.IO.File.Exists(options.OutputFile))
                await writeCsvFileHeader();
            await writeLine(formatResult(allTestsExecuted, testsPassed));
        }

        private string formatResult(int allTestsExecuted, int testsPassed)
            => $"{evaluationTask.StudentId};{DateTime.Now};{allTestsExecuted};{testsPassed}";

        private Task writeCsvFileHeader()
            => writeLine("HallgatoAzonosito;FuttatasDatum;OsszesTeszt;SikeresTeszt");

        private Task writeLine(string line)
            => System.IO.File.AppendAllTextAsync(options.OutputFile, line.Replace(Environment.NewLine, " ") + Environment.NewLine, System.Text.Encoding.UTF8);

        public void Dispose() { }
    }
}
