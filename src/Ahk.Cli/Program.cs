using System.Threading.Tasks;
using CliFx;
using Microsoft.Extensions.DependencyInjection;

namespace Ahk
{
    class Program
    {
        static async Task<int> Main(string[] args)
            => await new CliApplicationBuilder()
                            .SetTitle(AppName.Name)
                            .SetDescription(AppName.Description)
                            .SetVersion(AppName.Version)
                            .AddCommandsFromThisAssembly()
                            .UseTypeActivator(Startup.GetServiceProvider().GetRequiredService)
                            .Build()
                            .RunAsync(args);
    }
}
