using System;
using System.Runtime.InteropServices;
using Docker.DotNet;

namespace AHK.TaskRunner
{
    internal class DockerConnectionHelper
    {
        public static Uri LocalDockerUri()
        {
            // from https://github.com/Microsoft/Docker.DotNet/commit/21832ee6b822671f9ca5ab4ef056e4b37a5f1e3d
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            return isWindows ? new Uri("npipe://./pipe/docker_engine") : new Uri("unix:/var/run/docker.sock");
        }

        public static DockerClientConfiguration GetConnectionConfiguration() => new DockerClientConfiguration(LocalDockerUri());
    }
}