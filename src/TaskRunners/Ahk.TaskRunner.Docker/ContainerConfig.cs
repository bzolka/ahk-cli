using System;
using System.Collections.Generic;
namespace Ahk.TaskRunner
{

    public class ContainerConfig
    {
        public ContainerConfig(ImageName imageName, IReadOnlyCollection<string>? envVariables = null, IReadOnlyCollection<string>? createParams = null, string? name = null)
        {
            this.ImageName = imageName;
            this.EnvVariables = envVariables ?? Array.Empty<string>();
            this.CreateParams = createParams ?? Array.Empty<string>();
            this.Name = name;
        }

        public ImageName ImageName { get; }
        public IReadOnlyCollection<string> EnvVariables { get; }
        public IReadOnlyCollection<string> CreateParams { get; }
        public string? Name { get; }

    }
}
