using System;

namespace AHK.Evaluation
{
    internal static class DateTimeHelper
    {
        /// <summary>
        /// Gets date like 2018-10-08T10-02-45
        /// </summary>
        public static string ToPathCompatibleString(this DateTime dt) => dt.ToString("s").Replace(':', '-');
    }
}
