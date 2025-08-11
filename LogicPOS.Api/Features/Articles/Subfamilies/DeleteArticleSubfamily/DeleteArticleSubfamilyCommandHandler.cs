using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.Subfamilies.DeleteArticleSubfamily
{
    public class DeleteArticleSubfamilyCommandHandler :
        RequestHandler<DeleteArticleSubfamilyCommand, ErrorOr<bool>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;
        public DeleteArticleSubfamilyCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public async override Task<ErrorOr<bool>> Handle(DeleteArticleSubfamilyCommand command, CancellationToken cancellationToken = default)
        {
            var result = await HandleDeleteCommandAsync($"articles/subfamilies/{command.Id}", cancellationToken);

            if (result.IsError == false)
            {
                SubfamiliesCache.Clear(_keyedMemoryCache);
            }

            return result;
        }
    }
}
