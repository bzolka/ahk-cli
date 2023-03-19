using System.Collections.Generic;
using AHK.ExcelResultsWriter;

namespace AHK.Execution
{
    public class RunConfig
    {
        public readonly IReadOnlyList<ExecutionTask> Tasks;
        public readonly XlsxResultsWriterConfig XlsxResultsWriterConfig;

        public RunConfig(IReadOnlyList<ExecutionTask> tasks, XlsxResultsWriterConfig xlsxResultsWriterConfig)
        {
            Tasks = tasks;
            XlsxResultsWriterConfig = xlsxResultsWriterConfig;
        }
    }
}
