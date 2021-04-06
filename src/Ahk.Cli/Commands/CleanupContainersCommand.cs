using System;
using System.Threading.Tasks;
using Ahk.TaskRunner;
using CliFx.Attributes;
using CliFx.Exceptions;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace Ahk.Commands
{
    [Command("cleanup-containers", Description = "Remove orphan evaluation Docker containers")]
    public class CleanupContainersCommand : CommandBase
    {
        private readonly ILogger logger;

        public CleanupContainersCommand(ILogger logger)
            => this.logger = logger;

        protected override async Task executeCommandCore()
        {
            try
            {
                await AnsiConsole.Status()
                    .StartAsync("Removing orphan evaluation Docker containers",
                        _ => DockerCleanup.Cleanup(logger));

                AnsiConsole.MarkupLine($"[{Color.Green}]Removing orphan evaluation Docker containers completed[/]");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to remove orphan evaluation Docker containers");
                throw new CommandException("Failed to remove orphan evaluation Docker containers");
            }
        }
    }
}
