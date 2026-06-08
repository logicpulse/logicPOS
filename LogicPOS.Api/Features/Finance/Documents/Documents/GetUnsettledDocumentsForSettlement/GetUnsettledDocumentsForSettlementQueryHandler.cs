using ErrorOr;
using LogicPOS.Api.Features.Common.Pagination;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.Documents.Documents.Common;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Documents.Documents.GetUnsettledDocumentsForSettlement
{
    public class GetUnsettledDocumentsForSettlementQueryHandler
        : RequestHandler<GetUnsettledDocumentsForSettlementQuery, ErrorOr<PaginatedResult<DocumentViewModel>>>
    {
        public GetUnsettledDocumentsForSettlementQueryHandler(IHttpClientFactory factory, IMemoryCache cache)
            : base(factory, cache)
        {
        }

        public override async Task<ErrorOr<PaginatedResult<DocumentViewModel>>> Handle(
            GetUnsettledDocumentsForSettlementQuery query,
            CancellationToken cancellationToken = default)
        {
            var urlQuery = query.GetUrlQuery();
            return await HandleGetQueryAsync<PaginatedResult<DocumentViewModel>>(
                $"documents/unpaid-for-settlement{urlQuery}",
                cancellationToken);
        }
    }
}
