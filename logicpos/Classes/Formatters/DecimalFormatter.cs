using System;

namespace logicpos.Classes.Formatters
{
    public class DecimalFormatter : IFormatProvider, ICustomFormatter
    {

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg.ToString() != string.Empty)
            {

                return LogicPOS.Utility.DataConversionUtils.DecimalToString(Convert.ToDecimal(double.Parse(arg.ToString())));
            }

            return string.Empty;
        }

        public object GetFormat(Type formatType)
        {
            return (formatType == typeof(ICustomFormatter)) ? this : null;
        }
    }
}
