using System;
using System.Text.RegularExpressions;

namespace GoDaddyDDNS.Utils
{
    internal static class TimeSpanUtils
    {
        public static TimeSpan ParseTimeInterval(string timeInterval)
        {
            if (string.IsNullOrEmpty(timeInterval))
                throw new ArgumentException("Time interval cannot be null or empty.");

            var formatRegex = new Regex(@"^(\d+)([smhd])$", RegexOptions.IgnoreCase);
            var match = formatRegex.Match(timeInterval);

            if (!match.Success)
                throw new ArgumentException("Invalid time interval format. Use format like '30s', '10m', '1h', or '1d'.");

            var value = int.Parse(timeInterval.Substring(0, timeInterval.Length - 1));
            var unit = timeInterval.Substring(timeInterval.Length - 1).ToLower();

            switch (unit)
            {
                case "s": // seconds
                    return TimeSpan.FromSeconds(value);
                case "m": // minutes
                    return TimeSpan.FromMinutes(value);
                case "h": // hours
                    return TimeSpan.FromHours(value);
                case "d": // days
                    return TimeSpan.FromDays(value);
                default:
                    throw new ArgumentException("Invalid time interval unit.");
            }
        }
    }
}
