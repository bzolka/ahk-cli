using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ahk.Configuration
{
    internal static class StudentIdParser
    {
        public const int SuspiciousNeptunTxtFileSizeThreshold = 100;

        public static string GetStudentNeptunFor(string path)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            string neptunValue = null;
            if (Directory.Exists(path))
            {
                var neptunKodTxtFile = Directory.EnumerateFiles(path, "*", SearchOption.TopDirectoryOnly)
                                                        .FirstOrDefault(f => Path.GetFileName(f).Equals("neptun.txt", StringComparison.OrdinalIgnoreCase));
                if (neptunKodTxtFile != null)
                {
                    if (new FileInfo(neptunKodTxtFile).Length > SuspiciousNeptunTxtFileSizeThreshold) // safety check to make sure the file is not maliciously large to cause out-of-memory error
                        throw new Exception($"Gyanus neptunkod.txt fajl: {neptunKodTxtFile}");

                    neptunValue = getNeptunFromTextFileContent(File.ReadAllText(neptunKodTxtFile));
                }
            }
            else if (File.Exists(path) && Path.GetExtension(path).Equals(".zip", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    using (var zipFile = System.IO.Compression.ZipFile.OpenRead(path))
                    {
                        var neptunKodZipEntry = zipFile.Entries.FirstOrDefault(f => Path.GetFileName(f.Name).Equals("neptun.txt", StringComparison.OrdinalIgnoreCase));
                        if (neptunKodZipEntry != null)
                        {
                            if (neptunKodZipEntry.Length > SuspiciousNeptunTxtFileSizeThreshold) // safety check to make sure the file is not maliciously large to cause out-of-memory error
                                throw new Exception($"Gyanus neptunkod.txt fajl: {path}/{neptunKodZipEntry.Name}");

                            using (var zipEntryReader = new StreamReader(neptunKodZipEntry.Open()))
                                neptunValue = getNeptunFromTextFileContent(zipEntryReader.ReadToEnd());
                        }
                    }
                }
                catch { } // invalid zip, fallback to default
            }

            // fallback is the directory/file name
            if (string.IsNullOrEmpty(neptunValue))
                return Path.GetFileNameWithoutExtension(path);
            else
                return neptunValue;
        }

        private static readonly Regex studentNeptunCodeRegex = new Regex("[^a-zA-Z0-9]");

        private static string getNeptunFromTextFileContent(string textContent)
        {
            var neptun = textContent.Replace("\r", "").Replace("\n", "").Trim();
            return studentNeptunCodeRegex.Replace(neptun, "");
        }
    }
}
