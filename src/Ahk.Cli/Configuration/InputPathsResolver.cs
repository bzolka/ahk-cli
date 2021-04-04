using System;
using System.IO;

namespace Ahk.Configuration
{
    internal static class InputPathsResolver
    {
        public static (string assignmentsDir, string resultsDir, string defaultRunConfigFile) ResolveEffectivePaths(string assignmentsDirPathFromUser, string resultsDirPathFromUser, string configFilePathFromUser)
        {
            var assignmentsDir = getAssignmentsDir(assignmentsDirPathFromUser);
            var resultsDir = getResultsDir(assignmentsDir, resultsDirPathFromUser);
            var jobConfigFile = getConfigFilePath(configFilePathFromUser);

            return (assignmentsDir, resultsDir, jobConfigFile);
        }

        private static string getAssignmentsDir(string dirFromUser)
        {
            if (string.IsNullOrEmpty(dirFromUser))
            {
                return Environment.CurrentDirectory;
            }
            else
            {
                var absolutePath = Path.GetFullPath(dirFromUser);

                if (!Directory.Exists(absolutePath))
                    throw new Exception($"Megoldasok konyvtara '{absolutePath}' nem letezik");

                return absolutePath;
            }
        }

        private static string getResultsDir(string assignmentsDirectory, string resultsDirectory)
        {
            if (string.IsNullOrEmpty(resultsDirectory))
            {
                var assignmentDirName = string.IsNullOrEmpty(assignmentsDirectory) ? string.Empty : Path.GetFileName(assignmentsDirectory);
                return Path.Combine(Environment.CurrentDirectory, "_" + assignmentDirName + $"_eredmenyek-{DateTime.Now.ToPathCompatibleString()}");
            }
            else
            {
                if (!PathHelper.IsPathValid(resultsDirectory))
                    throw new Exception($"Eredmenyek konyvtara '{resultsDirectory}' nem ervenyes konyvtarnev");

                resultsDirectory = PathHelper.ResolveToFullPath(resultsDirectory);
                if (Directory.Exists(resultsDirectory))
                    resultsDirectory = resultsDirectory + "_" + DateTime.Now.ToPathCompatibleString();

                return resultsDirectory;
            }
        }

        private static string getConfigFilePath(string pathFromUser)
        {
            if (string.IsNullOrEmpty(pathFromUser))
                throw new Exception($"Nincs megadva a futtato konfiguracios fajl");

            if (File.Exists(pathFromUser))
            {
                return Path.GetFullPath(pathFromUser);
            }
            else
            {
                var possibleConfigFiles = Directory.GetFiles(pathFromUser, "*.json", SearchOption.TopDirectoryOnly);
                if (possibleConfigFiles.Length == 0)
                    throw new Exception($"Nem talalhato futtato konfiguracios fajl itt: '{pathFromUser}'");
                if (possibleConfigFiles.Length > 1)
                    throw new Exception($"Tobb, mint egy lehetseges json futtato konfiguracios fajl itt: '{pathFromUser}'");

                return Path.GetFullPath(possibleConfigFiles[0]);
            }
        }
    }
}
