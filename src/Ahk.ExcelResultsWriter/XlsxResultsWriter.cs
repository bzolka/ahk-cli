using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ahk.Grader;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Ahk.ExcelResultsWriter
{
    public class XlsxResultsWriter : IDisposable
    {
        private readonly string resultsXlsxFileName;
        private readonly XSSFWorkbook wb;
        private readonly ISheet workSheet;
        private readonly IRow headerRow;

        private readonly ICellStyle cellStyleComment;
        private readonly ICellStyle cellStyleFailure;
        private readonly ICellStyle cellStyleInconclusive;

        private int nextRowIndex = 1;
        private readonly Dictionary<string, int> exerciseToColumnIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        public XlsxResultsWriter(string resultsXlsxFileName)
        {
            this.resultsXlsxFileName = resultsXlsxFileName;
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
                sb.Append($"[{tr.TestName}] {tr.ResultPoints}");
                if (!string.IsNullOrEmpty(tr.Description))
                    sb.Append(" / " + tr.Description);
                sb.AppendLine();
            }

            return sb.ToString().TrimEnd();
        }

        private void saveFile()
        {
            using (var fs = File.Create(resultsXlsxFileName))
                wb.Write(fs);
        }

        public void Dispose() { }
    }
}
