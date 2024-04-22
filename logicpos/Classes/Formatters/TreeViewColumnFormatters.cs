using logicpos.datalayer.App;
using logicpos.financial.library.App;
using logicpos.resources.Resources.Localization;
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

    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

    public class FormatterDecimal : IFormatProvider, ICustomFormatter
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
                if (arg.ToString() != string.Empty)
                {
                    //Require to Convert Exponential from string to decimal
                    result = SharedUtils.DecimalToString(Convert.ToDecimal(double.Parse(arg.ToString())));
                }

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

    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

    public class FormatterStockMovement : IFormatProvider, ICustomFormatter
    {
        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            string result = string.Empty; ;

            try
            {
                decimal quantity = Convert.ToDecimal(arg);
                result = (quantity > 0) ? resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_stock_movement_in") : resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_stock_movement_out");
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
                    result = SharedFramework.PluginSoftwareVendor.Decrypt((arg).ToString());
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
