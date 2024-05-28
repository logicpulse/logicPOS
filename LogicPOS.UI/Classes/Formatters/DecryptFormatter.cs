using System;

namespace logicpos.Classes.Formatters
{
    public class DecryptFormatter : IFormatProvider, ICustomFormatter
    {

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg != null)
            {
                return LogicPOS.Settings.PluginSettings.SoftwareVendor.Decrypt((arg).ToString());
            }

            return string.Empty;
        }

        public object GetFormat(Type formatType)
        {
            return (formatType == typeof(ICustomFormatter)) ? this : null;
        }
    }
}
