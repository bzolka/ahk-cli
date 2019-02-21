using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Microsoft.Extensions.Logging;

namespace AHK.TaskRunner
{
    public class DockerRunner : ITaskRunner
    {
        private readonly DockerRunnerTask task;
        private readonly ILogger logger;
        private readonly ITempPathProvider tempPathProvider;
        private readonly DockerClient docker;

        public DockerRunner(DockerRunnerTask task, ILogger logger, ITempPathProvider tempPathProvider = null)
        {
            this.task = task ?? throw new ArgumentNullException(nameof(task));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.tempPathProvider = tempPathProvider ?? DefaultTempPathProvider.Instance;
            this.docker = DockerConnectionHelper.GetConnectionConfiguration().CreateClient();

            logger.LogTrace("Created Docker runner for {TaskId}", task.TaskId);
        }

        public async Task<RunnerResult> Run()
        {
            try
            {
                await ensureImageExists();
                using (var timeout = new CancellationTokenSource(task.EvaluationTimeout))
                using (var tempDirectoryForSolutionCopy = tempPathProvider.GetTempDirectory())
                {
                    var containerId = await createContainer(timeout.Token, tempDirectoryForSolutionCopy.Path);
                    string containerConsoleLog = string.Empty;
                    try
                    {
                        containerConsoleLog = await runContainerWaitForExit(containerId, timeout.Token);
                    }
                    finally
                    {
                        await cleanupContainer(containerId);
                    }

                    logger.LogInformation("Docker runner finished");
                    return RunnerResult.Success(containerConsoleLog);
                }
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

        private async Task cleanupContainer(string containerId)
        {
            try
            {
                await docker.Containers.RemoveContainerAsync(containerId, new Docker.DotNet.Models.ContainerRemoveParameters() { Force = true, RemoveVolumes = true });
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Cleanup of container {ContainerId} failed", containerId);
            }
        }

        private async Task<string> runContainerWaitForExit(string containerId, CancellationToken cancellationToken)
        {
            if (!await docker.Containers.StartContainerAsync(containerId, new Docker.DotNet.Models.ContainerStartParameters(), cancellationToken))
                throw new Exception("Failed to start container");

            logger.LogTrace("Container running");

            var attachResult = await docker.Containers.AttachContainerAsync(containerId, false, new Docker.DotNet.Models.ContainerAttachParameters() { Stderr = true, Stdout = true, Stream = true }, cancellationToken);
            var readContainerLogsTask = attachResult.ReadOutputToEndAsync(cancellationToken);

            await docker.Containers.WaitContainerAsync(containerId, cancellationToken);

            logger.LogTrace("Container stopped");

            var (logStdout, logStderr) = await readContainerLogsTask;

            return logStdout + Environment.NewLine + logStderr;
        }

        private async Task<string> createContainer(CancellationToken cancellationToken, string tempPathForSolutionDirCopy)
        {
            try
            {
                var createContainerParams = new Docker.DotNet.Models.CreateContainerParameters() {
                    Image = task.ImageName,
                    Labels = getContainerLabels(),
                    HostConfig = new Docker.DotNet.Models.HostConfig() {
                        Mounts = new List<Docker.DotNet.Models.Mount>() { }
                    }
                };

                if (task.ContainerParams != null && task.ContainerParams.Any())
                    setImageParameters(createContainerParams, task.ContainerParams);

                System.IO.Directory.CreateDirectory(tempPathForSolutionDirCopy);
                var effectiveSolutionFolderToMount = await extractSolutionGetEffectiveDirectory(task.SolutionDirectoryInMachine, tempPathForSolutionDirCopy);

                createContainerParams.HostConfig.Mounts.Add(new Docker.DotNet.Models.Mount() {
                    Type = "bind",
                    Source = effectiveSolutionFolderToMount,
                    Target = task.SolutionDirectoryInContainer,
                    ReadOnly = false
                });

                if (task.ShouldFetchResult)
                {
                    System.IO.Directory.CreateDirectory(task.ResultPathInMachine); // must exist for the mount
                    createContainerParams.HostConfig.Mounts.Add(new Docker.DotNet.Models.Mount() {
                        Type = "bind",
                        Source = task.ResultPathInMachine,
                        Target = task.ResultPathInContainer,
                        ReadOnly = false
                    });
                }

                var createContainerResponse = await docker.Containers.CreateContainerAsync(createContainerParams, cancellationToken);

                if (string.IsNullOrEmpty(createContainerResponse.ID))
                    throw new Exception("Container create failied with unknown error");

                logger.LogTrace("Container created with ID {ContainerId}", createContainerResponse.ID);

                return createContainerResponse.ID;
            }
            catch (DockerImageNotFoundException)
            {
                throw new Exception($"Image {task.ImageName ?? "N/A"} not found.");
            }
            catch (DockerApiException ex)
            {
                throw new Exception($"Container create failed with error: {ex.Message}", ex);
            }
        }

        private async Task<string> extractSolutionGetEffectiveDirectory(string sourceDirectoryOrFile, string targetDirectory)
        {
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

        private void setImageParameters(Docker.DotNet.Models.CreateContainerParameters createContainerParams, IReadOnlyDictionary<string, string> containerParams)
        {
            foreach (var param in containerParams)
            {
                try
                {
                    if (trySetDynamicPropertyOn(createContainerParams, param.Key, param.Value))
                        continue;

                    if (trySetDynamicPropertyOn(createContainerParams.HostConfig, param.Key, param.Value))
                        continue;

                    logger.LogWarning("Unknown container parameter {ParamName}", param.Key);
                }
                catch
                {
                    logger.LogWarning("Cannot set container parameter {ParamName} to value {ParamVale}", param.Key, param.Value);
                }
            }
        }

        private bool trySetDynamicPropertyOn(object objectToSetOn, string propertyName, object propertyValue)
        {
            if (objectToSetOn == null)
                return false;

            if (string.IsNullOrEmpty(propertyName))
                return false;

            if (Char.IsLower(propertyName[0]))
                propertyName = char.ToUpper(propertyName[0]).ToString() + propertyName.Substring(1);

            var ccpType = objectToSetOn.GetType();
            var prop = ccpType.GetProperty(propertyName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if (prop != null)
            {
                var valueConverted = Convert.ChangeType(propertyValue, prop.PropertyType);
                prop.SetValue(objectToSetOn, valueConverted);
                return true;
            }
            else
            {
                return false;
            }
        }

        private IDictionary<string, string> getContainerLabels()
            => new Dictionary<string, string>()
            {
                { DockerCleanup.ContainerLabel, "" },
                { "AHK_SolutionDir", task.SolutionDirectoryInMachine }
            };

        private async Task ensureImageExists()
        {
            try
            {
                var findImageResult = await docker.Images.ListImagesAsync(new Docker.DotNet.Models.ImagesListParameters() {
                    MatchName = task.ImageName
                });

                if (findImageResult.Any())
                {
                    logger.LogTrace("Found image {ImageName} with Docker ID {ImageId}", task.ImageName, findImageResult.First().ID);
                    return;
                }

                logger.LogTrace("Pulling image {ImageName}", task.ImageName);
                await docker.Images.CreateImageAsync(new Docker.DotNet.Models.ImagesCreateParameters() {
                    FromImage = task.ImageName
                },
                    null,
                    new Progress<Docker.DotNet.Models.JSONMessage>());
                logger.LogTrace("Pulling image {ImageName} completed", task.ImageName);
            }
            catch (Exception ex)
            {
                throw new Exception("Pulling image failed", ex);
            }
        }

        public void Dispose() => docker?.Dispose();
    }
}
