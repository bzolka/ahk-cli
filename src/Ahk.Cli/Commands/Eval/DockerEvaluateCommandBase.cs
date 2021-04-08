using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        [CommandOption("container-env", Description = "Key-value pairs to pass to the container as environment variables (multiple values are allowed)")]
        public IReadOnlyCollection<string>? ContainerEnvVariables { get; set; }

        [CommandOption("container-arg", Description = "Key-value pairs to pass to Docker when creating the container (multiple values are allowed)")]
        public IReadOnlyCollection<string>? ContainerParams { get; set; }


        [CommandOption("service-container", Description = "The name of the Docker image to run as a service container linked to the main container")]
        public string? ServiceContainerImage { get; set; } = null;

        [CommandOption("service-container-name", Description = "Name to set for the service container (also the DNS name)")]
        public string? ServiceContainerName { get; set; } = null;

        [CommandOption("service-container-env", Description = "Key-value pairs to pass to the service container as environment variables")]
        public IReadOnlyCollection<string>? ServiceContainerEnvVariables { get; set; }


        public DockerEvaluateCommandBase(ILogger logger)
            : base(logger)
        {
        }

        protected override ITaskRunner CreateRunner(string submissionSource, string studentId, string artifactPath)
            => DockerRunner.Create(
                task: new DockerRunnerTask(
                    submissionSource, studentId, Timeout,
                    new ContainerConfig(imageName: ImageName, envVariables: ContainerEnvVariables, createParams: ContainerParams),
                    submissionSource, SubmissionDirInContainer, artifactPath, ArtifactDirInContainer,
                    string.IsNullOrEmpty(ServiceContainerImage) ? null : new ContainerConfig(imageName: ServiceContainerImage, envVariables: ServiceContainerEnvVariables, name: ServiceContainerName)),
                logger);

        protected override async Task Initialize(Spectre.Console.ProgressContext ctx)
        {
            await runInitStep(ctx, "Checking Docker connection", () => DockerConnectionHelper.CheckConnection());

            var imagesToPull = new List<string>();
            await runInitStep(ctx, "Checking Docker images",
                async () =>
                {
                    if (!await ImagePuller.CheckImageExists(ImageName))
                        imagesToPull.Add(ImageName);

                    if (!string.IsNullOrEmpty(ServiceContainerImage) && !await ImagePuller.CheckImageExists(ServiceContainerImage))
                        imagesToPull.Add(ServiceContainerImage);
                });

            foreach (var imageName in imagesToPull)
                await runInitStep(ctx, $"Pulling {imageName}", () => ImagePuller.Pull(imageName));
        }

        private async Task runInitStep(Spectre.Console.ProgressContext ctx, string name, Func<Task> action)
        {
            var progressPrepare = ctx.AddTask(name);
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                throw new CliFx.Exceptions.CommandException(ex.Message);
            }
            progressPrepare.MaxValue = progressPrepare.Value = 1;
        }
    }
}
