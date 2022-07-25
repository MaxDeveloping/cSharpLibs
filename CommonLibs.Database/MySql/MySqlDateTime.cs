using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibs.Database.MySql
{
    public static class MySqlDateTime
    {
        public static DateTime MinValue { get; } = new DateTime(1000, 01, 01, 0, 0, 0);

        public static DateTime MaxValue { get; } = new DateTime(9999, 12, 31, 23, 59, 59);

        public static DateTime EnsureValidValue(DateTime pDateTime)
        {
            if (pDateTime < MinValue)
                return MinValue;

            return pDateTime > MaxValue ? MaxValue : pDateTime;
        }
    }
}
