using System;

namespace Ahk
{
    public static class TimeSpanHelper
    {
        public static TimeSpan Smaller(TimeSpan one, TimeSpan other)
            => one < other ? one : other;
    }
}
