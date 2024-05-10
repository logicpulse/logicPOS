using System;

namespace logicpos.Classes.Formatters
{

    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

    /// <summary>
    /// This formatter is an option to "DecryptValue = true" from "GenericTreeViewColumnProperty", where the same demands a query call to get the proper attribute values
    /// </summary>
    public class FormatterDecrypt : IFormatProvider, ICustomFormatter
    {
        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            string result = string.Empty; ;

            try
            {
                if (arg != null)
                {
                    result = LogicPOS.Settings.PluginSettings.SoftwareVendor.Decrypt((arg).ToString());
                }
            }
            catch (Exception ex)
            {
                _logger.Error("string Format(string format, object arg, IFormatProvider formatProvider) :: " + ex.Message, ex);
            }
            return result;
        }

        public object GetFormat(Type formatType)
        {
            return (formatType == typeof(ICustomFormatter)) ? this : null;
        }
    }
}
