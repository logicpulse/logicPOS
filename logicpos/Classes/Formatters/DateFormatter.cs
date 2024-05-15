using logicpos.datalayer.Xpo;
using System;

namespace logicpos.Classes.Formatters
{
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

    public class DateFormatter : IFormatProvider, ICustomFormatter
    {

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            return XPOHelper.DateToString(arg).ToString();
        }

        public object GetFormat(Type formatType)
        {
            return (formatType == typeof(ICustomFormatter)) ? this : null;
        }
    }
}
