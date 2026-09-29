using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.Families.UpdateArticleFamily
{
    public class UpdateArticleFamilyCommandHandler :
        RequestHandler<UpdateArticleFamilyCommand, ErrorOr<Success>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;
        public UpdateArticleFamilyCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public override async Task<ErrorOr<Success>> Handle(UpdateArticleFamilyCommand command, CancellationToken cancellationToken = default)
        {
            var result = await HandleUpdateCommandAsync($"articles/families/{command.Id}", command, cancellationToken);

            if (result.IsError == false)
            {
                ArticleFamiliesCache.Clear(_keyedMemoryCache);
            }

            return result;
        }
    }
}
