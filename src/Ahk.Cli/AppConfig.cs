using System;

namespace Ahk
{
    class AppConfig
    {
        public TimeSpan MaxTaskRuntime { get; set; } = TimeSpan.FromMinutes(2);
    }
}
