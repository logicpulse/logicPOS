using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Services;
using System.Linq;

namespace LogicPOS.UI.Components.POS
{
    public static class SaleContext
    {
        public static SaleItemsPage ItemsPage { get; set; }
        public static Table CurrentTable { get; private set; }
        public static PosOrder CurrentOrder { get; private set; }

        public static void ReloadCurrentOrder()
        {
            if(CurrentOrder.Id == null)
            {
                return;
            }

            CurrentOrder = OrdersService.GetPosOrder(CurrentOrder.Id.Value);
            ItemsPage.Clear(true);
            ItemsPage.PresentOrderItems();
            POSWindow.Instance.UpdateUI();
        }

        public static void SetCurrentTable(Table table)
        {
            CurrentTable = table;
            CurrentOrder = OrdersService.GetOpenPosOrders(table.Id)
                                        .FirstOrDefault();

            if (CurrentOrder == null)
            {
                CurrentOrder = new PosOrder(table);
            }

            ItemsPage.Clear(true);
            ItemsPage.PresentOrderItems();
            POSWindow.Instance.UpdateUI();
        }

        public static void Initialize()
        {
            if (CurrentTable == null)
            {
                var defaultTable = GetDefaultTable();

                if (defaultTable != null)
                {
                    SetCurrentTable(defaultTable);
                }
            }
        }

        private static Table GetDefaultTable()
        {
            var freeTable = TablesService.GetFreeTables().FirstOrDefault();

            if (freeTable != null)
            {
                return freeTable;
            }

            return TablesService.GetOpenTables().FirstOrDefault();  
        }
    }
}
