using System.IO;
using System.Linq;

namespace AHK.Evaluation
{
    internal static class PathHelper
    {
        public static bool IsPathValid(string path)
            => !string.IsNullOrEmpty(path) && !string.IsNullOrWhiteSpace(path) && !Path.GetInvalidPathChars().Any(c => path.Contains(c));
    }
}
