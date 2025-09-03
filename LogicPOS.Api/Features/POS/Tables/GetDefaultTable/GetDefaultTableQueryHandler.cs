using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Tables.GetDefaultTable
{
    public class GetDefaultTableQueryHandler :
        RequestHandler<GetDefaultTableQuery, ErrorOr<Table>>
    {
        public GetDefaultTableQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Table>> Handle(GetDefaultTableQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetEntityQueryAsync<Table>($"tables/default{query.GetUrlQuery()}", cancellationToken);
        }
    }
}
