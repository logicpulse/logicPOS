using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.Documents.Types.Common;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Documents.Types.GetActiveDocumentTypes
{
    public class GetActiveDocumentTypesQueryHandler :
        RequestHandler<GetActiveDocumentTypesQuery, ErrorOr<IEnumerable<DocumentType>>>
    {
        public GetActiveDocumentTypesQueryHandler(IHttpClientFactory httpClientFactory, IMemoryCache cache) : base(httpClientFactory, cache)
        {

        }

        public override async Task<ErrorOr<IEnumerable<DocumentType>>> Handle(GetActiveDocumentTypesQuery query,
                                                                        CancellationToken cancellationToken = default)
        {
            var cacheOptions = GetCacheOptions();
            var result = await HandleGetListQueryAsync<DocumentType>("documents/types/active", cancellationToken, cacheOptions);

            return result;
        }

        private MemoryCacheEntryOptions GetCacheOptions()
        {
            return new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
        }
    }
}
