namespace Ahk.ConsoleProgress
{
    public interface IConsoleTask
    {
        void SetProgress(double max, double current);
    }
}
