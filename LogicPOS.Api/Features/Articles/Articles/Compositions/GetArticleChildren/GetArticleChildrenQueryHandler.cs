using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.GetArticleChildren
{
    public class GetArticleChildrenQueryHandler : RequestHandler<GetArticleChildrenQuery, ErrorOr<IEnumerable<ArticleChild>>>
    {
        public GetArticleChildrenQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<ArticleChild>>> Handle(GetArticleChildrenQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetListQueryAsync<ArticleChild>($"articles/{query.Id}/children", cancellationToken);
        }
    }
}
