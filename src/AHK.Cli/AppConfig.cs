using System;

namespace AHK
{
    class AppConfig
    {
        public string AssignmentsDir { get; set; }
        public string ResultsDir { get; set; }
        public TimeSpan MaxTaskRuntime { get; set; } = TimeSpan.FromMinutes(2);
    }
}
