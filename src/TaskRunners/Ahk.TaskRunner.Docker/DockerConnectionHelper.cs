using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Docker.DotNet;

namespace Ahk.TaskRunner
{
    public class DockerConnectionHelper
    {
        public static async Task<bool> CheckConnection()
        {
            try
            {
                using var docker = GetConnectionConfiguration().CreateClient();
                await docker.System.PingAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal static Uri LocalDockerUri()
        {
            // from https://github.com/Microsoft/Docker.DotNet/commit/21832ee6b822671f9ca5ab4ef056e4b37a5f1e3d
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            return isWindows ? new Uri("npipe://./pipe/docker_engine") : new Uri("unix:/var/run/docker.sock");
        }

        internal static DockerClientConfiguration GetConnectionConfiguration() => new DockerClientConfiguration(LocalDockerUri());
    }
}
