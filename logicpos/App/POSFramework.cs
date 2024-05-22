using logicpos.financial.library.Classes.Stocks;

namespace logicpos.App
{
    /// <summary>
    /// This force not using class confilt namespace when using GlobalFramework
    /// </summary>
    public static class POSFramework 
    {
        internal static IStockManagementModule StockManagementModule { get; set; }
    }
}
