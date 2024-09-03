using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.Classes.GetAllArticleClasses
{
    public class GetAllArticleClassesQueryHandler :
        RequestHandler<GetAllArticleClassesQuery, ErrorOr<IEnumerable<ArticleClass>>>
    {
        public GetAllArticleClassesQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<ArticleClass>>> Handle(GetAllArticleClassesQuery query,
                                                                     CancellationToken cancellationToken = default)
        {
            try
            {
                var items = await _httpClient.GetFromJsonAsync<List<ArticleClass>>("article/classes",
                                                                                cancellationToken);
                return items;
            }
            catch (HttpRequestException)
            {
                return ApiErrors.CommunicationError;
            }
        }
    }
}
