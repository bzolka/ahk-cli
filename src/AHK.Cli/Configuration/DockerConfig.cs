using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace AHK.Configuration
{
    public class DockerConfig
    {
        public string ImageName { get; set; }
        public string SolutionInContainer { get; set; }
        public string ResultInContainer { get; set; }
        public TimeSpan EvaluationTimeout { get; set; } = TimeSpan.FromMinutes(3);
        public Dictionary<string, string> ContainerParams { get; set; }

        public bool Validate(ILogger logger)
        {
            if (string.IsNullOrEmpty(ImageName))
            {
                logger.LogError("Docker image name not specified");
                return false;
            }

            if (string.IsNullOrEmpty(SolutionInContainer))
            {
                logger.LogError("Docker solution mount directory not specified");
                return false;
            }

            return true;
        }
    }
}
