using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ahk.TaskRunner
{
    public static class ImagePuller
    {
        public static async Task<bool> CheckImageExists(string imageName)
        {
            using var docker = DockerConnectionHelper.GetConnectionConfiguration().CreateClient();

            var findImageResult = await docker.Images.ListImagesAsync(new Docker.DotNet.Models.ImagesListParameters()
            {
                Filters = new Dictionary<string, IDictionary<string, bool>>() { ["reference"] = new Dictionary<string, bool>() { [imageName] = true } },
                All = false
            });

            return findImageResult.Any();
        }

        public static async Task Pull(string imageName)
        {
            using var docker = DockerConnectionHelper.GetConnectionConfiguration().CreateClient();
            await docker.Images.CreateImageAsync(new Docker.DotNet.Models.ImagesCreateParameters() { FromImage = imageName }, null, new Progress<Docker.DotNet.Models.JSONMessage>());
        }
    }
}
