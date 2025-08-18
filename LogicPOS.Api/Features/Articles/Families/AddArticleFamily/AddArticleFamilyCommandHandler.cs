using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.Families.AddArticleFamily
{
    public class AddArticleFamilyCommandHandler :
        RequestHandler<AddArticleFamilyCommand, ErrorOr<Guid>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;
        public AddArticleFamilyCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public override async Task<ErrorOr<Guid>> Handle(AddArticleFamilyCommand command, CancellationToken cancellationToken = default)
        {      
            var result = await HandleAddCommandAsync("articles/families", command, cancellationToken);
            if(result.IsError == false)
            {
                ArticleFamiliesCache.Clear(_keyedMemoryCache);
            }
            return result;
        }
    }
}
