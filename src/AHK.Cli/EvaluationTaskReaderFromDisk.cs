using System;
using System.Collections.Generic;
using System.IO;
using AHK.Evaluation;
using Microsoft.Extensions.Configuration;

namespace AHK
{
    internal static class EvaluationTaskReaderFromDisk
    {
        public static (IEvaluatorInputQueue, FilesystemTaskSolutionProvider) ReadFrom(string assignmentsDir)
        {
            validateAssignmentsDir(assignmentsDir, out string configFilePath);

            var config = getConfig(configFilePath);

            var evaluationTasks = new Evaluation.InputQueues.SimpleQueue();
            var studentIdToSolutionDirMapping = new Dictionary<string, string>();
            foreach (var assignmentSolutionDir in Directory.EnumerateDirectories(assignmentsDir))
            {
                var t = createTaskFrom(config, assignmentSolutionDir);
                evaluationTasks.Enqueue(t);
                studentIdToSolutionDirMapping[t.StudentId] = assignmentSolutionDir;
            }

            return (evaluationTasks, new FilesystemTaskSolutionProvider(studentIdToSolutionDirMapping));
        }

        private static EvaluationTask createTaskFrom(EvaluationConfig config, string solutionDirectoryPath)
        {
            var studentId = Path.GetFileName(solutionDirectoryPath);
            return new EvaluationTask(studentId, config);
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
