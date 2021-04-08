using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Microsoft.Extensions.Logging;

namespace Ahk.TaskRunner
{
    internal class DockerNetwork : DockerObjectBase
    {
        public DockerNetwork(string id, ILogger logger, DockerClient docker)
            : base(id, logger, docker)
        {
        }

        public async Task ConnectContainer(string containerId, string? containerName, CancellationToken cancellationToken)
        {
            try
            {
                var createNetworkParams = new Docker.DotNet.Models.NetworkConnectParameters() { Container = containerId };
                if (!string.IsNullOrEmpty(containerName))
                    createNetworkParams.EndpointConfig = new Docker.DotNet.Models.EndpointSettings() { Aliases = new List<string>() { containerName } };

                await docker.Networks.ConnectNetworkAsync(Id, createNetworkParams, cancellationToken);
                logger.LogTrace($"Network {Id} connected to {containerId}");
            }
            catch (DockerApiException ex)
            {
                throw new Exception($"Network connect to container failed with error: {ex.Message}", ex);
            }
        }

        protected override string ResourceTypeName => @"network";
        protected override Task removeObject() => docker.Networks.DeleteNetworkAsync(Id);
    }
}
