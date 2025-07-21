using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Tables.GetAllTables
{
    public class GetAllTablesQueryHandler :
        RequestHandler<GetAllTablesQuery, ErrorOr<IEnumerable<Table>>>
    {
        public GetAllTablesQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<Table>>> Handle(GetAllTablesQuery query, CancellationToken cancellationToken = default)
        {
            var endpoint = "tables" + (query.Status.HasValue ? $"?status={(int)query.Status}" : string.Empty);

            return await HandleGetListQueryAsync<Table>(endpoint, cancellationToken);
        }
    }
}
