using ErrorOr;
using LogicPOS.Api.Features.Articles.Articles;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.DeleteArticle
{
    public class DeleteArticleCommandHandler :
        RequestHandler<DeleteArticleCommand, ErrorOr<bool>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;
        public DeleteArticleCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public override async Task<ErrorOr<bool>> Handle(DeleteArticleCommand command, CancellationToken cancellationToken = default)
        {
            var result = await HandleDeleteCommandAsync($"articles/{command.Id}", cancellationToken);

            if (result.IsError == false)
            {
                ArticleCache.Clear(_keyedMemoryCache);
            }

            return result;
        }
    }

}
