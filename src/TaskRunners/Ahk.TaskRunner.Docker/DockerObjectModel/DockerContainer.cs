using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Ahk.TaskRunner
{
    internal class DockerContainer : DockerObjectBase
    {
        public DockerContainer(string id, ILogger logger, Docker.DotNet.DockerClient docker)
            : base(id, logger, docker)
        {
        }

        public async Task StartContainer(CancellationToken cancellationToken)
        {
            if (!await docker.Containers.StartContainerAsync(Id, new Docker.DotNet.Models.ContainerStartParameters(), cancellationToken))
                throw new Exception("Failed to start container");
        }

        public async Task<string> RunContainerWaitForExit(CancellationToken cancellationToken)
        {
            await StartContainer(cancellationToken);

            logger.LogTrace("Container running");

            var attachResult = await docker.Containers.AttachContainerAsync(Id, false, new Docker.DotNet.Models.ContainerAttachParameters() { Stderr = true, Stdout = true, Stream = true }, cancellationToken);
            var readContainerLogsTask = attachResult.ReadOutputToEndAsync(cancellationToken);

            await docker.Containers.WaitContainerAsync(Id, cancellationToken);

            logger.LogTrace("Container stopped");

            var (logStdout, logStderr) = await readContainerLogsTask;

            return logStdout + Environment.NewLine + logStderr;
        }

        protected override string ResourceTypeName => @"container";
        protected override Task removeObject() => docker.Containers.RemoveContainerAsync(Id, new Docker.DotNet.Models.ContainerRemoveParameters() { Force = true, RemoveVolumes = true });
    }
}
