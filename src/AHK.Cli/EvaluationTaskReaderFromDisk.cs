using System;
using System.Collections.Generic;
using System.IO;
using AHK.Evaluation;
using Microsoft.Extensions.Configuration;

namespace AHK
{
    internal static class EvaluationTaskReaderFromDisk
    {
        public static IEnumerable<EvaluationTask> ReadFrom(string assignmentsDir)
        {
            validateAssignmentsDir(assignmentsDir, out string configFilePath);

            var config = getConfig(configFilePath);

            foreach (var assignmentSolutionDir in Directory.EnumerateDirectories(assignmentsDir))
                yield return createTaskFrom(config, assignmentSolutionDir);
        }

        private static EvaluationTask createTaskFrom(EvaluationConfig config, string solutionDirectoryPath)
        {
            // TO-DO
            var studentId = Path.GetDirectoryName(solutionDirectoryPath);
            return new EvaluationTask(studentId, solutionDirectoryPath, config);
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
            if(possibleConfigFiles.Length == 0)
                throw new Exception($"Missing a configration json file in '{dir}'");
            if (possibleConfigFiles.Length > 1)
                throw new Exception($"Found more than one configration json file in '{dir}'");

            configFilePath = possibleConfigFiles[0];

            if (Directory.GetDirectories(dir).Length == 0)
                throw new Exception($"Missing assignment solution directories in '{dir}'");
        }
    }
}
