using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Tables.GetAllTables;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application;
using LogicPOS.UI.Components.Windows;
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
        public static List<PosOrder> Orders { get; private set; } = new List<PosOrder>();

        public static void SetCurrentTable(Table table)
        {
            CurrentTable = table;
            CurrentOrder = Orders.FirstOrDefault(o => o.Table.Id == table.Id);
            
            if(CurrentOrder == null)
            {
                CurrentOrder = new PosOrder(table);
                Orders.Add(CurrentOrder);
            }

            UpdatePOSLabels();

            if (ItemsPage != null)
            {
                ItemsPage.Clear(true);
                ItemsPage.Order = CurrentOrder;
                ItemsPage.PresentOrderItems();
                ItemsPage.UpdateLabelTotalValue();
            }
        }

        public static void UpdatePOSLabels()
        {
            if (CurrentTable != null)
            {
                string tableDenomination = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, string.Format("global_table_appmode_{0}", AppOperationModeSettings.CustomAppOperationMode.AppOperationTheme).ToLower());
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
                SetCurrentTable(defaultTable);
            }
        }

        private static Table GetDefaultTable()
        {
            var tables = DependencyInjection.Services.GetRequiredService<ISender>().Send(new GetAllTablesQuery()).Result;

            if (tables.IsError)
            {
                CustomAlerts.ShowApiErrorAlert(POSWindow.Instance, tables.FirstError);
                Gtk.Application.Quit();
            }

            return tables.Value.First();
        }
    }
}
