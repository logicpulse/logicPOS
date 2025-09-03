using ErrorOr;
using LogicPOS.Api.Features.POS.Tables.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogicPOS.Api.Features.Tables.GetAllTables
{
    public class GetTablesQuery : IRequest<ErrorOr<IEnumerable<TableViewModel>>>
    {
        public TableStatus? Status { get; set; }
        public Guid? PlaceId { get; set; }

        public GetTablesQuery(TableStatus? status = null, Guid? placeId = null)
        {
            Status = status;
            PlaceId = placeId;
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
