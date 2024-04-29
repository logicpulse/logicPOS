using logicpos.shared.App;
using System;

namespace logicpos.Classes.Formatters
{
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

    public class FormatterDate : IFormatProvider, ICustomFormatter
    {
        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            string result = string.Empty; 

            try
            {
                result = SharedUtils.DateToString(arg).ToString();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return result;
        }

        public object GetFormat(Type formatType)
        {
            return (formatType == typeof(ICustomFormatter)) ? this : null;
        }
    }
}
