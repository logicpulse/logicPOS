using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.Subfamilies.AddArticleSubfamily
{
    public class AddArticleSubfamilyCommandHandler :
        RequestHandler<AddArticleSubfamilyCommand, ErrorOr<Guid>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;
        public AddArticleSubfamilyCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public override async Task<ErrorOr<Guid>> Handle(AddArticleSubfamilyCommand request,
                                                         CancellationToken cancellationToken = default)
        {
            var result = await HandleAddCommandAsync("articles/subfamilies", request, cancellationToken);
            if(result.IsError == false)
            {
                SubfamiliesCache.Clear(_keyedMemoryCache);
            }
            return result;
        }
    }
}
