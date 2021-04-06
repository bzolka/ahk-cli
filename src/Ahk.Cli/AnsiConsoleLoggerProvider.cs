using Microsoft.Extensions.Logging;

namespace Ahk
{
    internal class AnsiConsoleLoggerProvider : ILoggerProvider
    {
        private static readonly AnsiConsoleLogger logger = new AnsiConsoleLogger();

        public ILogger CreateLogger(string categoryName) => logger;
        public void Dispose() { }
    }
}
