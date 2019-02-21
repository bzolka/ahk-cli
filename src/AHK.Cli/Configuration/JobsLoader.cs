using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AHK.Configuration;
using AHK.Execution;
using AHK.Execution.Evaluation;
using Microsoft.Extensions.Logging;

namespace AHK
{
    internal static class JobsLoader
    {
        public static RunConfig Load(string assignmentsDirPathFromUser, string configFilePathFromUser, string resultsDirPathFromUser, ILogger logger)
        {
            (var assignmentsDir, var resultsDir, var defaultRunConfigFile) = InputPathsResolver.ResolveEffectivePaths(assignmentsDirPathFromUser, resultsDirPathFromUser, configFilePathFromUser);

            if (Directory.GetDirectories(assignmentsDir).Length + Directory.GetFiles(assignmentsDir, "*.zip").Length == 0)
                throw new Exception($"Megoldasok konyvtaraban '{assignmentsDir}' nincsenek alkonyvtarak vagy zipek a hallgatoi megoldasokkal");

            AHKJobConfig defaultRunConfig = null;
            if (!string.IsNullOrEmpty(defaultRunConfigFile))
                defaultRunConfig = RunConfigReader.GetAndValidateConfig(defaultRunConfigFile);

            var evaluationTasks = new List<ExecutionTask>();
            foreach (var assignmentSolution in enumeratePossibleAssignmentSolutions(assignmentsDir))
            {
                var t = createTaskFrom(assignmentSolution, resultsDir, defaultRunConfig, logger);
                if (t == null)
                    logger.LogWarning("Megoldas konyvar hibas: {Dir}", assignmentSolution);
                else
                    evaluationTasks.Add(t);
            }

            var resultsXlsxFileName = Path.Combine(resultsDir, "eredmenyek.xlsx");
            return new RunConfig(evaluationTasks, resultsXlsxFileName);
        }

        private static IEnumerable<string> enumeratePossibleAssignmentSolutions(string assignmentsDir) =>
            Directory.EnumerateDirectories(assignmentsDir).Union(Directory.EnumerateFiles(assignmentsDir, "*.zip"));

        private static ExecutionTask createTaskFrom(string path, string resultsDir, AHKJobConfig defaultConfig, ILogger logger)
        {
            var studentId = StudentIdParser.GetStudentIdFor(path);
            var effectiveConfig = RunConfigReader.Get(path) ?? defaultConfig;

            if (effectiveConfig == null)
            {
                logger.LogWarning("Nincs megadva futtato konfiguracio a kovetkezo mappahoz: {Dir}", path);
                return null;
            }

            resultsDir = Path.Combine(resultsDir, studentId);
            return new ExecutionTask(studentId, path, resultsDir,
                                     effectiveConfig.Docker.ImageName, effectiveConfig.Docker.SolutionInContainer, effectiveConfig.Docker.ResultInContainer, effectiveConfig.Docker.EvaluationTimeout, effectiveConfig.Docker.ContainerParams,
                                     createEvaluationTask(effectiveConfig));
        }

        private static EvaluationTask createEvaluationTask(AHKJobConfig effectiveConfig)
        {
            if (effectiveConfig.Trx != null)
                return new TrxEvaluationTask(effectiveConfig.Trx.TrxFileName);
            else if (effectiveConfig.ConsoleMessageGrader != null)
                return new ConsoleMessagesEvaluationTask(effectiveConfig.ConsoleMessageGrader.ValidationCode);
            else
                return null;
        }
    }
}
