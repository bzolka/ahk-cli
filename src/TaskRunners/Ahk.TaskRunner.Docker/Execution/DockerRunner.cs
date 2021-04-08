using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Ahk.TaskRunner
{
    public abstract class DockerRunner : ITaskRunner
    {
        protected readonly DockerRunnerTask task;
        protected readonly ILogger logger;
        protected readonly ITempPathProvider tempPathProvider;

        protected DockerRunner(DockerRunnerTask task, ILogger logger, ITempPathProvider? tempPathProvider = null)
        {
            this.task = task ?? throw new ArgumentNullException(nameof(task));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.tempPathProvider = tempPathProvider ?? DefaultTempPathProvider.Instance;
        }

        public static DockerRunner Create(DockerRunnerTask task, ILogger logger, ITempPathProvider? tempPathProvider = null)
        {
            if (task.ServiceContainer is null)
                return new DockerRunnerSingleContainer(task, logger, tempPathProvider);
            else
                return new DockerRunnerMultiContainer(task, logger, tempPathProvider);
        }

        public async Task<RunnerResult> Run()
        {
            using var docker = DockerConnectionHelper.GetConnectionConfiguration().CreateClient();

            logger.LogTrace($"Created Docker runner for source {task.SubmissionSource} student {task.StudentId}");
            try
            {
                using var timeout = new CancellationTokenSource(task.EvaluationTimeout);

                using var tempDirectoryForSolutionCopy = tempPathProvider.GetTempDirectory();
                var solutionFolderToMount = await extractSolutionGetEffectiveDirectory(task.SubmissionDirectoryInMachine, tempDirectoryForSolutionCopy.Path);

                var containerConsoleLog = await runCore(docker, timeout.Token, solutionFolderToMount);

                logger.LogInformation("Docker runner finished");
                return RunnerResult.Success(containerConsoleLog);
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("Docker runner timeout");
                return RunnerResult.Timeout(string.Empty);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Docker runner failed");
                return RunnerResult.Failed(string.Empty, ex);
            }
        }
        public void Dispose() { }

        protected abstract Task<string> runCore(Docker.DotNet.DockerClient docker, CancellationToken timeout, string solutionFolderToMount);

        protected void configureMount(Docker.DotNet.Models.CreateContainerParameters createContainerParams, string solutionFolderToMount)
        {
            createContainerParams.HostConfig.Mounts.Add(new Docker.DotNet.Models.Mount()
            {
                Type = "bind",
                Source = solutionFolderToMount,
                Target = task.SubmissionDirectoryInContainer,
                ReadOnly = false
            });

            if (task.HasArtifactDirectory)
            {
                System.IO.Directory.CreateDirectory(task.ArtifactPathInMachine); // must exist for the mount
                createContainerParams.HostConfig.Mounts.Add(new Docker.DotNet.Models.Mount()
                {
                    Type = "bind",
                    Source = task.ArtifactPathInMachine,
                    Target = task.ArtifactPathInContainer,
                    ReadOnly = false
                });
            }
        }

        private static async Task<string> extractSolutionGetEffectiveDirectory(string sourceDirectoryOrFile, string targetDirectory)
        {
            System.IO.Directory.CreateDirectory(targetDirectory);

            if (System.IO.Directory.Exists(sourceDirectoryOrFile))
            {
                await DirectoryHelper.DirectoryCopy(sourceDirectoryOrFile, targetDirectory, true);
                return targetDirectory;
            }
            else if (System.IO.File.Exists(sourceDirectoryOrFile))
            {
                return await ZipHelper.ExtractAndGetContentsDir(sourceDirectoryOrFile, targetDirectory);
            }
            else
                throw new System.IO.FileNotFoundException("Cannot find solution to evaluate", sourceDirectoryOrFile);
        }
    }
}
