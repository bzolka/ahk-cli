using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Ahk.TaskRunner
{
    internal static class ImagePuller
    {
        public static async Task EnsureImageExists(Docker.DotNet.DockerClient docker, ContainerConfig config, ILogger logger)
        {
            try
            {
                var findImageResult = await docker.Images.ListImagesAsync(new Docker.DotNet.Models.ImagesListParameters()
                {
                    Filters = new Dictionary<string, IDictionary<string, bool>>()
                    {
                        ["reference"] = new Dictionary<string, bool>()
                        {
                            [config.ImageName] = true
                        }
                    },
                    All = false
                });

                if (findImageResult.Any())
                {
                    logger.LogTrace("Found image {ImageName} with Docker ID {ImageId}", config.ImageName, findImageResult.First().ID);
                    return;
                }

                logger.LogWarning("Pulling image {ImageName}", config.ImageName);
                await docker.Images.CreateImageAsync(new Docker.DotNet.Models.ImagesCreateParameters() { FromImage = config.ImageName }, null, new Progress<Docker.DotNet.Models.JSONMessage>());
                logger.LogTrace("Pulling image {ImageName} completed", config.ImageName);
            }
            catch (Exception ex)
            {
                throw new Exception("Pulling image failed", ex);
            }
        }
    }
}
