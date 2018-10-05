using System;

namespace AHK.TaskRunner
{
    public class RunnerTask
    {
        public readonly string ImageName;
        public readonly string SolutionDirectoryInMachine;
        public readonly string SolutionDirectoryInContainer;
        public readonly TimeSpan EvaluationTimeout;
        public readonly string ResultPathInContainer;
        public readonly string ResultPathInMachine;

        public RunnerTask(string imageName,
                    string solutionDirrectoryInMachine, string solutionDirectoryInContainer,
                    string resultPathInContainer, string resultPathInMachine,
                    TimeSpan evaluationTimeout)
        {
            this.ImageName = imageName;
            this.SolutionDirectoryInMachine = solutionDirrectoryInMachine;
            this.SolutionDirectoryInContainer = solutionDirectoryInContainer;
            this.ResultPathInContainer = resultPathInContainer;
            this.ResultPathInMachine = resultPathInMachine;
            this.EvaluationTimeout = evaluationTimeout;
        }

        public bool HasResultDirectory => !string.IsNullOrEmpty(ResultPathInMachine);
        public bool ShouldFetchResult => !string.IsNullOrEmpty(ResultPathInContainer) && HasResultDirectory;
    }
}
