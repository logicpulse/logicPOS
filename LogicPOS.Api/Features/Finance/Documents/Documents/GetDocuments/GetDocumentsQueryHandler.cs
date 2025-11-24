using ErrorOr;
using LogicPOS.Api.Features.Common.Pagination;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.Documents.Documents.Common;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Documents.GetDocuments
{
    public class GetDocumentsQueryHandler :
        RequestHandler<GetDocumentsQuery, ErrorOr<PaginatedResult<DocumentViewModel>>>
    {
        public GetDocumentsQueryHandler(IHttpClientFactory factory, IMemoryCache cache) : base(factory, cache)
        { }

        public async override Task<ErrorOr<PaginatedResult<DocumentViewModel>>> Handle(GetDocumentsQuery query, CancellationToken cancellationToken = default)
        {
            var cacheOptions = GetCacheOptions();
            var urlQuery = query.GetUrlQuery();
            return await HandleGetQueryAsync<PaginatedResult<DocumentViewModel>>($"documents{urlQuery}", cancellationToken, cacheOptions);
        }

        private MemoryCacheEntryOptions GetCacheOptions()
        {
            return new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
        }
    }
}
