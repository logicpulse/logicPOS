using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Tables.GetTableTotal
{
    public class GetTableTotalQueryHandler :
        RequestHandler<GetTableTotalQuery, ErrorOr<decimal>>
    {
        public GetTableTotalQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<decimal>> Handle(GetTableTotalQuery query,
                                                            CancellationToken cancellationToken = default)
        {
            return await HandleGetQueryAsync<decimal>($"tables/{query.TableId}/total",cancellationToken);
        }
    }
}
