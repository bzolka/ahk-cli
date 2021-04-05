namespace Ahk.ConsoleProgress
{
    internal interface IConsoleProgress
    {
        IConsoleTask BeginTask(string name);
    }
}
