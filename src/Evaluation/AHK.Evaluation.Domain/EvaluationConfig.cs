using System;

namespace AHK.Evaluation
{
    public class EvaluationConfig
    {
        public string ImageName { get; set; }
        public string SolutionInContainer { get; set; }
        public string ResultInContainer { get; set; }

        public TimeSpan EvaluationTimeout { get; set; }

        public string TrxFileName { get; set; }
        public string ResultFileName { get; set; }
    }
}
