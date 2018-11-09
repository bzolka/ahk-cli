using System;
using System.Collections.Generic;

namespace AHK.Configuration
{
    public class DockerConfig
    {
        public string ImageName { get; set; }
        public string SolutionInContainer { get; set; } = "/megoldas";
        public string ResultInContainer { get; set; }
        public TimeSpan EvaluationTimeout { get; set; } = TimeSpan.FromMinutes(3);
        public Dictionary<string, string> ContainerParams { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(ImageName))
                throw new Exception("Docker image nev hianyzik");
        }
    }
}
