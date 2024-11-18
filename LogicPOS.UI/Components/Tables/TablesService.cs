using LogicPOS.Api.Entities;
using LogicPOS.Api.Enums;
using LogicPOS.Api.Features.Tables.GetAllTables;
using LogicPOS.UI.Errors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace LogicPOS.UI.Services
{
    public static class TablesService
    {
        private static readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();

        public static IEnumerable<Table> GetTables()
        {
            var query = new GetAllTablesQuery();
            var getResult = _mediator.Send(query).Result;

            if (getResult.IsError)
            {
                ErrorHandlingService.HandleApiError(getResult.FirstError, true);
                return null;
            }

            return getResult.Value;
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

            return getResult.Value;
        }

        public static IEnumerable<Table> GetOpenTables() => GetTablesByStatus(TableStatus.Open);

        public static IEnumerable<Table> GetFreeTables() => GetTablesByStatus(TableStatus.Free);

        public static IEnumerable<Table> GetReservedTables() => GetTablesByStatus(TableStatus.Reserved);

    }
}
