using System;
using System.Collections.Generic;
using System.IO;
using AHK.Configuration;
using AHK.Execution;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AHK
{
    internal class JobsLoader
    {
        private readonly ILogger logger;
        private readonly DateTime dateTime = DateTime.Now;

        public JobsLoader(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public RunConfig ReadFrom(string configFileFromUser, string assignmentsDirFromUser, string resultsDirFromUser)
        {
            var configFilePath = validateConfigFile(configFileFromUser);
            var assignmentsDir = validateAssignmentsDir(assignmentsDirFromUser);
            var resultsDir = validateResultsDir(resultsDirFromUser);

            var config = getAndValidateConfig(configFilePath);

            var evaluationTasks = new List<ExecutionTask>();
            foreach (var assignmentSolutionDir in Directory.EnumerateDirectories(assignmentsDir))
                evaluationTasks.Add(createTaskFrom(config, assignmentSolutionDir, resultsDir));

            return new RunConfig(evaluationTasks, config.ResultXlsxName);
        }

        private ExecutionTask createTaskFrom(AHKJobConfig config, string solutionDirectoryPath, string resultsBaseDir)
        {
            var studentId = Path.GetFileName(solutionDirectoryPath);
            var resultsDir = Path.Combine(resultsBaseDir, studentId);
            return new ExecutionTask(studentId, solutionDirectoryPath, resultsDir,
                                     config.Docker.ImageName, config.Docker.SolutionInContainer, config.Docker.ResultInContainer, config.Docker.EvaluationTimeout, config.Docker.ContainerParams,
                                     config.Trx.TrxFileName);
        }

        private AHKJobConfig getAndValidateConfig(string configFilePath)
        {
            var configurationRoot = new ConfigurationBuilder().AddJsonFile(configFilePath).Build();
            var c = configurationRoot.Get<AHKJobConfig>();
            if (!c.Validate(logger))
                throw new Exception("A futtato konfiguracios fajlban hiba van");

            c.ResultXlsxName = c.ResultXlsxName
                                    .Replace("{date}", dateTime.ToPathCompatibleString())
                                    .Replace("{datum}", dateTime.ToPathCompatibleString());

            return c;
        }

        private static string validateAssignmentsDir(string dir)
        {
            if (string.IsNullOrEmpty(dir))
                throw new Exception("Megoldasok konyvtara nincs megadva");

            var absolutePath = Path.GetFullPath(dir);

            if (!Directory.Exists(absolutePath))
                throw new Exception($"Megoldasok konyvtara '{absolutePath}' nem letezik");
            if (Directory.GetDirectories(absolutePath).Length == 0)
                throw new Exception($"Megoldasok konyvtaraban '{absolutePath}' nincsenek alkonyvtarak a hallgatoi megoldasokkal");

            return absolutePath;
        }

        private static string validateConfigFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new Exception("Futtato konfiguracios fajl nincs megadva");

            if (File.Exists(path))
            {
                return Path.GetFullPath(path);
            }
            else
            {
                var possibleConfigFiles = Directory.GetFiles(path, "*.json", SearchOption.TopDirectoryOnly);
                if (possibleConfigFiles.Length == 0)
                    throw new Exception($"Nem talalhato futtato konfiguracios fajlt itt: '{path}'");
                if (possibleConfigFiles.Length > 1)
                    throw new Exception($"Tobb, mint egy lehetseges futtato konfiguracios fajlt itt: '{path}'");

                return Path.GetFullPath(possibleConfigFiles[0]);
            }
        }

        private string validateResultsDir(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new Exception("Eredmenyek konyvtara nincs megadva");

            if (!PathHelper.IsPathValid(path))
                throw new Exception($"Eredmenyek konyvtara '{path}' nem ervenyes konyvtarnev");

            path = Path.GetFullPath(path
                .Replace("{date}", dateTime.ToPathCompatibleString())
                .Replace("{datum}", dateTime.ToPathCompatibleString()));

            var originalPath = path;
            int counter = 0;
            while (Directory.Exists(path))
                path = originalPath + "_" + (++counter);

            return path;
        }
    }
}
