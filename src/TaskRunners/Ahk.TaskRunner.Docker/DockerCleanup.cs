using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Ahk.TaskRunner
{
    public static class DockerCleanup
    {
        public static async Task Cleanup(ILogger logger)
        {
            using (var docker = DockerConnectionHelper.GetConnectionConfiguration().CreateClient())
            {
                // containers first, then networks so that no container is connected to a network
                await cleanupContainers(logger, docker);
                await cleanupNetworks(logger, docker);
            }
        }

        private static async Task cleanupContainers(ILogger logger, Docker.DotNet.DockerClient docker)
        {
            var items = await docker.Containers.ListContainersAsync(new Docker.DotNet.Models.ContainersListParameters() { Filters = DockerObjectBase.LabelsForFilter });
            foreach (var c in items)
                await new DockerContainer(c.ID, logger, docker).DisposeAsync();
        }

        private static async Task cleanupNetworks(ILogger logger, Docker.DotNet.DockerClient docker)
        {
            var items = await docker.Networks.ListNetworksAsync(new Docker.DotNet.Models.NetworksListParameters() { Filters = DockerObjectBase.LabelsForFilter });
            foreach (var n in items)
                await new DockerNetwork(n.ID, logger, docker).DisposeAsync();
        }
    }
}
