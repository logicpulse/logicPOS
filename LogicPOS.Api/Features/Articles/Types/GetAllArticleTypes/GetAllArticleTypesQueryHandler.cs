using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.Types.GetAllArticleTypes
{
   public class GetAllArticleTypesQueryHandler :
        RequestHandler<GetAllArticleTypesQuery, ErrorOr<IEnumerable<ArticleType>>>
    {
        public GetAllArticleTypesQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<ArticleType>>> Handle(GetAllArticleTypesQuery request,
                                                                              CancellationToken cancellationToken = default)
        {
           return await HandleGetAllQuery<ArticleType>("article/types", cancellationToken);
        }
    }
}
