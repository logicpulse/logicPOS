using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.WorkSessions.Movements.GetTotalCashInDrawer
{
    public class GetTotalCashInDrawerQueryHandler :
        RequestHandler<GetTotalCashInDrawerQuery, ErrorOr<decimal?>>
    {
        public GetTotalCashInDrawerQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<decimal?>> Handle(GetTotalCashInDrawerQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetQueryAsync<decimal?>($"worksessions/movements/total-cash-in-drawer/{query.TerminalId}", cancellationToken);
        }
    }
}
