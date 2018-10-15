using System;
using System.IO;
using AHK.Evaluation;
using Microsoft.Extensions.Configuration;

namespace AHK
{
    internal static class EvaluationTaskReaderFromDisk
    {
        public static IEvaluatorInputQueue ReadFrom(string configFileFromUser, string assignmentsDirFromUser, string resultsDirFromUser)
        {
            var configFilePath = validateConfigFile(configFileFromUser);
            var assignmentsDir = validateAssignmentsDir(assignmentsDirFromUser);
            var resultsDir = validateResultsDir(resultsDirFromUser);

            var config = getConfig(configFilePath);

            var evaluationTasksQueue = new Evaluation.InputQueues.SimpleQueue();
            foreach (var assignmentSolutionDir in Directory.EnumerateDirectories(assignmentsDir))
                evaluationTasksQueue.Enqueue(createTaskFrom(config, assignmentSolutionDir, resultsDir));

            return evaluationTasksQueue;
        }

        private static EvaluationTask createTaskFrom(EvaluationConfig config, string solutionDirectoryPath, string resultsBaseDir)
        {
            var studentId = Path.GetFileName(solutionDirectoryPath);
            var resultsDir = Path.Combine(resultsBaseDir, studentId);
            return new EvaluationTask(studentId, solutionDirectoryPath, resultsDir, config);
        }

        private static EvaluationConfig getConfig(string configFilePath)
        {
            var configurationRoot = new ConfigurationBuilder().AddJsonFile(configFilePath).Build();
            var configObject = new EvaluationConfig();
            configurationRoot.Bind(configObject);
            return configObject;
        }

        private static string validateAssignmentsDir(string dir)
        {
            if (string.IsNullOrEmpty(dir))
                throw new Exception("Assignment solutions directory not specified");

            var absolutePath = Path.GetFullPath(dir);

            if (!Directory.Exists(absolutePath))
                throw new Exception($"Assignments directory '{absolutePath}' missing.");
            if (Directory.GetDirectories(absolutePath).Length == 0)
                throw new Exception($"Missing assignment solution directories in '{absolutePath}'");

            return absolutePath;
        }

        private static string validateConfigFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new Exception($"Configuration file not specified");

            if (File.Exists(path))
            {
                return Path.GetFullPath(path);
            }
            else
            {
                var possibleConfigFiles = Directory.GetFiles(path, "*.json", SearchOption.TopDirectoryOnly);
                if (possibleConfigFiles.Length == 0)
                    throw new Exception($"Missing a configration json file in '{path}'");
                if (possibleConfigFiles.Length > 1)
                    throw new Exception($"Found more than one configration json file in '{path}'");

                return Path.GetFullPath(possibleConfigFiles[0]);
            }
        }

        private static string validateResultsDir(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new Exception($"Results directory not specified");

            path = Path.GetFullPath(path
                .Replace("{date}", DateTime.Now.ToPathCompatibleString())
                .Replace("{datum}", DateTime.Now.ToPathCompatibleString()));

            var originalPath = path;
            int counter = 0;
            while (Directory.Exists(path))
                path = originalPath + "_" + (++counter);

            return path;
        }
    }
}
