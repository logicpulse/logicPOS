using System.Globalization;
using System;
using static LogicPOS.Settings.CultureSettings;

namespace LogicPOS.Utility
{
    public static class DataConversionUtils
    {
        public static decimal StringToDecimal(string input)
        {
            decimal result;

            NumberStyles numberStyle = NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent;

            if (!decimal.TryParse(input, numberStyle, CurrentCulture.NumberFormat, out result))
            {
                decimal.TryParse(input, numberStyle, CultureInfo.InvariantCulture.NumberFormat, out result);
            }

            return result;
        }

        public static string DecimalToString(decimal input)
        {
            return input.ToString(DecimalFormat, CurrentCultureNumberFormat);
        }

        public static string DecimalToString(decimal input, string decimalFormat)
        {
            return input.ToString(decimalFormat, CurrentCulture.NumberFormat);
        }

        public static string DecimalToStringCurrency(decimal input, string acronym)
        {
            return $"{DecimalToString(input)}{acronym}";
        }

        public static string DoubleToString(double input)
        {
            return input.ToString(DecimalFormat, CurrentCulture.NumberFormat);
        }

        public static string StringToDecimalAndToStringAgain(
            string input)
        {
            string result = DecimalToString(StringToDecimal(input));

            if (result[0] == Convert.ToChar(CurrentCulture.NumberFormat.NumberDecimalSeparator))
            {
                result = $"{0}{result}";
            }

            return result;
        }

        public static string FormatDataTableFieldFromType(
            string fieldValue,
            string fieldType)
        {
            string resultFieldValue;
            switch (fieldType)
            {
                case "Decimal":
                case "Int64":
                    decimal valueDecimal;
                    decimal.TryParse(fieldValue, out valueDecimal);

                    resultFieldValue = DecimalToString(valueDecimal);
                    break;
                case "Double":
                    double valueDouble;
                    double.TryParse(fieldValue, out valueDouble);
                    resultFieldValue = DoubleToString(valueDouble);
                    break;
                default:
                    resultFieldValue = fieldValue;
                    break;
            }
            return resultFieldValue;
        }
    }
}
