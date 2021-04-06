using System;
using System.IO;
using System.Linq;

namespace Ahk
{
    internal static class StudentIdParser
    {
        public const int SuspiciousNeptunTxtFileSizeThreshold = 100;

        public static string GetStudentId(string submissionPath, string studentIdfileName)
        {
            if (string.IsNullOrEmpty(submissionPath) || string.IsNullOrWhiteSpace(submissionPath))
                throw new ArgumentNullException(nameof(submissionPath));

            string? studentIdValue = null;
            if (Directory.Exists(submissionPath))
            {
                var textFile = Directory.EnumerateFiles(submissionPath, "*", SearchOption.TopDirectoryOnly)
                                        .FirstOrDefault(f => Path.GetFileName(f).Equals(studentIdfileName, StringComparison.OrdinalIgnoreCase));
                if (textFile != null)
                {
                    if (new FileInfo(textFile).Length > SuspiciousNeptunTxtFileSizeThreshold) // safety check to make sure the file is not maliciously large to cause out-of-memory error
                        throw new Exception($"Suspicious {studentIdfileName} file: {textFile}");

                    studentIdValue = getStudentIdFromTextFileContent(File.ReadAllText(textFile));
                }
            }
            else if (File.Exists(submissionPath) && Path.GetExtension(submissionPath).Equals(".zip", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    using (var zipFile = System.IO.Compression.ZipFile.OpenRead(submissionPath))
                    {
                        var zipEntry = zipFile.Entries.FirstOrDefault(f => Path.GetFileName(f.Name).Equals(studentIdfileName, StringComparison.OrdinalIgnoreCase));
                        if (zipEntry != null)
                        {
                            if (zipEntry.Length > SuspiciousNeptunTxtFileSizeThreshold) // safety check to make sure the file is not maliciously large to cause out-of-memory error
                                throw new Exception($"Suspicious {studentIdfileName} file: {submissionPath}/{zipEntry.Name}");

                            using (var zipEntryReader = new StreamReader(zipEntry.Open()))
                                studentIdValue = getStudentIdFromTextFileContent(zipEntryReader.ReadToEnd());
                        }
                    }
                }
                catch { } // invalid zip, fallback to default
            }

            // fallback is the directory/file name
            if (string.IsNullOrEmpty(studentIdValue))
                return Path.GetFileNameWithoutExtension(submissionPath);
            else
                return studentIdValue;
        }

        private static string getStudentIdFromTextFileContent(string textContent)
            => textContent.Replace("\r", "").Replace("\n", "").Trim();
    }
}
