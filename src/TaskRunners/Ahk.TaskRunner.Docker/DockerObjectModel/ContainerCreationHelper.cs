using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Ahk.TaskRunner
{
    internal static class ContainerCreationHelper
    {
        public static async Task<DockerContainer> CreateNewContainer(ContainerConfig config, Docker.DotNet.DockerClient docker, ILogger logger, CancellationToken cancellationToken, Action<Docker.DotNet.Models.CreateContainerParameters>? configure = null)
        {
            try
            {
                var createContainerParams = new Docker.DotNet.Models.CreateContainerParameters()
                {
                    Image = config.ImageName,
                    Labels = DockerObjectBase.LabelsForCreation,
                    HostConfig = new Docker.DotNet.Models.HostConfig()
                    {
                        Mounts = new List<Docker.DotNet.Models.Mount>() { }
                    },
                    Env = config.EnvVariables.ToList()
                };

                if (config.CreateParams != null && config.CreateParams.Any())
                    setImageParameters(createContainerParams, config.CreateParams, logger);

                configure?.Invoke(createContainerParams);

                var createContainerResponse = await docker.Containers.CreateContainerAsync(createContainerParams, cancellationToken);

                if (string.IsNullOrEmpty(createContainerResponse.ID))
                    throw new Exception("Container create failed with unknown error");

                logger.LogTrace($"Container created with ID {createContainerResponse.ID}");

                return new DockerContainer(createContainerResponse.ID, logger, docker);
            }
            catch (Docker.DotNet.DockerImageNotFoundException)
            {
                throw new Exception($"Image {config.ImageName} not found.");
            }
            catch (Docker.DotNet.DockerApiException ex)
            {
                throw new Exception($"Container create failed with error: {ex.Message}", ex);
            }
        }

        private static void setImageParameters(Docker.DotNet.Models.CreateContainerParameters createContainerParams, IReadOnlyCollection<string> containerParams, ILogger logger)
        {
            foreach (var param in containerParams)
            {
                var eqIdx = param.IndexOf('=');
                if (eqIdx == -1)
                {
                    logger.LogWarning($"Container param {param} invalid");
                    continue;
                }

                try
                {
                    var key = param.Substring(0, eqIdx);
                    var value = param.Substring(eqIdx + 1);

                    if (trySetDynamicPropertyOn(createContainerParams, key, value))
                        continue;

                    if (trySetDynamicPropertyOn(createContainerParams.HostConfig, key, value))
                        continue;

                    logger.LogWarning($"Unknown container parameter {key}");
                }
                catch
                {
                    logger.LogWarning($"Cannot set container parameter {param}");
                }
            }
        }

        private static bool trySetDynamicPropertyOn(object objectToSetOn, string propertyName, object propertyValue)
        {
            if (objectToSetOn == null)
                return false;

            if (string.IsNullOrEmpty(propertyName))
                return false;

            if (char.IsLower(propertyName[0]))
                propertyName = char.ToUpper(propertyName[0]).ToString() + propertyName.Substring(1);

            var ccpType = objectToSetOn.GetType();
            var prop = ccpType.GetProperty(propertyName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if (prop != null)
            {
                var valueConverted = Convert.ChangeType(propertyValue, prop.PropertyType);
                prop.SetValue(objectToSetOn, valueConverted);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
