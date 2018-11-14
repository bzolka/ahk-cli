using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AHK.Grader
{
    public static class TrxReader
    {
        public static async Task<TrxResult> Read(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(filePath);

            if (!System.IO.File.Exists(filePath))
                throw new System.IO.FileNotFoundException("Trx file not found", filePath);

            int total = 0;
            int passed = 0;
            var failedNames = new List<string>();

            using (var readerStream = System.IO.File.OpenRead(filePath))
            {
                var xdoc = await XDocument.LoadAsync(readerStream, LoadOptions.None, System.Threading.CancellationToken.None);
                foreach (var utr in xdoc.Descendants().Where(x => x.Name.LocalName == "UnitTestResult"))
                {
                    var testName = utr.Attribute("testName").Value ?? "N/A";
                    var outcome = utr.Attribute("outcome").Value;
                    if (outcome == null)
                        continue;

                    ++total;
                    if (outcome.Equals("passed", StringComparison.OrdinalIgnoreCase))
                        ++passed;
                    else
                    {
                        var messages = utr.Descendants().Where(x => x.Name.LocalName == "Message").Select(n => n.Value).ToArray();
                        if (messages.Length == 0)
                            failedNames.Add(testName);
                        else
                            failedNames.Add($"{testName}: {string.Join(' ', messages.Select(formatTestMessage).ToArray())}");
                    }
                }

                return new TrxResult(total, passed, failedNames);
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
    }
}
