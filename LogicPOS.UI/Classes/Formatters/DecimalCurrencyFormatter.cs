using logicpos.datalayer.Xpo;
using LogicPOS.Data.XPO.Settings;
using System;

namespace logicpos.Classes.Formatters
{
    public class DecimalCurrencyFormatter : IFormatProvider, ICustomFormatter
    {
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            return LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(Convert.ToDecimal(double.Parse(arg.ToString())), XPOSettings.ConfigurationSystemCurrency.Acronym);
        }

        public object GetFormat(Type formatType)
        {
            return (formatType == typeof(ICustomFormatter)) ? this : null;
        }
    }
}
