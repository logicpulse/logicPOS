using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogicPOS.Api.Features.Tables.GetAllTables
{
    public class GetAllTablesQuery : IRequest<ErrorOr<IEnumerable<Table>>>
    {
        public TableStatus? Status { get; set; }
        public Guid? PlaceId { get; set; }

        public GetAllTablesQuery(TableStatus? status = null)
        {
            Status = status;
        }

        public string GetUrlQuery()
        {
            var queryBuilder = new StringBuilder("?");

            if (Status != null) {
                queryBuilder.Append($"Status={(int)Status}");
            }

            if (PlaceId != null)
            {
                queryBuilder.Append($"PlaceId={PlaceId}");
            }

            return queryBuilder.ToString();
        }

    }
}
