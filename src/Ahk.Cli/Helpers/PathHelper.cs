using System;
using System.IO;
using System.Linq;

namespace Ahk
{
    internal static class PathHelper
    {
        public static bool IsPathValid(string path)
            => !string.IsNullOrEmpty(path) && !string.IsNullOrWhiteSpace(path) && !Path.GetInvalidPathChars().Any(c => path.Contains(c));

        public static string ResolveToFullPath(string pathFromUser) => ResolveToFullPath(pathFromUser, DateTime.Now);

        public static string ResolveToFullPath(string pathFromUser, DateTime currentTime) =>
            Path.GetFullPath(pathFromUser
                               .Replace("{date}", currentTime.ToPathCompatibleString())
                               .Replace("{datum}", currentTime.ToPathCompatibleString()));
    }
}
