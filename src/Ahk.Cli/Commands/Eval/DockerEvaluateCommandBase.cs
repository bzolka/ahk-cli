using System;
using System.Collections.Generic;
using Ahk.TaskRunner;
using CliFx.Attributes;
using Microsoft.Extensions.Logging;

namespace Ahk.Commands.Eval
{
    public abstract class DockerEvaluateCommandBase : EvaluateCommandBase
    {
        [CommandOption("image", Description = "The name of the Docker image to run", IsRequired = true)]
        public string ImageName { get; set; } = string.Empty;

        [CommandOption("mount-path", Description = "The path within the container to mount the submission folder to")]
        public string SubmissionDirInContainer { get; set; } = "/submission";

        [CommandOption("artifact-path", Description = "The path within the container to fetch artifacts from (e.g., output files, generated content, etc.); defaults to none (nothing is fetched)")]
        public string? ArtifactDirInContainer { get; set; }

        [CommandOption("timeout", Description = "Maximum time frame for a container to finish evaluation; the container is terminated if exceeded.")]
        public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(3);

        [CommandOption("container-arg", Description = "Key-value pairs to pass to Docker when creating the container (multiple values are allowed)")]
        public IReadOnlyCollection<string>? ContainerParams { get; set; }

        public DockerEvaluateCommandBase(ILogger logger)
            : base(logger)
        {
        }

        protected override ITaskRunner CreateRunner(string submissionSource, string studentId, string artifactPath)
            => new DockerRunner(new DockerRunnerTask(submissionSource, studentId, Timeout, ImageName, submissionSource, SubmissionDirInContainer, artifactPath, ArtifactDirInContainer, ContainerParams), logger);
    }
}
