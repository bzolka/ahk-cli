using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AHK.Grader;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace AHK.ExcelResultsWriter
{
    public class XlsxResultsWriter : IDisposable
    {
        private readonly XlsxResultsWriterConfig config;

        private readonly XSSFWorkbook wb;
        private readonly ISheet workSheet;
        private readonly IRow headerRow;

        private readonly ICellStyle cellStyleComment;
        private readonly ICellStyle cellStyleFailure;
        private readonly ICellStyle cellStyleInconclusive;

        private int nextRowIndex = 1;
        private readonly Dictionary<string, int> exerciseToColumnIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        public XlsxResultsWriter(XlsxResultsWriterConfig config)
        {
            this.config = config;

            this.wb = new XSSFWorkbook();
            this.workSheet = wb.CreateSheet();

            this.cellStyleComment = wb.CreateCellStyle();
            this.cellStyleComment.WrapText = true;

            this.cellStyleFailure = wb.CreateCellStyle();
            this.cellStyleFailure.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightOrange.Index;
            this.cellStyleFailure.FillPattern = FillPattern.SolidForeground;
            this.cellStyleFailure.WrapText = true;

            this.cellStyleInconclusive = wb.CreateCellStyle();
            this.cellStyleInconclusive.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightYellow.Index;
            this.cellStyleInconclusive.FillPattern = FillPattern.SolidForeground;
            this.cellStyleInconclusive.WrapText = true;

            this.headerRow = workSheet.CreateRow(0);
            headerRow.CreateCell(0, CellType.String).SetCellValueWithSanitize("Név");
            headerRow.CreateCell(1, CellType.String).SetCellValueWithSanitize("Neptun kód");
        }

        public void Write(string studentName, string studentId, GraderResult graderResult)
        {
            updateHeaderRowWithExercises(graderResult.Exercises);

            var row = workSheet.CreateRow(nextRowIndex++);

            row.CreateCell(0, CellType.String).SetCellValueWithSanitize(studentName);
            row.CreateCell(1, CellType.String).SetCellValueWithSanitize(studentId.ToUpper());

            foreach (var exercise in graderResult.Exercises)
            {
                var cellIndex = exerciseToColumnIndex[exercise.ExerciseName];

                var cell1 = row.CreateCell(cellIndex, CellType.String);
                cell1.SetCellValueWithSanitize(formatCommentForCell(exercise.TestsResults));

                var cell2 = row.CreateCell(cellIndex + 1, CellType.String);
                cell2.SetCellValue(exercise.SumResultPoints);

                if (exercise.GradingOutcome == GradingOutcomes.FailedToGrade)
                    cell1.CellStyle = cell2.CellStyle = cellStyleFailure;
                else if (exercise.GradingOutcome == GradingOutcomes.Inconclusive)
                    cell1.CellStyle = cell2.CellStyle = cellStyleInconclusive;
                else
                    cell1.CellStyle = cellStyleComment;
            }

            saveFile();
        }

        private void updateHeaderRowWithExercises(IReadOnlyList<ExerciseResult> exercises)
        {
            foreach (var exercise in exercises)
            {
                if (!exerciseToColumnIndex.TryGetValue(exercise.ExerciseName, out var index))
                {
                    index = 2 + (exerciseToColumnIndex.Count * 2);
                    exerciseToColumnIndex[exercise.ExerciseName] = index;

                    var columnPrefix = string.Empty;
                    if (exercise.ExerciseName != ExerciseResult.DefaultExerciseName)
                        columnPrefix = $"({exercise.ExerciseName}) ";

                    headerRow.CreateCell(index, CellType.String).SetCellValueWithSanitize(columnPrefix + "Megjegyzés a hallgatónak");
                    headerRow.CreateCell(index + 1, CellType.String).SetCellValueWithSanitize(columnPrefix + "Eredmény");

                    workSheet.SetColumnWidth(index, 800 * 20);
                }
            }
        }

        private string formatCommentForCell(IReadOnlyList<TestResult> testResults)
        {
            if (testResults == null || testResults.Count == 0)
                return string.Empty;

            var sb = new StringBuilder();
            foreach (var tr in testResults)
            {

                // Semantics of xlsxResultsWriter_Comment_SkipSuccessfullTests:
                // We can't check if test is truly successful here. It's because console parser (but only this) supports points
                // larger than 1, and we don't know the max point.
                // So, instead of checking if test is successful, filter out irrelevant test, and have info in the comment only if:
                // - grading is unsuccessful or grading is inclonclusive or have something in the Description.
                // Also, if grading is unsuccessful or grading is inclonclusive, then add some info about this in the resulting text.

                bool isTestNameWritten = false;

                if (!config.Comment_MinimalOutput)
                    AppendText_PrependTestNameIfNotYetWritten($" {tr.ResultPoints}");

                if (config.Comment_MinimalOutput)
                {
                    if (tr.GradingOutcome == GradingOutcomes.FailedToGrade)
                        AppendText_PrependTestNameIfNotYetWritten(" / Failed to grade.");
                    else if (tr.GradingOutcome == GradingOutcomes.Inconclusive)
                        AppendText_PrependTestNameIfNotYetWritten(" / Test is inclonclusive.");
                }

                if (!string.IsNullOrEmpty(tr.Description))
                    AppendText_PrependTestNameIfNotYetWritten(" / " + tr.Description);

                // If Comment_MinimalOutput is true, we could add an extra line here for better readability.
                // I used to have that for Sznikák HF1 and HF2. Maybe I'll re-add it later.

                sb.AppendLine();

                // Probably this is not very performant, a closure is create in each iteration ...
                // Alternatively we could build another string here without the test name, and when we are though everything could check
                // if we have anything to write out, and if yes, write out the test name and then this other string.
                void AppendText_PrependTestNameIfNotYetWritten(string text)
                {
                    if (!isTestNameWritten)
                    {
                        sb.Append($"[{tr.TestName}]");
                        isTestNameWritten = true;
                    }
                    sb.Append(text);
                }
            }

            return sb.ToString().TrimEnd();
        }



        private void saveFile()
        {
            using (var fs = File.Create(config.ResultsXlsxFileName))
                wb.Write(fs);
        }

        public void Dispose() { }
    }
}
