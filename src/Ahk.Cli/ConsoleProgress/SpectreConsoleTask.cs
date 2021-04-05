using Spectre.Console;

namespace Ahk.ConsoleProgress
{
    public class SpectreConsoleTask : IConsoleTask
    {
        private readonly ProgressTask progressTask;

        public SpectreConsoleTask(ProgressTask progressTask)
            => this.progressTask = progressTask;

        public void SetProgress(double max, double current)
        {
            progressTask.MaxValue = max;
            progressTask.Value = current;
        }
    }
}
