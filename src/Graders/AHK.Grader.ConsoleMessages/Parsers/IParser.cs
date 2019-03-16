namespace AHK.Grader.ConsoleMessages.Parsers
{
    interface IParser
    {
        bool Parse(System.Collections.Generic.IEnumerable<string> content, GraderResultBuilder resultBuilder, Microsoft.Extensions.Logging.ILogger logger);
    }
}
