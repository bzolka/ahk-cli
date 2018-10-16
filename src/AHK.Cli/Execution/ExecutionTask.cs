using System;

namespace AHK.Execution
{
    public class ExecutionTask
    {
        public readonly Guid TaskId = Guid.NewGuid();

        public readonly string StudentId;
        public readonly string SolutionPath;
        public readonly string ResultArtifactPath;

        public readonly string DockerImageName;
        public readonly string DockerSolutionDirectoryInContainer;
        public readonly string DockerResultPathInContainer;
        public readonly TimeSpan DockerTimeout;

        public readonly string TrxFileName;
        public readonly string TrxOutputFile;

        public ExecutionTask(string studentId, string solutionPath, string resultArtifactPath,
            string dockerImageName, string dockerSolutionDirectoryInContainer, string dockerResultPathInContainer, TimeSpan dockerTimeout,
            string trxFileName, string trxOutputFile)
        {
            this.StudentId = studentId;
            this.SolutionPath = solutionPath;
            this.ResultArtifactPath = resultArtifactPath;
            this.DockerImageName = dockerImageName;
            this.DockerSolutionDirectoryInContainer = dockerSolutionDirectoryInContainer;
            this.DockerResultPathInContainer = dockerResultPathInContainer;
            this.DockerTimeout = dockerTimeout;
            this.TrxFileName = trxFileName;
            this.TrxOutputFile = trxOutputFile;
        }
    }
}
