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

        public static string GetSubmissionsDir(string? pathFromUser)
        {
            if (string.IsNullOrEmpty(pathFromUser))
            {
                return Environment.CurrentDirectory;
            }
            else
            {
                var absolutePath = Path.GetFullPath(pathFromUser);

                if (!Directory.Exists(absolutePath))
                    throw new CliFx.Exceptions.CommandException($"Submissions directory '{absolutePath}' does not exist");

                return absolutePath;
            }
        }

        public static string GetOutputDir(string? pathFromUser)
        {
            if (string.IsNullOrEmpty(pathFromUser))
            {
                return Path.Combine(Environment.CurrentDirectory, $"_evaluation-{DateTime.Now.ToPathCompatibleString()}");
            }
            else
            {
                if (!IsPathValid(pathFromUser))
                    throw new CliFx.Exceptions.CommandException($"Output directory '{pathFromUser}' not a valid path or name");

                pathFromUser = ResolveToFullPath(pathFromUser);
                if (Directory.Exists(pathFromUser))
                    pathFromUser = pathFromUser + "_" + DateTime.Now.ToPathCompatibleString();

                return pathFromUser;
            }
        }
    }
}
