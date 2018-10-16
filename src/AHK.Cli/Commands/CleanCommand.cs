using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AHK
{
    internal static class CleanCommand
    {
        public static async Task<int> Execute(ILogger logger)
        {
            await TaskRunner.DockerCleanup.Cleanup(logger);
            return 0;
        }
    }
}
