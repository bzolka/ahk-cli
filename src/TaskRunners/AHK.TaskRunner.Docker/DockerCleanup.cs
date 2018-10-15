using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AHK.TaskRunner
{
    public static class DockerCleanup
    {
        internal const string ContainerLabel = "AHK";

        public static async Task Cleanup(ILogger logger)
        {
            using (var docker = DockerConnectionHelper.GetConnectionConfiguration().CreateClient())
            {
                var containers = await docker.Containers.ListContainersAsync(new Docker.DotNet.Models.ContainersListParameters() {
                    All = true,
                    Filters = new Dictionary<string, IDictionary<string, bool>>()
                    {
                        { "label", new Dictionary<string, bool>(){ { ContainerLabel, true } } }
                    }
                });

                if (containers.Count == 0)
                    return;

                logger.LogWarning("Found {Count} Docker containers", containers.Count);

                foreach (var c in containers)
                {
                    try
                    {
                        await docker.Containers.RemoveContainerAsync(c.ID, new Docker.DotNet.Models.ContainerRemoveParameters() { Force = true, RemoveVolumes = true });
                        logger.LogInformation("Removed {ID} Docker container", c.ID);
                    }
                    catch
                    {
                        logger.LogWarning("Cannot remove {ID} Docker container in {State} state", c.ID, c.State);
                    }
                }
            }
        }
    }
}
