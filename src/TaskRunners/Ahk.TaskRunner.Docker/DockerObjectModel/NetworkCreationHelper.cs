using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Ahk.TaskRunner
{
    internal static class NetworkCreationHelper
    {
        public static async Task<DockerNetwork> CreateNewNetwork(ILogger logger, Docker.DotNet.DockerClient docker, CancellationToken cancellationToken)
        {
            try
            {
                var createNetworkParams = new Docker.DotNet.Models.NetworksCreateParameters()
                {
                    Driver = "bridge",
                    Name = Guid.NewGuid().ToString(),
                    Internal = true,
                    Labels = DockerObjectBase.LabelsForCreation,
                };

                var createNetworkResponse = await docker.Networks.CreateNetworkAsync(createNetworkParams, cancellationToken);

                if (string.IsNullOrEmpty(createNetworkResponse.ID))
                    throw new Exception("Network create failed with unknown error");

                logger.LogTrace($"Network created with ID {createNetworkResponse.ID}");
                return new DockerNetwork(createNetworkResponse.ID, logger, docker);
            }
            catch (Docker.DotNet.DockerApiException ex)
            {
                throw new Exception($"Network create failed with error: {ex.Message}", ex);
            }
        }
    }
}
