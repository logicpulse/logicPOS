using logicpos.financial.library.Classes.Finance;

namespace logicpos.financial.library.App
{
    /// <summary>
    /// This force not using class confilt namespace when using GlobalFramework
    /// </summary>
    public static class FinancialLibraryFramework 
    {
        internal static IStockManagementModule StockManagementModule { get; set; }
    }
}
