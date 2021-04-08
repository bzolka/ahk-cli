using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Microsoft.Extensions.Logging;

namespace Ahk.TaskRunner
{
    public class DockerRunnerMultiContainer : DockerRunner
    {
        public DockerRunnerMultiContainer(DockerRunnerTask task, ILogger logger, ITempPathProvider? tempPathProvider = null)
            : base(task, logger, tempPathProvider)
        {
        }

        protected override async Task pullImages(DockerClient docker)
        {
            await base.pullImages(docker);
            await ImagePuller.EnsureImageExists(docker, task.ServiceContainer!, logger);
        }

        protected override async Task<string> runCore(DockerClient docker, CancellationToken timeout, string solutionFolderToMount)
        {
            await using var containerNetwork = await NetworkCreationHelper.CreateNewNetwork(logger, docker, timeout);

            await using var serviceContainer = await ContainerCreationHelper.CreateNewContainer(task.ServiceContainer!, docker, logger, timeout);
            await containerNetwork.ConnectContainer(serviceContainer.Id, task.ServiceContainer!.Name, timeout);
            await serviceContainer.StartContainer(timeout);

            await using var mainContainer = await ContainerCreationHelper.CreateNewContainer(task.Container, docker, logger, timeout, cp => configureMount(cp, solutionFolderToMount));
            await containerNetwork.ConnectContainer(mainContainer.Id, task.Container.Name, timeout);
            return await mainContainer.RunContainerWaitForExit(timeout);
        }
    }
}
