using System;
using System.Linq;
using AHK.Grader.ConsoleMessages.Parsers;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AHK.Grader.ConsoleMessages.Tests
{
    [TestClass]
    public class TestResultParserTests
    {
        [TestMethod]
        public void ParsePassedNoComment()
        {
            var parseSuccess = parseContent("apple#passed", out var result);
            var tr = assertResult(result, null, "apple");

            Assert.IsTrue(parseSuccess);
            Assert.AreEqual(1, tr.ResultPoints);
            Assert.AreEqual(string.Empty, tr.Description);
        }

        [TestMethod]
        public void ParsePassedWithComment()
        {
            var parseSuccess = parseContent("apple#passed#this is the comment", out var result);
            var tr = assertResult(result, null, "apple");

            Assert.IsTrue(parseSuccess);
            Assert.AreEqual(1, tr.ResultPoints);
            Assert.AreEqual("this is the comment", tr.Description);
        }

        [TestMethod]
        public void ParseFailedNoComment()
        {
            var parseSuccess = parseContent("apple#failed", out var result);
            var tr = assertResult(result, null, "apple");

            Assert.IsTrue(parseSuccess);
            Assert.AreEqual(0, tr.ResultPoints);
            Assert.AreEqual(string.Empty, tr.Description);
        }

        [TestMethod]
        public void ParseFailedWithComment()
        {
            var parseSuccess = parseContent("apple#failed#this is the comment", out var result);
            var tr = assertResult(result, null, "apple");

            Assert.IsTrue(parseSuccess);
            Assert.AreEqual(0, tr.ResultPoints);
            Assert.AreEqual("this is the comment", tr.Description);
        }

        [TestMethod]
        public void ParseInconclusiveNoComment()
        {
            var parseSuccess = parseContent("apple#inconclusive", out var result);
            var tr = assertResult(result, null, "apple", GradingOutcomes.Inconclusive);

            Assert.IsTrue(parseSuccess);
            Assert.AreEqual(0, tr.ResultPoints);
            Assert.AreEqual(string.Empty, tr.Description);
        }

        [TestMethod]
        public void ParseInconclusiveWithComment()
        {
            var parseSuccess = parseContent("apple#inconclusive#this is the comment", out var result);
            var tr = assertResult(result, null, "apple", GradingOutcomes.Inconclusive);

            Assert.IsTrue(parseSuccess);
            Assert.AreEqual(0, tr.ResultPoints);
            Assert.AreEqual("this is the comment", tr.Description);
        }

        [TestMethod]
        public void ParseNumerericNoComment()
        {
            var parseSuccess = parseContent("apple#78", out var result);
            var tr = assertResult(result, null, "apple");

            Assert.IsTrue(parseSuccess);
            Assert.AreEqual(78, tr.ResultPoints);
            Assert.AreEqual(string.Empty, tr.Description);
        }

        [TestMethod]
        public void ParseNumerericWithComment()
        {
            var parseSuccess = parseContent("apple#78#this is the comment", out var result);
            var tr = assertResult(result, null, "apple");

            Assert.IsTrue(parseSuccess);
            Assert.AreEqual(78, tr.ResultPoints);
            Assert.AreEqual("this is the comment", tr.Description);
        }

        [TestMethod]
        public void ParseNumerericWithExerciseNameAndComment()
        {
            var parseSuccess = parseContent("ex1@apple#2#this is the comment", out var result);
            var tr = assertResult(result, "ex1", "apple");

            Assert.IsTrue(parseSuccess);
            Assert.AreEqual(2, tr.ResultPoints);
            Assert.AreEqual("this is the comment", tr.Description);
        }

        [TestMethod]
        public void ParseSyntaxError1()
        {
            var parseSuccess = parseContent("not good at all", out var result);
            Assert.IsFalse(parseSuccess);
            Assert.IsFalse(result.HasResult);
        }

        [TestMethod]
        public void ParseSyntaxError2()
        {
            var parseSuccess = parseContent("apple#not valid result", out var result);
            Assert.IsFalse(parseSuccess);
            Assert.IsFalse(result.HasResult);
        }

        [TestMethod]
        public void ParseMultilineComment()
        {
            var str =
@"apple#failed#this is the comment
and some
more comment";
            var parseSuccess = parseContent(str, out var result);
            var tr = assertResult(result, null, "apple");

            Assert.IsTrue(parseSuccess);
            Assert.AreEqual(0, tr.ResultPoints);
            Assert.IsTrue(tr.Description.StartsWith("this is the comment"));
            Assert.IsTrue(tr.Description.EndsWith("more comment"));
        }

        private TestResult assertResult(GraderResult result, string exerciseName, string testName, GradingOutcomes expectedTestOutcome = GradingOutcomes.Graded)
        {
            if (result == null)
                throw new ArgumentNullException();
            Assert.IsTrue(result.HasResult, "result is empty");

            ExerciseResult exerciseResult;
            if (string.IsNullOrEmpty(exerciseName))
                exerciseResult = result.Exercises.FirstOrDefault();
            else
                exerciseResult = result.Exercises.FirstOrDefault(er => er.ExerciseName == exerciseName);
            Assert.IsNotNull(exerciseResult, "cannot find exercise");

            var testResult = exerciseResult.TestsResults.FirstOrDefault(tr => tr.TestName == testName);
            Assert.IsNotNull(testResult, "cannot find test");

            Assert.AreEqual(expectedTestOutcome, testResult.GradingOutcome);

            return testResult;
        }

        private static bool parseContent(string contentString, out GraderResult result)
        {
            var parser = new TestResultParser();
            var graderResultBuilder = new GraderResultBuilder();

            var outcome = parser.Parse(contentString.Split('#'), graderResultBuilder, NullLogger.Instance);
            result = graderResultBuilder.ToResult();
            return outcome;
        }
    }
}
