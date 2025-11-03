using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.DocumentTypes.GetAllDocumentTypes
{
    public class GetAllDocumentTypesQueryHandler :
        RequestHandler<GetAllDocumentTypesQuery, ErrorOr<IEnumerable<DocumentType>>>
    {
        public GetAllDocumentTypesQueryHandler(IHttpClientFactory httpClientFactory, IMemoryCache cache) : base(httpClientFactory, cache)
        {

        }

        public override async Task<ErrorOr<IEnumerable<DocumentType>>> Handle(GetAllDocumentTypesQuery query,
                                                                        CancellationToken cancellationToken = default)
        {
            var cacheOptions = GetCacheOptions();
            var result = await HandleGetListQueryAsync<DocumentType>("documents/types", cancellationToken, cacheOptions);

            return result;
        }

        private MemoryCacheEntryOptions GetCacheOptions()
        {
            return new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
        }
    }
}
