using logicpos.financial.library.Classes.Stocks;

namespace logicpos.App
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
