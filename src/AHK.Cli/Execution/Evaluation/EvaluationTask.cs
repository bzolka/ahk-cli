using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AHK.Execution.Evaluation
{
    public abstract class EvaluationTask
    {
        public abstract Task<Grader.GraderResult> ExecuteGrader(ExecutionTask task, TaskRunner.RunnerResult runnerResult, ILogger logger);
    }
}
