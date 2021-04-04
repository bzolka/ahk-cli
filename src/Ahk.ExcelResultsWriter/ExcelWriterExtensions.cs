using NPOI.SS.UserModel;

namespace Ahk.ExcelResultsWriter
{
    internal static class ExcelWriterExtensions
    {
        public static void SetCellValueWithSanitize(this ICell cell, string content) => cell.SetCellValue(content.SanitizeStringForOutput());

        public static string SanitizeStringForOutput(this string s)
        {
            // A System.Web.HttpUtility.HtmlEncode a magyar ekezetes karakterek tobbseget is kodolja, ezert nem hasznaljuk

            if (s == null)
                return string.Empty;
            else
            {
                // Az AUT portal a   <abc    >abc     <abc>    jellegu tartalmakat nem fogadja el, csak ezt csereljuk
                return s.Replace('<', '[').Replace('>', ']');
            }
        }
    }
}
