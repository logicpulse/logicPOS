using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Enums;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Tables.GetAllTables
{
    public class GetAllTablesQuery : IRequest<ErrorOr<IEnumerable<Table>>>
    {
        public TableStatus? Status { get; set; }

        public GetAllTablesQuery(TableStatus? status = null)
        {
            Status = status;
        }

    }
}
