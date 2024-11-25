using LogicPOS.Api.Entities;
using LogicPOS.Api.Enums;
using LogicPOS.Api.Features.Tables.FreeTable;
using LogicPOS.Api.Features.Tables.GetAllTables;
using LogicPOS.Api.Features.Tables.GetTableTotal;
using LogicPOS.Api.Features.Tables.ReserveTable;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Errors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Services
{
    public static class TablesService
    {
        private static readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();

        public static IEnumerable<Table> GetAllTables()
        {
            var query = new GetAllTablesQuery();
            var getResult = _mediator.Send(query).Result;

            if (getResult.IsError)
            {
                ErrorHandlingService.HandleApiError(getResult.FirstError, true);
                return null;
            }

            var tables = getResult.Value;

            return FilterTerminalTables(getResult.Value);
        }

        private static IEnumerable<Table> FilterTerminalTables(IEnumerable<Table> tables)
        {
            if (TerminalService.Terminal.PlaceId != null)
            {
                return tables.Where(t => t.PlaceId == TerminalService.Terminal.PlaceId);
            }

            return tables;
        }

        public static IEnumerable<Table> GetTablesByStatus(TableStatus status)
        {
            var query = new GetAllTablesQuery(status);
            var getResult = _mediator.Send(query).Result;

            if (getResult.IsError)
            {
                ErrorHandlingService.HandleApiError(getResult.FirstError, true);
                return null;
            }

            return FilterTerminalTables(getResult.Value);
        }

        public static IEnumerable<Table> GetOpenTables() => GetTablesByStatus(TableStatus.Open);

        public static IEnumerable<Table> GetFreeTables() => GetTablesByStatus(TableStatus.Free);

        public static IEnumerable<Table> GetReservedTables() => GetTablesByStatus(TableStatus.Reserved);

        public static void ReserveTable(Table table)
        {
            if (table.Status != TableStatus.Free)
            {
                return;
            }

            var result = _mediator.Send(new ReserveTableCommand(table.Id)).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result.FirstError, true);
            }
        }

        public static void FreeTable(Table table)
        {
            if (table.Status != TableStatus.Reserved)
            {
                return;
            }

            var result = _mediator.Send(new FreeTableCommand(table.Id)).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result.FirstError, true);
            }
        }

        public static decimal GetTableTotal(Guid tableId)
        {
            var query = new GetTableTotalQuery(tableId);
            var getResult = _mediator.Send(query).Result;

            if (getResult.IsError)
            {
                ErrorHandlingService.HandleApiError(getResult.FirstError, true);
                return 0;
            }

            return getResult.Value;
        }
    }
}
