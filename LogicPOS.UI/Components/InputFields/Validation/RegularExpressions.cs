using System.Globalization;

namespace LogicPOS.UI.Components.InputFields.Validation
{
    public class RegularExpressions
    {
        public static string Money => @"^\d+(\" + CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator +@"\d{1,2})?$";
        public static string DecimalNumber => @"^\d+(\" + CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator + @"\d+)?$";
        public static string IntegerNumber => @"^\d+$";
        public static string Alphanumeric => @"^[a-zA-Z0-9]+$";
    }
}
