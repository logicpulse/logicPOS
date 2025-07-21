using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Pagination;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Receipts.GetReceipts
{
    public class GetReceiptsQueryHandler :
        RequestHandler<GetReceiptsQuery, ErrorOr<PaginatedResult<ReceiptViewModel>>>
    {
        public GetReceiptsQueryHandler(IHttpClientFactory factory) : base(factory)
        { }

        public async override Task<ErrorOr<PaginatedResult<ReceiptViewModel>>> Handle(GetReceiptsQuery query, CancellationToken cancellationToken = default)
        {
            var urlQuery = query.GetUrlQuery();
            return await HandleGetEntityQueryAsync<PaginatedResult<ReceiptViewModel>>($"receipts{urlQuery}", cancellationToken);
        }
    }
}
