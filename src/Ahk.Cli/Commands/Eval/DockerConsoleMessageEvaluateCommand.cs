using System;
using Ahk.Grader;
using Ahk.TaskRunner;
using CliFx.Attributes;
using Microsoft.Extensions.Logging;

namespace Ahk.Commands.Eval
{
    [Command("eval docker consolemessage", Description = "Run evaluation using Docker container and ConsoleMessage grader")]
    public class DockerConsoleMessageEvaluateCommand : DockerEvaluateCommandBase
    {
        public DockerConsoleMessageEvaluateCommand(ILogger logger)
            : base(logger)
        {
        }

        [CommandOption("validationcode", Description = "Validation code expected in every message printed to console by the evaluation application.", IsRequired = true)]
        public string ValidationCode { get; set; } = string.Empty;

        protected override IGrader CreateGrader(string submissionSource, string studentId, string artifactPath, RunnerResult runnerResult)
            => new ConsoleMessagesGrader(new ConsoleMessagesGraderTask(submissionSource, studentId, runnerResult.ConsoleOutput, this.ValidationCode), logger);

        protected override string sanitizeContainerConsoleOutput(string text)
        {
            var builder = new System.Text.StringBuilder(text.Length);

            using (var sr = new System.IO.StringReader(text))
            {
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    // skip lines that are ment for the ConsoleMessagesGrader
                    if (line.StartsWith(@"###ahk", StringComparison.OrdinalIgnoreCase))
                        continue;

                    // remove the validation code from any line as a safety measure
                    line = line.Replace(ValidationCode, "{***}");

                    builder.AppendLine(line);
                }
            }

            return builder.ToString();
        }
    }
}
