using logicpos.financial.library.Classes.Finance;
using logicpos.financial.library.Classes.Stocks;

namespace logicpos.financial.library.App
{
    /// <summary>
    /// This force not using class confilt namespace when using GlobalFramework
    /// </summary>
    public static class FinancialLibraryFramework 
    {
        //Modules
        private static IStockManagementModule stockManagementModule;

        internal static IStockManagementModule StockManagementModule { get => stockManagementModule; set => stockManagementModule = value; }
    }
}
