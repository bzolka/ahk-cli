using System;
using System.Collections.Generic;

namespace Ahk.TaskRunner
{
    public class DockerRunnerTask : RunnerTask
    {
        public readonly string ImageName;
        public readonly string SolutionDirectoryInMachine;
        public readonly string SolutionDirectoryInContainer;
        public readonly string ResultPathInMachine;
        public readonly string ResultPathInContainer;
        public readonly IReadOnlyDictionary<string, string> ContainerParams;

        public DockerRunnerTask(
                    Guid taskId,
                    string imageName,
                    string solutionDirrectoryInMachine, string solutionDirectoryInContainer,
                    string resultPathInMachine, string resultPathInContainer,
                    TimeSpan evaluationTimeout,
                    IReadOnlyDictionary<string, string> containerParams = null)
            : base(taskId, evaluationTimeout)
        {
            this.ImageName = imageName;
            this.SolutionDirectoryInMachine = solutionDirrectoryInMachine;
            this.SolutionDirectoryInContainer = solutionDirectoryInContainer;
            this.ResultPathInMachine = resultPathInMachine;
            this.ResultPathInContainer = resultPathInContainer;
            this.ContainerParams = containerParams ?? new Dictionary<string, string>();
        }

        public bool HasResultDirectory => !string.IsNullOrEmpty(ResultPathInMachine);
        public bool ShouldFetchResult => !string.IsNullOrEmpty(ResultPathInContainer) && HasResultDirectory;
    }
}
