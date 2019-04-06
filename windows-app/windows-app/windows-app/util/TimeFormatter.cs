using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace windows_app
{
    class TimeFormatter
    {
        public static string FormatMillis(long milliseconds)
        {
            long seconds = milliseconds / 1000;

            if (seconds < 60)
            {
                long seconds10 = (seconds / 10) * 10;

                if (seconds10 == 0) return "just started";
                return $"{seconds10} seconds";
            }

            long minutes = milliseconds / (1000 * 60);
            if (minutes == 1) return "1 minute";
            if (minutes <= 10) return $"{minutes} minutes";

            long minutes5 = (minutes / 5) * 5;
            long hours = minutes5 / 60;
            minutes5 %= 60;

            if (hours == 0) return $"{minutes5} minutes";
            if (hours == 1)
            {
                if (minutes5 == 0) return "1 hour";
                return $"1 hour and {minutes5} minutes";
            }

            if (minutes5 == 0) return $"{hours} hours";
            return $"{hours} hours and {minutes5} minutes";
        }
    }
}
