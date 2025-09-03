using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.POS.Tables.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Tables.GetAllTables
{
    public class GetTablesQueryHandler :
        RequestHandler<GetTablesQuery, ErrorOr<IEnumerable<TableViewModel>>>
    {
        public GetTablesQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<TableViewModel>>> Handle(GetTablesQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetListQueryAsync<TableViewModel>($"tables{query.GetUrlQuery()}", cancellationToken);
        }
    }
}
