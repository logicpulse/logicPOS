using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Common.Pagination;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Receipts.GetReceipts
{
    public class GetReceiptsQueryHandler :
        RequestHandler<GetReceiptsQuery, ErrorOr<PaginatedResult<Receipt>>>
    {
        public GetReceiptsQueryHandler(IHttpClientFactory factory) : base(factory)
        { }

        public async override Task<ErrorOr<PaginatedResult<Receipt>>> Handle(GetReceiptsQuery query, CancellationToken cancellationToken = default)
        {
            var urlQuery = query.GetUrlQuery();
            return await HandleGetEntityQueryAsync<PaginatedResult<Receipt>>($"receipts/paginated{urlQuery}", cancellationToken);
        }
    }
}
