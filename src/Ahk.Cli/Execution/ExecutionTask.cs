using System;
using System.Collections.Generic;

namespace Ahk.Execution
{
    public class ExecutionTask
    {
        public readonly Guid TaskId = Guid.NewGuid();

        public readonly string StudentName;
        public readonly string StudentNeptun;
        public readonly string SolutionPath;
        public readonly string ResultArtifactPath;

        public readonly string DockerImageName;
        public readonly string DockerSolutionDirectoryInContainer;
        public readonly string DockerResultPathInContainer;
        public readonly TimeSpan DockerTimeout;
        public readonly IReadOnlyDictionary<string, string> DockerImageParams;

        public readonly Evaluation.EvaluationTask EvaluationTask;

        public ExecutionTask(string studentName, string studentNeptun, string solutionPath, string resultArtifactPath,
            string dockerImageName, string dockerSolutionDirectoryInContainer, string dockerResultPathInContainer, TimeSpan dockerTimeout, IReadOnlyDictionary<string, string> dockerImageParams,
            Evaluation.EvaluationTask evaluationTask)
        {
            this.StudentName = studentName;
            this.StudentNeptun = studentNeptun;
            this.SolutionPath = solutionPath;
            this.ResultArtifactPath = resultArtifactPath;
            this.DockerImageName = dockerImageName;
            this.DockerSolutionDirectoryInContainer = dockerSolutionDirectoryInContainer;
            this.DockerResultPathInContainer = dockerResultPathInContainer;
            this.DockerTimeout = dockerTimeout;
            this.DockerImageParams = dockerImageParams;
            this.EvaluationTask = evaluationTask;
        }
    }
}
