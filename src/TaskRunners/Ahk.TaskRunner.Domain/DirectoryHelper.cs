using System.IO;
using System.Threading.Tasks;

namespace Ahk.TaskRunner
{
    public static class DirectoryHelper
    {
        public static async Task DirectoryCopy(string sourceDirectory, string destinationDirectory, bool recursive)
        {
            if (!Directory.Exists(sourceDirectory))
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirectory);

            if (!Directory.Exists(destinationDirectory))
                Directory.CreateDirectory(destinationDirectory);

            foreach (var file in Directory.GetFiles(sourceDirectory))
            {
                using (var sourceStream = File.Open(file, FileMode.Open))
                using (var destinationStream = File.Create(Path.Combine(destinationDirectory, Path.GetFileName(file))))
                    await sourceStream.CopyToAsync(destinationStream);
            }

            if (recursive)
            {
                foreach (var subdir in Directory.GetDirectories(sourceDirectory))
                {
                    // do not need the .git directory, that is not part of the final solution
                    if (Path.GetFileName(subdir) == ".git")
                        continue;

                    await DirectoryCopy(subdir, Path.Combine(destinationDirectory, Path.GetFileName(subdir)), recursive);
                }
            }
        }
    }
}
