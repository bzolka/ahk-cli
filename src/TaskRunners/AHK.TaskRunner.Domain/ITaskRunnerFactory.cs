using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AHK.TaskRunner
{
    public interface ITaskRunnerFactory
    {
        ITaskRunner CreateRunner(RunnerTask task);
        Task Cleanup(ILogger logger);
    }
}
