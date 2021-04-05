using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ahk.Grader.Trx.Tests
{
    [TestClass]
    public class TrxReaderTests
    {
        [TestMethod]
        public async Task GradeSample1Trx()
        {
            var reader = new TrxReader(Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance);
            using (var sampleTrx = getSampleFile("sample1.trx"))
            {
                var result = await reader.Parse(sampleTrx);

                Assert.IsTrue(result.HasResult);

                var exResult = result.Exercises![0];
                Assert.AreEqual(4, exResult.TestsResults.Count);

                var t1 = exResult.TestsResults.SingleOrDefault(x => x.TestName == "TPassed");
                Assert.IsNotNull(t1);
                Assert.AreEqual(1, t1.ResultPoints);
                Assert.AreEqual(GradingOutcomes.Graded, t1.GradingOutcome);

                var t2 = exResult.TestsResults.SingleOrDefault(x => x.TestName == "TFailed1");
                Assert.IsNotNull(t2);
                Assert.AreEqual(0, t2.ResultPoints);
                Assert.AreEqual(GradingOutcomes.Graded, t2.GradingOutcome);

                var t3 = exResult.TestsResults.SingleOrDefault(x => x.TestName == "TFailed2");
                Assert.IsNotNull(t3);
                Assert.AreEqual(0, t3.ResultPoints);
                Assert.AreEqual(GradingOutcomes.Graded, t3.GradingOutcome);

                var t4 = exResult.TestsResults.SingleOrDefault(x => x.TestName == "TInconclusive");
                Assert.IsNotNull(t4);
                Assert.AreEqual(0, t4.ResultPoints);
                Assert.AreEqual(GradingOutcomes.Inconclusive, t4.GradingOutcome);
            }
        }

        private static System.IO.Stream getSampleFile(string filename)
            => Assembly.GetExecutingAssembly().GetManifestResourceStream("Ahk.Grader.Trx.Tests.assets." + filename)!;
    }
}
