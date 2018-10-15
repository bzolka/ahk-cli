using System;
using System.IO;
using AHK.Evaluation;
using Microsoft.Extensions.Configuration;

namespace AHK
{
    internal static class EvaluationTaskReaderFromDisk
    {
        public static IEvaluatorInputQueue ReadFrom(string assignmentsDir, string resultsDir)
        {
            validateAssignmentsDir(assignmentsDir, out string configFilePath);
            resultsDir = getResultsDir(resultsDir);

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

        private static void validateAssignmentsDir(string dir, out string configFilePath)
        {
            if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
                throw new Exception($"Assignments directory '{dir}' missing.");

            var possibleConfigFiles = Directory.GetFiles(dir, "*.json", SearchOption.TopDirectoryOnly);
            if (possibleConfigFiles.Length == 0)
                throw new Exception($"Missing a configration json file in '{dir}'");
            if (possibleConfigFiles.Length > 1)
                throw new Exception($"Found more than one configration json file in '{dir}'");

            configFilePath = possibleConfigFiles[0];

            if (Directory.GetDirectories(dir).Length == 0)
                throw new Exception($"Missing assignment solution directories in '{dir}'");
        }

        private static string getResultsDir(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new Exception($"Results directory not specified.");

            path = path
                .Replace("{date}", DateTime.Now.ToPathCompatibleString())
                .Replace("{datum}", DateTime.Now.ToPathCompatibleString());

            var originalPath = path;
            int counter = 0;
            while (Directory.Exists(path))
                path = originalPath + "_" + (++counter);

            return path;
        }
    }
}
