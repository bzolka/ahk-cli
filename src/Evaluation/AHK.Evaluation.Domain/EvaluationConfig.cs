using System;

namespace AHK.Evaluation
{
    public class EvaluationConfig
    {
        public string ImageName { get; set; }
        public string SolutionDirectoryInContainer { get; set; }
        public string ResultInContainer { get; set; }
        public TimeSpan EvaluationTimeout { get; set; }
    }
}
