using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AHK.Grader
{
    public class ConsoleMessagesReader
    {
        private const string LinePrefix = @"###ahk#";
        private const string TestPassedMessage = @"passed";
        private const string TestFailedMessage = @"failed";

        private readonly string expectedValidationCode;
        private readonly ILogger logger;

        private int totalTests = 0;
        private int passedTests = 0;
        private List<string> failedTestNames = new List<string>();

        public ConsoleMessagesReader(string expectedValidationCode, ILogger logger)
        {
            this.expectedValidationCode = expectedValidationCode;
            this.logger = logger;
        }

        public async Task<ConsoleMessagesTestResult> Read(string consoleLog)
        {
            using (var f = new StringReader(consoleLog))
            {
                string line;
                while ((line = await f.ReadLineAsync()) != null)
                {
                    if (line.StartsWith(LinePrefix, System.StringComparison.OrdinalIgnoreCase))
                    {
                        var values = line.Substring(LinePrefix.Length).Split('#');

                        if (values.Length < 2)
                        {
                            logger.LogWarning("Console message grader: syntax error in line.");
                            continue;
                        }

                        var validationCode = values[0];
                        if (validationCode != expectedValidationCode)
                        {
                            logger.LogWarning("Console message grader: invalid validation code.");
                            continue;
                        }

                        var operation = values[1];
                        switch (operation)
                        {
                            case "testresult":
                                readTestResult(values.Skip(2));
                                break;
                            default:
                                logger.LogWarning($"Console message grader: unrecognized operation {operation}");
                                break;
                        }
                    }
                }
            }

            return new ConsoleMessagesTestResult(totalTests, passedTests, failedTestNames);
        }

        private void readTestResult(IEnumerable<string> messageParts)
        {
            var messagePartsArray = messageParts.ToArray();
            if (messagePartsArray.Length < 2)
            {
                logger.LogWarning($"Console messages reader: testresult operation with too few parameters");
                return;
            }

            string testName = messagePartsArray[0] ?? "N/A";
            string resultString = messagePartsArray[1] ?? string.Empty;
            string comment = messagePartsArray.Length > 2 ? (messagePartsArray[2] ?? string.Empty) : string.Empty;

            if (resultString.Equals(TestPassedMessage, System.StringComparison.OrdinalIgnoreCase))
            {
                ++totalTests;
                ++passedTests;
            }
            else if (resultString.Equals(TestFailedMessage, System.StringComparison.OrdinalIgnoreCase))
            {
                ++totalTests;
                if (string.IsNullOrEmpty(comment))
                    failedTestNames.Add(testName);
                else
                    failedTestNames.Add($"{testName} ({comment})");
            }
            else
            {
                // does not count into the total number
                logger.LogWarning($"Console messages reader: testresult with unrecognized status {resultString}");
            }
        }
    }
}
