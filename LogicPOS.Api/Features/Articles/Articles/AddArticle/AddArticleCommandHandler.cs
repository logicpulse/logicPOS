using ErrorOr;
using LogicPOS.Api.Features.Articles.Articles;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.AddArticle
{
    public class AddArticleCommandHandler :
        RequestHandler<AddArticleCommand, ErrorOr<Guid>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;

        public AddArticleCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public override async Task<ErrorOr<Guid>> Handle(AddArticleCommand request, CancellationToken cancellationToken = default)
        {
            var result = await HandleAddCommandAsync("articles", request, cancellationToken);

            if(result.IsError == false)
            {
                ArticleCache.Clear(_keyedMemoryCache);
            }

            return result;
        }
    }
}
