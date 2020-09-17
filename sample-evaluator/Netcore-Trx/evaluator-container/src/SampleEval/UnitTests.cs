using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SampleEval
{
    [TestClass]
    public class UnitTests
    {
        public static string StudentResult;

        [ClassInitialize]
        public static void ClassInit(TestContext testContext)
        {
            var expectedFilePath = "/submission/submission.txt";

            if(!System.IO.File.Exists(expectedFilePath))
                throw new Exception("Submission does not contain required TXT file");

            StudentResult = System.IO.File.ReadAllText(expectedFilePath);
        }

        [TestMethod]
        public void FileNotEmpty()
        {
            Assert.IsTrue(!string.IsNullOrEmpty(StudentResult), "Empty text file");
        }

        [TestMethod]
        public void SolutionIs42()
        {
            if(StudentResult.Contains("84"))
                Assert.Inconclusive("Yields inconclusive to check the results manually");
            
            Assert.IsTrue(StudentResult.Contains("42"), "Invalid result");
        }
    }
}
