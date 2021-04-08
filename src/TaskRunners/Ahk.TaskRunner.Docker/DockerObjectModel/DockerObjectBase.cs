using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Ahk.TaskRunner
{
    internal abstract class DockerObjectBase : IAsyncDisposable
    {
        public const string ContainerLabel = "Ahk";

        protected readonly ILogger logger;
        protected readonly Docker.DotNet.DockerClient docker;

        public DockerObjectBase(string id, ILogger logger, Docker.DotNet.DockerClient docker)
        {
            this.Id = id;
            this.logger = logger;
            this.docker = docker;
        }

        public string Id { get; }
        protected abstract string ResourceTypeName { get; }

        public static IDictionary<string, string> LabelsForCreation => new Dictionary<string, string>() { { ContainerLabel, "" } };
        public static IDictionary<string, IDictionary<string, bool>> LabelsForFilter => new Dictionary<string, IDictionary<string, bool>>() { { "label", new Dictionary<string, bool>() { { ContainerLabel, true } } } };

        public async ValueTask DisposeAsync()
        {
            try
            {
                await removeObject();
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, $"Cleanup of {ResourceTypeName} {Id} failed");
            }
        }

        protected abstract Task removeObject();

    }
}
