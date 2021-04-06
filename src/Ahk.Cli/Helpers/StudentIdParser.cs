using System;
using System.IO;
using System.Linq;

namespace Ahk
{
    internal static class StudentIdParser
    {
        public const int SuspiciousNeptunTxtFileSizeThreshold = 100;
        public const string FileName = "neptun.txt";

        public static string GetStudentId(string path)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            string? studentIdValue = null;
            if (Directory.Exists(path))
            {
                var textFile = Directory.EnumerateFiles(path, "*", SearchOption.TopDirectoryOnly)
                                        .FirstOrDefault(f => Path.GetFileName(f).Equals(FileName, StringComparison.OrdinalIgnoreCase));
                if (textFile != null)
                {
                    if (new FileInfo(textFile).Length > SuspiciousNeptunTxtFileSizeThreshold) // safety check to make sure the file is not maliciously large to cause out-of-memory error
                        throw new Exception($"Suspicious {FileName} file: {textFile}");

                    studentIdValue = getStudentIdFromTextFileContent(File.ReadAllText(textFile));
                }
            }
            else if (File.Exists(path) && Path.GetExtension(path).Equals(".zip", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    using (var zipFile = System.IO.Compression.ZipFile.OpenRead(path))
                    {
                        var zipEntry = zipFile.Entries.FirstOrDefault(f => Path.GetFileName(f.Name).Equals(FileName, StringComparison.OrdinalIgnoreCase));
                        if (zipEntry != null)
                        {
                            if (zipEntry.Length > SuspiciousNeptunTxtFileSizeThreshold) // safety check to make sure the file is not maliciously large to cause out-of-memory error
                                throw new Exception($"Suspicious {FileName} file: {path}/{zipEntry.Name}");

                            using (var zipEntryReader = new StreamReader(zipEntry.Open()))
                                studentIdValue = getStudentIdFromTextFileContent(zipEntryReader.ReadToEnd());
                        }
                    }
                }
                catch { } // invalid zip, fallback to default
            }

            // fallback is the directory/file name
            if (string.IsNullOrEmpty(studentIdValue))
                return Path.GetFileNameWithoutExtension(path);
            else
                return studentIdValue;
        }

        private static string getStudentIdFromTextFileContent(string textContent)
            => textContent.Replace("\r", "").Replace("\n", "").Trim();
    }
}
