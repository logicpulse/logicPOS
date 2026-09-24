using LogicPOS.Api.Features.Orders.ChangeOrderTable;
using LogicPOS.Api.Features.POS.Orders.Orders.GetOrderById;
using LogicPOS.Api.Features.POS.Tables.Common;
using LogicPOS.UI.Application;
using LogicPOS.UI.Components.Finance.Currencies;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Errors;
using LogicPOS.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace LogicPOS.UI.Components.POS
{
    public static class SaleContext
    {
        public static SaleItemsPage ItemsPage { get; set; }
        public static TableViewModel CurrentTable { get; private set; }
        public static PosOrder CurrentOrder { get; private set; }

        public static void ReloadCurrentOrder()
        {
            if (CurrentOrder.Id == null)
            {
                return;
            }

            CurrentOrder = OrdersService.GetPosOrder(CurrentOrder.Id.Value);
            ItemsPage.Clear(true);
            ItemsPage.PresentOrderItems();
            POSWindow.Instance.UpdateUI();
        }

        public static void ChangeOrderTable(TableViewModel table, Guid orderId)
        {
            CurrentTable = table;
            TablesService.FreeTable(CurrentOrder.Table);

            var command = new ChangeOrderTableCommand()
            {
                OrderId = CurrentOrder.Id.Value,
                NewTableId = table.Id
            };

            var getResult = DependencyInjection.Mediator.Send(command).Result;

            if (getResult.IsError)
            {
                ErrorHandlingService.HandleApiError(getResult);
                return;
            }

            // Avoid GetOpenOrders after table change — we already know the order id.
            CurrentOrder = OrdersService.GetPosOrder(orderId);

            if (CurrentOrder == null)
            {
                CurrentOrder = new PosOrder(table);
            }

            ItemsPage.Clear(true);
            ItemsPage.PresentOrderItems();
            POSWindow.Instance.UpdateUI();
        }

        public static void SetCurrentTable(TableViewModel table)
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
                var defaultTable = TablesService.GetDefaultTable();

                if (defaultTable != null)
                {
                    SetCurrentTable(defaultTable);
                }
            }
        }

        public static bool HasOpenTicket() => ItemsPage.Ticket != null;

        public static void ShowItemOnPoleDisplay(SaleItem item)
        {
            var poleDisplay = LogicPOSApp.UsbDisplay;
            if (poleDisplay == null || item?.Article == null)
            {
                return;
            }

            poleDisplay.ShowSaleItem(item.Article.Designation, item.Quantity, item.UnitPriceWithVat, item.TotalFinal, PoleDisplayCurrency);
        }

        public static void ShowOrderTotalOnPoleDisplay()
        {
            var poleDisplay = LogicPOSApp.UsbDisplay;
            if (poleDisplay == null || CurrentOrder == null)
            {
                return;
            }

            poleDisplay.ShowTotal(CurrentTable?.Designation, CurrentOrder.TotalFinal, PoleDisplayCurrency);
        }

        public static void ShowPoleDisplayStandBy()
        {
            LogicPOSApp.UsbDisplay?.WriteStandBy();
        }

        private static string PoleDisplayCurrency => CurrenciesService.Default?.Acronym ?? PreferenceParametersService.SystemCurrency;

    }
}
