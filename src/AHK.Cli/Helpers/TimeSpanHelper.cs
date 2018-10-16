using System;

namespace AHK
{
    public static class TimeSpanHelper
    {
        public static TimeSpan Smaller(TimeSpan one, TimeSpan other)
            => one < other ? one : other;
    }
}
