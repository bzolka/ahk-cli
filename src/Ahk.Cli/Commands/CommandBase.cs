using System.Threading.Tasks;
using CliFx;
using CliFx.Infrastructure;

namespace Ahk.Commands
{
    public abstract class CommandBase : ICommand
    {
        public async ValueTask ExecuteAsync(IConsole console)
        {
            console.Output.WriteLine($"{AppName.Name} {AppName.Version}");
            console.Output.WriteLine();

            await executeCommandCore();
        }

        protected abstract Task executeCommandCore();
    }
}
