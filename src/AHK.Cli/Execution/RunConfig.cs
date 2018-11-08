using System.Collections.Generic;

namespace AHK.Execution
{
    public class RunConfig
    {
        public readonly IReadOnlyList<ExecutionTask> Tasks;
        public readonly string ResultsXlsxFileName;

        public RunConfig(IReadOnlyList<ExecutionTask> tasks, string resultsXlsxFileName)
        {
            this.Tasks = tasks;
            this.ResultsXlsxFileName = resultsXlsxFileName;
        }
    }
}
