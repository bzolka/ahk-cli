using System;

namespace AHK
{
    class AppConfig
    {
        public TimeSpan MaxTaskRuntime { get; set; } = TimeSpan.FromMinutes(2);
    }
}
