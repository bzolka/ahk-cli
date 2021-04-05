using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ahk.Grader.ConsoleMessages.Tests
{
    [TestClass]
    public class ConsoleMessagesReaderTests
    {
        public const string ValidationCode = @"aBc12";

        [TestMethod]
        public async Task EmptyLog()
        {
            var result = await executeGrading(string.Empty);

            Assert.IsFalse(result.HasResult);
            Assert.AreEqual(GradingOutcomes.FailedToGrade, result.GradingOutcome);
        }

        [TestMethod]
        public async Task SkipUnknownKeywords()
        {
            var result = await executeGrading(
@"some text
###ahk#aBc12#hfdkjshfdkjshfdkljshf#apple#passed
more lines"
);

            Assert.IsFalse(result.HasResult);
            Assert.AreEqual(GradingOutcomes.FailedToGrade, result.GradingOutcome);
        }

        [TestMethod]
        public async Task SkipInvalidCode()
        {
            var result = await executeGrading(
@"some text
###ahk#InvalidCode#testresult#apple#passed
more lines
###ahk#OtherInvalidCode#testresult#banana#failed
apple and banana"
);

            Assert.IsFalse(result.HasResult);
            Assert.AreEqual(GradingOutcomes.FailedToGrade, result.GradingOutcome);
        }

        [TestMethod]
        public async Task ParseTwoResults()
        {
            var result = await executeGrading(
@"some text
some more
###ahk#aBc12#testresult#apple#passed
more lines
of just plain text
###ahk#aBc12#testresult#banana#failed
apple and banana"
);

            Assert.IsTrue(result.HasResult);
            Assert.AreEqual(GradingOutcomes.Graded, result.GradingOutcome);

            Assert.AreEqual(1, result.Exercises!.Count);
            Assert.AreEqual(1, result.Exercises[0].SumResultPoints);

            var testResults = result.Exercises[0].TestsResults;
            Assert.AreEqual(2, testResults.Count);

            var tr1 = testResults.SingleOrDefault(x => x.TestName == "apple");
            Assert.IsNotNull(tr1);
            Assert.AreEqual(GradingOutcomes.Graded, tr1.GradingOutcome);
            Assert.AreEqual(1, tr1.ResultPoints);

            var tr2 = testResults.SingleOrDefault(x => x.TestName == "banana");
            Assert.IsNotNull(tr2);
            Assert.AreEqual(GradingOutcomes.Graded, tr2.GradingOutcome);
            Assert.AreEqual(0, tr2.ResultPoints);

            var tr3 = testResults.SingleOrDefault(x => x.TestName == "apple and banana");
            Assert.IsNull(tr3);
        }

        [TestMethod]
        public async Task SkipInvalidCodeParseOneResult()
        {
            var result = await executeGrading(
@"some text
some more
###ahk#InvalidCode#testresult#apple#passed
more lines
of just plain text
###ahk#aBc12#testresult#banana#failed
apple and banana"
);

            Assert.IsTrue(result.HasResult);
            Assert.AreEqual(GradingOutcomes.Graded, result.GradingOutcome);

            Assert.AreEqual(1, result.Exercises!.Count);

            var testResults = result.Exercises[0].TestsResults;
            Assert.AreEqual(1, testResults.Count);

            var tr1 = testResults.SingleOrDefault(x => x.TestName == "apple");
            Assert.IsNull(tr1);

            var tr2 = testResults.SingleOrDefault(x => x.TestName == "banana");
            Assert.IsNotNull(tr2);
            Assert.AreEqual(GradingOutcomes.Graded, tr2.GradingOutcome);
            Assert.AreEqual(0, tr2.ResultPoints);
        }

        [TestMethod]
        public async Task ParseMultiline()
        {
            var result = await executeGrading(
@"some text
some more
###ahk#aBc12#testresult#banana#failed#this is line 1\
this is line 2\
this is last line
apple and banana
###ahk#aBc12#testresult#apple#failed#single line
apple and banana
apple and banana"
);

            Assert.IsTrue(result.HasResult);
            Assert.AreEqual(GradingOutcomes.Graded, result.GradingOutcome);

            var testResults = result.Exercises![0].TestsResults;
            Assert.AreEqual(2, testResults.Count);

            var tr = testResults.SingleOrDefault(x => x.TestName == "banana");
            Assert.IsNotNull(tr);
            Assert.AreEqual(GradingOutcomes.Graded, tr.GradingOutcome);
            Assert.AreEqual(0, tr.ResultPoints);
            Assert.IsTrue(tr.Description.Contains("line 1"));
            Assert.IsTrue(tr.Description.Contains("line 2"));
            Assert.IsTrue(tr.Description.EndsWith("last line"));
            Assert.IsFalse(tr.Description.Contains("\\"));

            tr = testResults.SingleOrDefault(x => x.TestName == "apple");
            Assert.IsNotNull(tr);
        }

        private static async Task<GraderResult> executeGrading(string log)
        {
            var reader = new ConsoleMessagesReader(ValidationCode, Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance);
            return await reader.Grade(log);
        }
    }
}
