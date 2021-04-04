using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Ahk.Grader.ConsoleMessages
{
    public class ConsoleMessagesReader
    {
        private const string MessagePrefix = @"###ahk#";

        private readonly string expectedValidationCode;
        private readonly ILogger logger;

        private readonly IReadOnlyDictionary<string, Type> parsers = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
        {
            { Parsers.TestResultParser.Keyword, typeof(Parsers.TestResultParser) }
        };

        private readonly GraderResultBuilder resultBuilder = new GraderResultBuilder();

        public ConsoleMessagesReader(string expectedValidationCode, ILogger logger)
        {
            this.expectedValidationCode = expectedValidationCode;
            this.logger = logger;
        }

        public async Task<GraderResult> Grade(string consoleLog)
        {
            using (var reader = new StringReader(consoleLog))
            {
                while (true)
                {
                    var line = await reader.ReadLineAsync();
                    if (line == null)
                        break;

                    if (line.StartsWith(MessagePrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        var entry = line.TrimEnd().TrimEnd('\\'); // first trim trailing whitespaces, then trim \ charaters
                        while (line.TrimEnd().EndsWith('\\'))
                        {
                            line = await reader.ReadLineAsync();

                            if (line != null)
                                entry += Environment.NewLine + line.TrimEnd().TrimEnd('\\');
                            else
                                break;
                        }

                        parseMessage(entry);
                    }
                }
            }

            return resultBuilder.ToResult();
        }

        private void parseMessage(string message)
        {
            var values = message.Substring(MessagePrefix.Length).Split('#');

            if (values.Length < 2)
            {
                logger.LogWarning("Console message grader: syntax error in line {Line}.", message);
                return;
            }

            var validationCode = values[0];
            if (validationCode != expectedValidationCode)
            {
                logger.LogWarning("Console message grader: invalid validation code {InvalidCode} expected {ExpectedCode}.", validationCode, expectedValidationCode);
                return;
            }

            var keyword = values[1]; // determines the type of message
            var content = values.Skip(2); // the remaining parts of the line, the effective content
            if (parsers.TryGetValue(keyword, out var parserType))
            {
                var parserInstance = Activator.CreateInstance(parserType) as Parsers.IParser;
                var parserSuccess = parserInstance.Parse(content, resultBuilder, logger);

                if (!parserSuccess)
                    resultBuilder.AddFailedToGrade(GraderResultBuilder.UnknownExercise, GraderResultBuilder.UnknownTest, "invalid result");
            }
            else
            {
                logger.LogWarning($"Console message grader: unrecognized operation {keyword}");
            }
        }
    }
}
