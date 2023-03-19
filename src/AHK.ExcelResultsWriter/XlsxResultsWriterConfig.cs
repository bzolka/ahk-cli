using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHK.ExcelResultsWriter
{
    public class XlsxResultsWriterConfig
    {
        public readonly string ResultsXlsxFileName ;
        /// <summary>
        /// If true, only minimal info is written to the Comment field:
        /// - No points are logged (use only when point can be 0 or one)
        /// - No entry is added about tests which are successful and have no description (any additional text).
        /// - Extra short text is added for inclonclusive test and for test for which grading fails.
        /// </summary>
        public readonly bool Comment_MinimalOutput;

        public XlsxResultsWriterConfig(string resultsXlsxFileName,
            bool comment_SkipSuccessfullTests)
        {
            ResultsXlsxFileName = resultsXlsxFileName;
            Comment_MinimalOutput = comment_SkipSuccessfullTests;
        }
    }
}
