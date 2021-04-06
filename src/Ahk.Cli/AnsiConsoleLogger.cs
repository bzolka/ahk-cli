using System;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace Ahk
{
    internal class AnsiConsoleLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;
        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            var text = formatter(state, exception);
            if (string.IsNullOrEmpty(text))
                return;

            AnsiConsole.MarkupLine(getMarkup(text, logLevel));
            if (exception != null)
                AnsiConsole.WriteLine(exception.ToString());
        }

        private string getMarkup(string text, LogLevel logLevel)
            => $"[{getColor(logLevel)}]{logLevel}[/] {text}";

        private Color getColor(LogLevel logLevel)
            => logLevel switch
            {
                LogLevel.Warning => Color.Yellow,
                LogLevel.Error => Color.Red,
                LogLevel.Critical => Color.Red,
                _ => Color.Default,
            };

        private class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new NullScope();

            public void Dispose() { }
        }
    }
}
