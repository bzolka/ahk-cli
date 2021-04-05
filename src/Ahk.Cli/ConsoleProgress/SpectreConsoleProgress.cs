using Spectre.Console;

namespace Ahk.ConsoleProgress
{
    public class SpectreConsoleProgress : IConsoleProgress
    {
        private readonly ProgressContext progressContext;

        public SpectreConsoleProgress(ProgressContext progressContext)
            => this.progressContext = progressContext;

        public IConsoleTask BeginTask(string name)
        {
            var task = this.progressContext.AddTask(name);
            return new SpectreConsoleTask(task);
        }
    }
}
