using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Tables.GetAllTables;
using LogicPOS.UI.Alerts;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.POS
{
    public static class SaleContext
    {
        public static Table CurrentTable { get; private set; }
        public static List<PosOrder> Orders { get; private set; } = new List<PosOrder>();

        public static void SetCurrentTable(Table table)
        {
            CurrentTable = table;
        }

        public static PosOrder GetCurrentOrder()
        {
            return Orders.FirstOrDefault(o => o.Table.Id == CurrentTable.Id);
        }

        public static void Initialize()
        {
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
