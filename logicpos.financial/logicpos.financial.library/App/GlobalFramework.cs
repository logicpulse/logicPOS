using logicpos.financial.library.Classes.Finance;
using logicpos.financial.library.Classes.Stocks;

namespace logicpos.financial.library.App
{
    /// <summary>
    /// This force not using class confilt namespace when using GlobalFramework
    /// </summary>
    public class GlobalFramework : logicpos.shared.App.GlobalFramework
    {
        //Modules
        private static IStockManagementModule stockManagementModule;

        internal static IStockManagementModule StockManagementModule { get => stockManagementModule; set => stockManagementModule = value; }
    }
}
