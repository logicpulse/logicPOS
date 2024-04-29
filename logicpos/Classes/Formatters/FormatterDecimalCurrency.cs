using logicpos.shared.App;
using System;
using System.Globalization;

namespace logicpos.Classes.Formatters
{
    // Used In TreeView Columns

    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //Formaters
    public class FormatterDecimalCurrency : IFormatProvider, ICustomFormatter
    {
        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // always use dot separator for doubles
        private readonly CultureInfo culture = SharedFramework.CurrentCulture;

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            string result = string.Empty; ;

            try
            {
                //Require to Convert Exponential from string to decimal currency
                result = SharedUtils.DecimalToStringCurrency(Convert.ToDecimal(double.Parse(arg.ToString())));
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
