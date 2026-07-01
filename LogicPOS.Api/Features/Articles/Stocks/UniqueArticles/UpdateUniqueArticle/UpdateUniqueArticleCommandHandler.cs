using ErrorOr;
using LogicPOS.Api.Features.Articles.Articles;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.StockManagement.UpdateUniqueArticle
{
    public class UpdateUniqueArticleCommandHandler :
        RequestHandler<UpdateUniqueArticleCommand, ErrorOr<Success>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;

        public UpdateUniqueArticleCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public async override Task<ErrorOr<Success>> Handle(UpdateUniqueArticleCommand command, CancellationToken cancellationToken = default)
        {
            var result = await HandleUpdateCommandAsync($"articles/uniques/{command.Id}", command, cancellationToken);

            if (result.IsError == false)
            {
                ArticlesCache.Clear(_keyedMemoryCache);
            }

            return result;
        }
    }
}
