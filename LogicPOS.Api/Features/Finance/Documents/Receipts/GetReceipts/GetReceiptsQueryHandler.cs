using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Pagination;
using LogicPOS.Api.Features.Common.Requests;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Receipts.GetReceipts
{
    public class GetReceiptsQueryHandler :
        RequestHandler<GetReceiptsQuery, ErrorOr<PaginatedResult<ReceiptViewModel>>>
    {
        public GetReceiptsQueryHandler(IHttpClientFactory factory, IMemoryCache cache ) : base(factory, cache)
        { 
        }

        public async override Task<ErrorOr<PaginatedResult<ReceiptViewModel>>> Handle(GetReceiptsQuery query, CancellationToken cancellationToken = default)
        {
            var cacheOptions = GetCacheOptions();
            var urlQuery = query.GetUrlQuery();
            return await HandleGetQueryAsync<PaginatedResult<ReceiptViewModel>>($"receipts{urlQuery}", cancellationToken,cacheOptions);
        }

        private MemoryCacheEntryOptions GetCacheOptions()
        {
            return new MemoryCacheEntryOptions().SetAbsoluteExpiration(global::System.TimeSpan.FromMinutes(5));
        }
    }
}
