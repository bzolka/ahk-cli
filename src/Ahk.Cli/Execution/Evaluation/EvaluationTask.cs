using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Ahk.Execution.Evaluation
{
    public abstract class EvaluationTask
    {
        public abstract Task<Grader.GraderResult> ExecuteGrader(ExecutionTask task, TaskRunner.RunnerResult runnerResult, ILogger logger);
    }
}
