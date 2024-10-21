using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Tables.GetAllTables;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.Shared.Orders;
using LogicPOS.Shared;
using LogicPOS.UI.Alerts;
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
        public static List<PosOrder> Orders { get; private set; } = new List<PosOrder>();
        public static POSMainWindow POSWindow { get; private set; }
        public static void SetCurrentTable(Table table)
        {
            CurrentTable = table;
            UpdatePOSLabels();

            if (ItemsPage == null)
            {
                return;
            }

            ItemsPage.Clear(true);
            ItemsPage.Order = GetCurrentOrder();
            ItemsPage.PresentOrderItems();
        }

        public static void UpdatePOSLabels()
        {
            string tableDenomination = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, string.Format("global_table_appmode_{0}", AppOperationModeSettings.CustomAppOperationMode.AppOperationTheme).ToLower());

            POSWindow.LabelCurrentTable.Text = $"{tableDenomination} {CurrentTable.Designation}";
        }

        public static PosOrder GetCurrentOrder()
        {
            var currentOrder  = Orders.FirstOrDefault(o => o.Table.Id == CurrentTable.Id);

            if (currentOrder == null)
            {
                currentOrder = new PosOrder(CurrentTable);
                Orders.Add(currentOrder);
            }

            return currentOrder;
        }

        public static void Initialize(POSMainWindow posWindow)
        {
            POSWindow = posWindow;
            SetCurrentTable(GetDefaultTable());
            Orders.Add(new PosOrder(CurrentTable));

        }

        private static Table GetDefaultTable()
        {
            var tables = DependencyInjection.Services.GetRequiredService<ISender>().Send(new GetAllTablesQuery()).Result;

            if (tables.IsError)
            {
                SimpleAlerts.ShowApiErrorAlert(GlobalApp.PosMainWindow, tables.FirstError);
                Gtk.Application.Quit();
            }

            return tables.Value.First();
        }
    }
}
