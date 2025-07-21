using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.Subfamilies.GetAllArticleSubfamilies
{
    public class GetAllArticleSubfamiliesQueryHandler :
        RequestHandler<GetAllArticleSubfamiliesQuery, ErrorOr<IEnumerable<ArticleSubfamily>>>
    {
        public GetAllArticleSubfamiliesQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<ArticleSubfamily>>> Handle(GetAllArticleSubfamiliesQuery request,
                                                                            CancellationToken cancellationToken = default)
        {
            return await HandleGetListQueryAsync<ArticleSubfamily>("articles/subfamilies", cancellationToken);
        }
    }
}
