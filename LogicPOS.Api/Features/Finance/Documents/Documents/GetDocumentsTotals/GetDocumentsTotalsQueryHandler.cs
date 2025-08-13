using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Documents.GetDocumentsTotals
{
    public class GetDocumentsTotalsQueryHandler :
        RequestHandler<GetDocumentsTotalsQuery, ErrorOr<IEnumerable<DocumentTotals>>>
    {
        
        public GetDocumentsTotalsQueryHandler(IHttpClientFactory factory, IMemoryCache cache) : base(factory, cache)
        {
        }

        public override async Task<ErrorOr<IEnumerable<DocumentTotals>>> Handle(GetDocumentsTotalsQuery query,
                                                                                CancellationToken cancellationToken = default)
        {
            var cacheOptions = GetCacheOptions();
            var queryUrl = query.GetUrlQuery();
            return await HandleGetListQueryAsync<DocumentTotals>($"documents/totals{queryUrl}", cancellationToken,cacheOptions);
        }

        private MemoryCacheEntryOptions GetCacheOptions()
        {
            return new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
        }
    }
}
