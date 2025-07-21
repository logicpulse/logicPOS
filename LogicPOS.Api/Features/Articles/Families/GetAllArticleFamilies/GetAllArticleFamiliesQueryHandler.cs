using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.Families.GetAllArticleFamilies
{
    public class GetAllArticleFamiliesQueryHandler :
        RequestHandler<GetAllArticleFamiliesQuery, ErrorOr<IEnumerable<ArticleFamily>>>
    {
        public GetAllArticleFamiliesQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<ArticleFamily>>> Handle(GetAllArticleFamiliesQuery request, CancellationToken cancellationToken = default)
        {
            return await HandleGetListQueryAsync<ArticleFamily>("articles/families", cancellationToken);
        }
    }
}
