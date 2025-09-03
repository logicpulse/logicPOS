using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.POS.Tables.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Tables.GetDefaultTable
{
    public class GetDefaultTableQueryHandler :
        RequestHandler<GetDefaultTableQuery, ErrorOr<TableViewModel>>
    {
        public GetDefaultTableQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<TableViewModel>> Handle(GetDefaultTableQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetEntityQueryAsync<TableViewModel>($"tables/default{query.GetUrlQuery()}", cancellationToken);
        }
    }
}
