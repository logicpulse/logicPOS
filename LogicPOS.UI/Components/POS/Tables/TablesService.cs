using LogicPOS.Api.Features.POS.Tables.Common;
using LogicPOS.Api.Features.Tables.FreeTable;
using LogicPOS.Api.Features.Tables.GetAllTables;
using LogicPOS.Api.Features.Tables.GetDefaultTable;
using LogicPOS.Api.Features.Tables.GetTableById;
using LogicPOS.Api.Features.Tables.GetTableTotal;
using LogicPOS.Api.Features.Tables.GetTableViewModel;
using LogicPOS.Api.Features.Tables.ReserveTable;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Errors;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Services
{
    public static class TablesService
    {
        public static Table GetTable(Guid id)
        {
            var requestResult = DependencyInjection.Mediator.Send(new GetTableByIdQuery(id)).Result;

            if (requestResult.IsError)
            {
                ErrorHandlingService.HandleApiError(requestResult, true);
                return null;
            }

            return requestResult.Value;
        }

        public static TableViewModel GetTableViewModel(Guid id)
        {
            var requestResult = DependencyInjection.Mediator.Send(new GetTableViewModelQuery(id)).Result;

            if (requestResult.IsError)
            {
                ErrorHandlingService.HandleApiError(requestResult, true);
                return null;
            }

            return requestResult.Value;
        }

        public static IEnumerable<TableViewModel> GetAllTables()
        {
            var query = new GetTablesQuery();

            if (TerminalService.Terminal.PlaceId != null)
            {
                query.PlaceId = TerminalService.Terminal.PlaceId;
            }

            var requestResult = DependencyInjection.Mediator.Send(query).Result;

            if (requestResult.IsError)
            {
                ErrorHandlingService.HandleApiError(requestResult, true);
                return null;
            }

            return requestResult.Value;
        }

        public static IEnumerable<TableViewModel> GetTablesByStatus(TableStatus status)
        {
            var query = new GetTablesQuery(status);

            if (TerminalService.Terminal.PlaceId != null)
            {
                query.PlaceId = TerminalService.Terminal.PlaceId;
            }

            var requestResult = DependencyInjection.Mediator.Send(query).Result;

            if (requestResult.IsError)
            {
                ErrorHandlingService.HandleApiError(requestResult, true);
                return null;
            }

            return requestResult.Value;
        }

        public static IEnumerable<TableViewModel> GetOpenTables() => GetTablesByStatus(TableStatus.Open);

        public static IEnumerable<TableViewModel> GetFreeTables() => GetTablesByStatus(TableStatus.Free);

        public static IEnumerable<TableViewModel> GetReservedTables() => GetTablesByStatus(TableStatus.Reserved);

        public static void ReserveTable(TableViewModel table)
        {
            if (table.Status != TableStatus.Free)
            {
                return;
            }

            var result = DependencyInjection.Mediator.Send(new ReserveTableCommand(table.Id)).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result, true);
            }
        }

        public static void FreeTable(TableViewModel table)
        {
            if (table.Status != TableStatus.Reserved)
            {
                return;
            }

            var result = DependencyInjection.Mediator.Send(new FreeTableCommand(table.Id)).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result, true);
            }
        }

        public static decimal GetTableTotal(Guid tableId)
        {
            var query = new GetTableTotalQuery(tableId);
            var getResult = DependencyInjection.Mediator.Send(query).Result;

            if (getResult.IsError)
            {
                ErrorHandlingService.HandleApiError(getResult, true);
                return 0;
            }

            return getResult.Value;
        }

        public static TableViewModel GetDefaultTable()
        {
            var query = new GetDefaultTableQuery(TerminalService.Terminal.Id);

            var result = DependencyInjection.Mediator.Send(query).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return null;
            }

            return result.Value;
        }
    }
}
