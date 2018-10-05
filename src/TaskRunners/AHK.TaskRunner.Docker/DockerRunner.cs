using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;

namespace AHK.TaskRunner
{
    public class DockerRunner : ITaskRunner
    {
        private readonly RunnerTask task;
        private readonly DockerClient docker;

        public DockerRunner(RunnerTask task)
        {
            this.task = task ?? throw new ArgumentNullException(nameof(task));
            this.docker = new DockerClientConfiguration(LocalDockerUri()).CreateClient();
        }

        public async Task<RunnerResult> Run()
        {
            try
            {
                await ensureImageExists();
                using (var timeout = new CancellationTokenSource(task.EvaluationTimeout))
                {
                    var containerId = await createContainer(timeout.Token);
                    try
                    {
                        await copySolutionIntoContainer(containerId, timeout.Token);
                        await runContainerWaitForExit(containerId, timeout.Token);
                        if (task.ShouldFetchResult)
                            await fetchResultFromContainer(containerId, timeout.Token);
                    }
                    finally
                    {
                        await cleanupContainer(containerId);
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch
            {
            }

            return new RunnerResult();
        }

        private async Task cleanupContainer(string containerId)
        {
            try
            {
                await docker.Containers.RemoveContainerAsync(containerId, new Docker.DotNet.Models.ContainerRemoveParameters() { Force = true, RemoveVolumes = true });
            }
            catch (DockerContainerNotFoundException)
            {
                // TO-DO log
            }
        }

        private async Task fetchResultFromContainer(string containerId, CancellationToken cancellationToken)
        {
            var getFileResponse = await docker.Containers.GetArchiveFromContainerAsync(containerId,
                                    new Docker.DotNet.Models.GetArchiveFromContainerParameters()
                                    {
                                        Path = task.ResultPathInContainer
                                    },
                                    false,
                                    cancellationToken);
            using (var contentStream = getFileResponse.Stream)
                TarHelper.ExtractTo(contentStream, task.ResultPathInMachine);
        }

        private async Task runContainerWaitForExit(string containerId, CancellationToken cancellationToken)
        {
            if (!await docker.Containers.StartContainerAsync(containerId, new Docker.DotNet.Models.ContainerStartParameters(), cancellationToken))
                throw new Exception("Failed to start container");

            var attachResult = await docker.Containers.AttachContainerAsync(containerId, false, new Docker.DotNet.Models.ContainerAttachParameters() { Stderr = true, Stdout = true, Stream = true }, cancellationToken);
            var readContainerLogsTask = attachResult.ReadOutputToEndAsync(cancellationToken);

            await docker.Containers.WaitContainerAsync(containerId, cancellationToken);

            var (logStdout, logStderr) = await readContainerLogsTask;

            if (task.HasResultDirectory)
            {
                var outputFilePath = System.IO.Path.Combine(task.ResultPathInMachine, "output.txt");
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(outputFilePath));
                System.IO.File.WriteAllText(
                        outputFilePath,
                        logStdout + Environment.NewLine + logStderr,
                        System.Text.Encoding.UTF8);
            }
        }

        private async Task copySolutionIntoContainer(string containerId, CancellationToken cancellationToken)
        {
            using (var memStreamForTar = new System.IO.MemoryStream())
            {
                TarHelper.CreateTarFromDirectory(memStreamForTar, task.SolutionDirectoryInMachine);

                memStreamForTar.Seek(0, System.IO.SeekOrigin.Begin);

                await docker.Containers.ExtractArchiveToContainerAsync(containerId,
                    new Docker.DotNet.Models.ContainerPathStatParameters()
                    {
                        Path = task.SolutionDirectoryInContainer,
                        AllowOverwriteDirWithFile = false
                    },
                    memStreamForTar,
                    cancellationToken
                );
            }
        }

        private async Task<string> createContainer(CancellationToken cancellationToken)
        {
            try
            {
                var createContainerParams = new Docker.DotNet.Models.CreateContainerParameters() { Image = task.ImageName, Labels = getContainerLabels() };
                var createContainerResponse = await docker.Containers.CreateContainerAsync(createContainerParams, cancellationToken);

                if (string.IsNullOrEmpty(createContainerResponse.ID))
                    throw new Exception("Container create failied with unknown error");

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

        private IDictionary<string, string> getContainerLabels()
            => new Dictionary<string, string>()
            {
                { "AHK", "1" },
                { "AHK_SolutionDir", task.SolutionDirectoryInMachine }
            };

        private async Task ensureImageExists()
        {
            try
            {
                var findImageResult = await docker.Images.ListImagesAsync(new Docker.DotNet.Models.ImagesListParameters()
                {
                    MatchName = task.ImageName
                });

                if (findImageResult.Any())
                    return;

                await docker.Images.CreateImageAsync(new Docker.DotNet.Models.ImagesCreateParameters()
                {
                    FromImage = task.ImageName
                },
                null,
                null);
            }
            catch (Exception ex)
            {
                throw new Exception("Pulling image failed", ex);
            }
        }

        private static Uri LocalDockerUri()
        {
            // from https://github.com/Microsoft/Docker.DotNet/commit/21832ee6b822671f9ca5ab4ef056e4b37a5f1e3d
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            return isWindows ? new Uri("npipe://./pipe/docker_engine") : new Uri("unix:/var/run/docker.sock");
        }

        public void Dispose() => docker?.Dispose();
    }
}
