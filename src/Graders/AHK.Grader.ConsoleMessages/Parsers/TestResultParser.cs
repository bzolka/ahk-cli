using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace AHK.Grader.ConsoleMessages.Parsers
{
    public class TestResultParser : IParser
    {
        public const string Keyword = @"testresult";

        private const string TestPassedMessage = @"passed";
        private const string TestFailedMessage = @"failed";
        private const string TestInconclusiveMessage = @"inconclusive";

        public bool Parse(IEnumerable<string> content, GraderResultBuilder resultBuilder, ILogger logger)
        {
            // <preamble>#testresult#exercise_name@test_name#result#comment

            var parts = new Stack<string>(content.Reverse());

            // must have these two
            if (!parts.TryPop(out string testName))
            {
                logger.LogWarning($"Console messages reader: testresult operation with too few parameters");
                return false;
            }
            if (!parts.TryPop(out string resultString))
            {
                logger.LogWarning($"Console messages reader: testresult operation with too few parameters");
                return false;
            }

            // other parts are optional
            parts.TryPop(out string comment);

            if (!tryGetTestNameAndExerciseName(ref testName, out string exerciseName))
                return false;

            if (!tryProcessResultString(testName, exerciseName, resultString, comment, resultBuilder, logger))
                return false;

            return true;
        }

        private bool tryProcessResultString(string testName, string exerciseName, string resultString, string comment, GraderResultBuilder resultBuilder, ILogger logger)
        {
            if (resultString.Equals(TestPassedMessage, System.StringComparison.OrdinalIgnoreCase))
            {
                resultBuilder.AddResult(exerciseName, testName, 1, comment);
                return true;
            }
            else if (resultString.Equals(TestFailedMessage, System.StringComparison.OrdinalIgnoreCase))
            {
                resultBuilder.AddResult(exerciseName, testName, 0, comment);
                return true;
            }
            else if (resultString.Equals(TestInconclusiveMessage, System.StringComparison.OrdinalIgnoreCase))
            {
                resultBuilder.AddInconclusive(exerciseName, testName, comment);
                return true;
            }
            else if (int.TryParse(resultString, out int points))
            {
                resultBuilder.AddResult(exerciseName, testName, points, comment);
                return true;
            }
            else
            {
                logger.LogWarning($"Console messages reader: testresult with unrecognized status {resultString}");
                return false;
            }
        }

        private static bool tryGetTestNameAndExerciseName(ref string testName, out string exerciseName)
        {
            exerciseName = string.Empty;

            if (string.IsNullOrEmpty(testName))
                return false;

            if (testName.Contains('@'))
            {
                var idx = testName.IndexOf('@');
                exerciseName = testName.Substring(0, idx);
                testName = testName.Substring(idx + 1);
            }

            return true;
        }
    }
}
