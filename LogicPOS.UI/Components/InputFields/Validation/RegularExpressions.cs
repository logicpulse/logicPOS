using System.Globalization;

namespace LogicPOS.UI.Components.InputFields.Validation
{
    public class RegularExpressions
    {
        private static string DecimalSeparator => CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        public static string Money => $@"^\d+(\{DecimalSeparator}\d{{1,2}})?$";
        public static string DecimalNumber => $@"^\d+(\{DecimalSeparator}\d+)?$";
        public static string Quantity => @"^-?\d+(\" + DecimalSeparator + @"\d+)?$";
        public static string PositiveQuantity => $@"^(0*[1-9]\d*(\{DecimalSeparator}\d+)?|0*\{DecimalSeparator}\d*[1-9]\d*)$";
        public static string IntegerNumber => @"^\d+$";
        public static string Alphanumeric => @"^[a-zA-Z0-9]+$";
    }
}
