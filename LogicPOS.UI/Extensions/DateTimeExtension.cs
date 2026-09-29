using System;

namespace LogicPOS.UI.Extensions
{
    internal static class DateTimeExtension
    {
        public static string ValidateDate(this string date)
        {
            var datePaties = date.Split('-');
            var year = int.Parse(datePaties[0]);
            var month = int.Parse(datePaties[1]);
            var lastDay = DateTime.DaysInMonth(year, month);

            if (lastDay < int.Parse(datePaties[2]))
            {
                if (month > 9)
                {
                    return ($"{year}-{month}-{lastDay}");
                }
                else
                {
                    return ($"{year}-0{month}-{lastDay}");
                }

            }
            return date;
        }

    }
}
