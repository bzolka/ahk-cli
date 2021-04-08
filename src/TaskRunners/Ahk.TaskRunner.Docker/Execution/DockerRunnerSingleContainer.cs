using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Ahk.TaskRunner
{
    public class DockerRunnerSingleContainer : DockerRunner
    {
        public DockerRunnerSingleContainer(DockerRunnerTask task, ILogger logger, ITempPathProvider? tempPathProvider = null)
            : base(task, logger, tempPathProvider)
        {
        }

        protected override async Task<string> runCore(Docker.DotNet.DockerClient docker, CancellationToken timeout, string solutionFolderToMount)
        {
            await using var container = await ContainerCreationHelper.CreateNewContainer(task.Container, docker, logger, timeout, cp => configureMount(cp, solutionFolderToMount));
            return await container.RunContainerWaitForExit(timeout);
        }

    }
}
