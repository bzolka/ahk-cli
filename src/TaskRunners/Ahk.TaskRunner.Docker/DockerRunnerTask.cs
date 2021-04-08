using System;

namespace Ahk.TaskRunner
{
    public class DockerRunnerTask : RunnerTask
    {
        public DockerRunnerTask(
                    string submissionSource, string studentId, TimeSpan evaluationTimeout,
                    ContainerConfig container,
                    string submissionDirectoryInMachine, string submissionDirectoryInContainer,
                    string artifactPathInMachine, string? artifactPathInContainer,
                    ContainerConfig? serviceContainer = null)
            : base(submissionSource, studentId, evaluationTimeout)
        {
            this.Container = container;
            this.SubmissionDirectoryInMachine = submissionDirectoryInMachine;
            this.SubmissionDirectoryInContainer = submissionDirectoryInContainer;
            this.ArtifactPathInMachine = artifactPathInMachine;
            this.ArtifactPathInContainer = artifactPathInContainer;
            this.ServiceContainer = serviceContainer;
        }

        public ContainerConfig Container { get; }
        public string SubmissionDirectoryInMachine { get; }
        public string SubmissionDirectoryInContainer { get; }
        public string ArtifactPathInMachine { get; }
        public string? ArtifactPathInContainer { get; }
        public ContainerConfig? ServiceContainer { get; }

        public bool HasArtifactDirectory => !string.IsNullOrEmpty(ArtifactPathInMachine) && !string.IsNullOrEmpty(ArtifactPathInContainer);
    }
}
