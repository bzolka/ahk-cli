using System;

namespace AHK.TaskRunner
{
    public class DockerRunnerTask : RunnerTask
    {
        public readonly string ImageName;
        public readonly string SolutionDirectoryInMachine;
        public readonly string SolutionDirectoryInContainer;
        public readonly string ResultPathInMachine;
        public readonly string ResultPathInContainer;

        public DockerRunnerTask(
                    Guid taskId,
                    string imageName,
                    string solutionDirrectoryInMachine, string solutionDirectoryInContainer,
                    string resultPathInMachine, string resultPathInContainer,
                    TimeSpan evaluationTimeout)
            : base(taskId, evaluationTimeout)
        {
            this.ImageName = imageName;
            this.SolutionDirectoryInMachine = solutionDirrectoryInMachine;
            this.SolutionDirectoryInContainer = solutionDirectoryInContainer;
            this.ResultPathInMachine = resultPathInMachine;
            this.ResultPathInContainer = resultPathInContainer;
        }

        public bool HasResultDirectory => !string.IsNullOrEmpty(ResultPathInMachine);
        public bool ShouldFetchResult => !string.IsNullOrEmpty(ResultPathInContainer) && HasResultDirectory;
    }
}
