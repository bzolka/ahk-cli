using System;
using System.Collections.Generic;

namespace AHK.Execution
{
    public class ExecutionTask
    {
        public readonly Guid TaskId = Guid.NewGuid();

        public readonly string StudentName;
        public readonly string StudentNeptun;
        public readonly string SolutionPath;
        public readonly string ResultArtifactPath;
        public readonly Evaluation.EvaluationTask EvaluationTask;

        protected ExecutionTask(string studentName, string studentNeptun, string solutionPath, string resultArtifactPath,
            Evaluation.EvaluationTask evaluationTask)
        {
            this.StudentName = studentName;
            this.StudentNeptun = studentNeptun;
            this.SolutionPath = solutionPath;
            this.ResultArtifactPath = resultArtifactPath;
            this.EvaluationTask = evaluationTask;
        }
    }

    public class DockerExecutionTask: ExecutionTask
    {
        public readonly string DockerImageName;
        public readonly string DockerSolutionDirectoryInContainer;
        public readonly string DockerResultPathInContainer;
        public readonly TimeSpan DockerTimeout;
        public readonly IReadOnlyDictionary<string, string> DockerImageParams;

        public DockerExecutionTask(string studentName, string studentNeptun, string solutionPath, string resultArtifactPath,
            string dockerImageName, string dockerSolutionDirectoryInContainer, string dockerResultPathInContainer, TimeSpan dockerTimeout, IReadOnlyDictionary<string, string> dockerImageParams,
            Evaluation.EvaluationTask evaluationTask):
            base (studentName, studentNeptun, solutionPath, resultArtifactPath, evaluationTask)
        {
            this.DockerImageName = dockerImageName;
            this.DockerSolutionDirectoryInContainer = dockerSolutionDirectoryInContainer;
            this.DockerResultPathInContainer = dockerResultPathInContainer;
            this.DockerTimeout = dockerTimeout;
            this.DockerImageParams = dockerImageParams;
        }
    }

    public class LocalCmdExecutionTask : ExecutionTask
    {
        public readonly string Command;
        public readonly TimeSpan CommandTimeout;
        public readonly Dictionary<string, string> EnvironmentVariables;

        public LocalCmdExecutionTask(string studentName, string studentNeptun, string solutionPath, string resultArtifactPath,
            string command, Dictionary<string, string> environmentVariables, TimeSpan cmdTimeout,
            Evaluation.EvaluationTask evaluationTask) :
            base(studentName, studentNeptun, solutionPath, resultArtifactPath, evaluationTask)
        {
            this.Command = command;
            this.EnvironmentVariables = environmentVariables;
            this.CommandTimeout = cmdTimeout;
        }
    }


}
