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
            (var assignmentsDir, var resultsDir, var jobConfigFile) = InputPathsResolver.ResolveEffectivePaths(assignmentsDirPathFromUser, resultsDirPathFromUser, configFilePathFromUser);

            if (Directory.GetDirectories(assignmentsDir).Length + Directory.GetFiles(assignmentsDir, "*.zip").Length == 0)
                throw new Exception($"Megoldasok konyvtaraban '{assignmentsDir}' nincsenek alkonyvtarak vagy zipek a hallgatoi megoldasokkal");

            var jobConfig = RunConfigReader.GetAndValidateConfig(jobConfigFile);

            var evaluationTasks = new List<ExecutionTask>();
            foreach (var assignmentSolution in enumeratePossibleAssignmentSolutions(assignmentsDir))
            {
                var t = createTaskFrom(assignmentSolution, resultsDir, jobConfig, logger);
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

        private static ExecutionTask createTaskFrom(string path, string resultsDir, AHKJobConfig jobConfig, ILogger logger)
        {
            // there is no name available, use the directory name for now
            var studentName = Path.GetFileNameWithoutExtension(path);
            var studentNeptun = StudentIdParser.GetStudentNeptunFor(path);

            if (jobConfig == null)
            {
                logger.LogWarning("Nincs megadva futtato konfiguracio a kovetkezo mappahoz: {Dir}", path);
                return null;
            }

            resultsDir = Path.Combine(resultsDir, studentNeptun);
            return new ExecutionTask(studentName, studentNeptun, path, resultsDir,
                                     jobConfig.Docker.ImageName, jobConfig.Docker.SolutionInContainer, jobConfig.Docker.ResultInContainer, jobConfig.Docker.EvaluationTimeout, jobConfig.Docker.ContainerParams,
                                     createEvaluationTask(jobConfig));
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
