using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.Families.DeleteArticleFamily
{
    public class DeleteArticleFamilyCommandHandler :
        RequestHandler<DeleteArticleFamilyCommand, ErrorOr<bool>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;
        public DeleteArticleFamilyCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public override async Task<ErrorOr<bool>> Handle(DeleteArticleFamilyCommand command, CancellationToken cancellationToken = default)
        {
            var result = await HandleDeleteCommandAsync($"articles/families/{command.Id}", cancellationToken);

            if (result.IsError == false)
            {
                ArticleFamiliesCache.Clear(_keyedMemoryCache);
            }

            return result;
        }
    }

}
