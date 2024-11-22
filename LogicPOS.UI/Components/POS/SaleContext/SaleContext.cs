using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Tables.GetAllTables;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
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
            UpdatePOSLabels();
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
            UpdatePOSLabels();
        }

        public static void UpdatePOSLabels()
        {
            if (CurrentTable != null)
            {
                string tableDenomination = LocalizedString.Instance[string.Format("global_table_appmode_{0}", AppOperationModeSettings.CustomAppOperationMode.AppOperationTheme).ToLower()];
                POSWindow.Instance.LabelCurrentTable.Text = $"{tableDenomination} {CurrentTable.Designation}";
            }

            if (CurrentOrder != null)
            {
                POSWindow.Instance.LabelTotalTable.Text = POSWindow.Instance.LabelTotalTable.Text = $"{CurrentOrder.TotalFinal:0.00} : #{CurrentOrder.Tickets.Count}";
            }
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
            return TablesService.GetFreeTables().FirstOrDefault();
        }
    }
}
