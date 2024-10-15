using System;
using System.Globalization;

namespace LogicPOS.Api.Extensions
{
    public static class DateTimeExtension
    {
        public static string ToISO8601DateTime(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ss");
        }

        public static string ToISO8601DateOnly(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        public static DateTime FromISO8601(this string datetime)
        {
            return DateTime.ParseExact(datetime, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}
