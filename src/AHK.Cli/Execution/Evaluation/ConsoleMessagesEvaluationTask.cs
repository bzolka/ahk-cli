﻿using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AHK.Execution.Evaluation
{
    public class ConsoleMessagesEvaluationTask : EvaluationTask
    {
        private readonly string ValidationCode;

        public ConsoleMessagesEvaluationTask(string validationCode)
        {
            this.ValidationCode = validationCode ?? throw new System.ArgumentNullException(nameof(validationCode));
        }

        public async override Task<Grader.GraderResult> ExecuteGrader(ExecutionTask task, TaskRunner.RunnerResult runnerResult, ILogger logger)
        {
            using (var grader = new Grader.ConsoleMessagesGrader(createTask(task, runnerResult.ConsoleOutput), logger))
                return await grader.GradeResult();
        }

        private Grader.ConsoleMessagesGraderTask createTask(ExecutionTask task, string consoleLog)
            => new Grader.ConsoleMessagesGraderTask(task.TaskId, task.StudentId, consoleLog, this.ValidationCode);
    }
}
