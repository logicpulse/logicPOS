using LogicPOS.Utility;
using System;

namespace logicpos.Classes.Formatters
{
    public class StockMovementFormatter : IFormatProvider, ICustomFormatter
    {
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            decimal quantity = Convert.ToDecimal(arg);
            return (quantity > 0) ? GeneralUtils.GetResourceByName("global_stock_movement_in") : GeneralUtils.GetResourceByName("global_stock_movement_out");
        }

        public object GetFormat(Type formatType)
        {
            return (formatType == typeof(ICustomFormatter)) ? this : null;
        }
    }
}
