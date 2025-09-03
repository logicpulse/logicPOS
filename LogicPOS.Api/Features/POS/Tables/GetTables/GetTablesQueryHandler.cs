using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Tables.GetAllTables
{
    public class GetTablesQueryHandler :
        RequestHandler<GetTablesQuery, ErrorOr<IEnumerable<Table>>>
    {
        public GetTablesQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<Table>>> Handle(GetTablesQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetListQueryAsync<Table>($"tables{query.GetUrlQuery()}", cancellationToken);
        }
    }
}
