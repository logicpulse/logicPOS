using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.Subfamilies.UpdateArticleSubfamily
{
    public class UpdateArticleSubfamilyCommandHandler :
        RequestHandler<UpdateArticleSubfamilyCommand, ErrorOr<Success>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;
        public UpdateArticleSubfamilyCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public override async Task<ErrorOr<Success>> Handle(UpdateArticleSubfamilyCommand command, CancellationToken cancellationToken = default)
        {
            var result = await HandleUpdateCommandAsync($"articles/subfamilies/{command.Id}", command, cancellationToken);

            if (result.IsError == false)
            {
                SubfamiliesCache.Clear(_keyedMemoryCache);
            }

            return result;
        }


    }
}
