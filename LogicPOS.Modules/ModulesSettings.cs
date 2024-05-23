using LogicPOS.Modules.StockManagement;

namespace LogicPOS.Modules
{
    public static class ModulesSettings
    {
        public static IStockManagementModule StockManagementModule { get; set; }
        public static bool HasStockManagementModule => StockManagementModule != null;
    }
}
