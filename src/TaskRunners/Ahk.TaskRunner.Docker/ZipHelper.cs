using System.Linq;
using System.Threading.Tasks;

namespace Ahk.TaskRunner
{
    internal class ZipHelper
    {
        public static async Task<string> ExtractAndGetContentsDir(string zipFilePath, string targetDirectory)
        {
            await Task.Run(() => System.IO.Compression.ZipFile.ExtractToDirectory(zipFilePath, targetDirectory, true));

            // if the zip root contains no files but a single directory, use that directory as the evaluated content
            if (!System.IO.Directory.EnumerateFiles(targetDirectory, "*", System.IO.SearchOption.TopDirectoryOnly).Any()
                && System.IO.Directory.GetDirectories(targetDirectory, "*", System.IO.SearchOption.TopDirectoryOnly).Length == 1)
            {
                return System.IO.Directory.GetDirectories(targetDirectory).First();
            }

            return targetDirectory;
        }
    }
}