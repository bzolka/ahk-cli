using System;
using System.Collections.Generic;

namespace Ahk.TaskRunner
{
    public class DockerRunnerTask : RunnerTask
    {
        public DockerRunnerTask(
                    string submissionSource, string studentId, TimeSpan evaluationTimeout,
                    string imageName,
                    string submissionDirectoryInMachine, string submissionDirectoryInContainer,
                    string artifactPathInMachine, string? artifactPathInContainer,
                    IReadOnlyCollection<string>? containerEnvVariables = null,
                    IReadOnlyCollection<string>? containerParams = null)
            : base(submissionSource, studentId, evaluationTimeout)
        {
            this.ImageName = imageName;
            this.SubmissionDirectoryInMachine = submissionDirectoryInMachine;
            this.SubmissionDirectoryInContainer = submissionDirectoryInContainer;
            this.ArtifactPathInMachine = artifactPathInMachine;
            this.ArtifactPathInContainer = artifactPathInContainer;
            this.ContainerParams = containerParams ?? Array.Empty<string>();
            this.ContainerEnvVariables = containerEnvVariables ?? Array.Empty<string>();
        }

        public string ImageName { get; }
        public string SubmissionDirectoryInMachine { get; }
        public string SubmissionDirectoryInContainer { get; }
        public string ArtifactPathInMachine { get; }
        public string? ArtifactPathInContainer { get; }
        public IReadOnlyCollection<string> ContainerEnvVariables { get; }
        public IReadOnlyCollection<string> ContainerParams { get; }

        public bool HasArtifactDirectory => !string.IsNullOrEmpty(ArtifactPathInMachine) && !string.IsNullOrEmpty(ArtifactPathInContainer);
    }
}
