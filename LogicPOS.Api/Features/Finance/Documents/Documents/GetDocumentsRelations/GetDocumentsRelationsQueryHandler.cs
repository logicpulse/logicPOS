using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Documents.GetDocumentsRelations
{
    public class GetDocumentsRelationsQueryHandler :
        RequestHandler<GetDocumentsRelationsQuery, ErrorOr<IEnumerable<DocumentRelation>>>
    {

        public GetDocumentsRelationsQueryHandler(IHttpClientFactory factory, IMemoryCache cache) : base(factory, cache)
        {
        }

        public override async Task<ErrorOr<IEnumerable<DocumentRelation>>> Handle(GetDocumentsRelationsQuery query,
                                                                            CancellationToken cancellationToken = default)
        {
            var cacheOptions = GetCacheOptions();
            var httpQuery = string.Join("&", query.DocumentIds.Select(id => $"documentIds={id}"));
            return  await HandleGetListQueryAsync<DocumentRelation>($"documents/relations?{httpQuery}", cancellationToken,cacheOptions);
        }

        private MemoryCacheEntryOptions GetCacheOptions()
        {
            return new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
        }
    }
}
