using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.GetArticleById;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.Articles.GetArticleImage
{
    public class GetArticleImageQueryHandler :
        RequestHandler<GetArticleImageQuery, ErrorOr<string>>
    {
        public GetArticleImageQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<string>> Handle(GetArticleImageQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetEntityQueryAsync<string>($"articles/{query.Id}/image", cancellationToken);
        }
    }
}
