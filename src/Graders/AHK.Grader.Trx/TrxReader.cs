using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;

namespace AHK.Grader.Trx
{
    public class TrxReader
    {
        private readonly GraderResultBuilder resultBuilder = new GraderResultBuilder();
        private readonly ILogger logger;

        public TrxReader(ILogger logger)
            => this.logger = logger;

        public async Task<GraderResult> Parse(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(filePath);

            if (!System.IO.File.Exists(filePath))
                throw new System.IO.FileNotFoundException("Trx file not found", filePath);

            using (var readerStream = System.IO.File.OpenRead(filePath))
                return await Parse(readerStream);
        }

        public async Task<GraderResult> Parse(System.IO.Stream readerStream)
        {
            var xdoc = await XDocument.LoadAsync(readerStream, LoadOptions.None, System.Threading.CancellationToken.None);
            foreach (var utr in xdoc.Descendants().Where(x => x.Name.LocalName == "UnitTestResult"))
                parseUnitTestResult(utr);

            return resultBuilder.ToResult();
        }

        private void parseUnitTestResult(XElement utr)
        {
            var testName = utr.Attribute("testName").Value ?? "N/A";
            getTestNameAndExerciseName(ref testName, out var exerciseName);
            var description = getTestDescriptionFromTestResult(utr);

            var outcome = utr.Attribute("outcome").Value;
            if (outcome == null)
            {
                resultBuilder.AddFailedToGrade(exerciseName, testName, description);
                logger.LogWarning("Trx parser: outcome attribute not found in xml {Xml}", utr.ToString());
                return;
            }

            if (outcome.Equals("NotExecuted", StringComparison.OrdinalIgnoreCase))
            {
                var rawDescription = getTestDescriptionFromTestResult(utr, formatForUserOutput: false);
                if (rawDescription.Contains("Assert.Inconclusive"))
                {
                    resultBuilder.AddInconclusive(exerciseName, testName, description);
                }
                else
                {
                    resultBuilder.AddFailedToGrade(exerciseName, testName, description);
                    logger.LogWarning("Trx parser: outcome is NotExecuted, but messages does not contain text Inconclusive; Description is: {Desc}", description);
                }
            }
            else if (outcome.Equals("Passed", StringComparison.OrdinalIgnoreCase))
            {
                resultBuilder.AddResult(exerciseName, testName, 1, description);
            }
            else if (outcome.Equals("Failed", StringComparison.OrdinalIgnoreCase))
            {
                resultBuilder.AddResult(exerciseName, testName, 0, description);
            }
            else
            {
                resultBuilder.AddFailedToGrade(exerciseName, testName, description);
                logger.LogWarning("Trx parser: outcome is unknown: {Outcome}", outcome);
            }
        }

        private static string getTestDescriptionFromTestResult(XElement unitTestResultElement, bool formatForUserOutput = true)
        {
            var messages = unitTestResultElement.Descendants().Where(x => x.Name.LocalName == "Message").Select(n => n.Value).ToArray();
            if (messages.Length > 0)
            {
                if (formatForUserOutput)
                    messages = messages.Select(formatTestMessage).ToArray();

                return string.Join(' ', messages).Trim();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Cleans up the test message, removed texts relatedd to Exceptions and Asserts.
        /// Keeps the text that is relevant to the student.
        /// </summary>
        private static string formatTestMessage(string testMessage)
        {
            if (testMessage == null)
                return string.Empty;

            // Remove texts like "Assembly initialization failed with System.Exception."
            testMessage = removeSubstringMatchingRegex(testMessage, @"\s*(Assembly|Class|Test) initialization .* exception\.\s*");

            // Remove texts like "System.Exception: System.ArgumentNullException:"
            testMessage = removeSubstringMatchingRegex(testMessage, @"\s*[\w\.]+Exception:\s*");

            // Remove messages like "Assert.IsTrue failed."
            // Pattern of these is from https://github.com/Microsoft/testfx/blob/3197e7bb9f7fd4219c7855be575fb5def205aff7/src/TestFramework/MSTest.Core/Assertions/Assert.cs
            testMessage = removeSubstringMatchingRegex(testMessage, @"\s*Assert\.\w+ failed.\s*");

            return testMessage;
        }

        private static string removeSubstringMatchingRegex(string s, string regexPattern)
            => System.Text.RegularExpressions.Regex.Replace(s, regexPattern, string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase, TimeSpan.FromSeconds(5));

        private static void getTestNameAndExerciseName(ref string testName, out string exerciseName)
        {
            if (testName.Contains("___"))
            {
                var idx = testName.IndexOf("___");
                exerciseName = testName.Substring(0, idx);
                testName = testName.Substring(idx + 3);
            }
            else
            {
                exerciseName = string.Empty;
            }
        }
    }
}
