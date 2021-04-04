using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Ahk.Execution.Evaluation
{
    public class TrxEvaluationTask : EvaluationTask
    {
        private readonly string TrxFileName;

        public TrxEvaluationTask(string trxFileName)
        {
            this.TrxFileName = trxFileName ?? throw new System.ArgumentNullException(nameof(trxFileName));
        }

        public async override Task<Grader.GraderResult> ExecuteGrader(ExecutionTask task, TaskRunner.RunnerResult runnerResult, ILogger logger)
        {
            using (var grader = new Grader.TrxGrader(createTrxTask(task), logger))
                return await grader.GradeResult();
        }

        private Grader.TrxGraderTask createTrxTask(ExecutionTask task)
            => new Grader.TrxGraderTask(task.TaskId, task.StudentName, task.StudentNeptun, System.IO.Path.Combine(task.ResultArtifactPath, this.TrxFileName));
    }
}
