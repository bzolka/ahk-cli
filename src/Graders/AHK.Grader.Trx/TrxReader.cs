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
                            failedNames.Add($"{testName}: {string.Join(' ', messages)}");
                    }
                }

                return new TrxResult(total, passed, failedNames);
            }

            //var summary = xdoc.Descendants().FirstOrDefault(x => x.Name.LocalName == "ResultSummary");
            //if (summary != null)
            //{
            //    var counters = summary.Descendants().FirstOrDefault(x => x.Name.LocalName == "Counters");
            //    if (counters != null)
            //    {
            //        var total = int.Parse(counters.Attribute("total").Value);
            //        var passed = int.Parse(counters.Attribute("passed").Value);
            //    }
            //}
        }
    }
}
