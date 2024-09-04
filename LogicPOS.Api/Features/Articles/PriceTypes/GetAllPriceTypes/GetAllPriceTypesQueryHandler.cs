using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.PriceTypes.GetAllPriceTypes
{
    public class GetAllPriceTypesQueryHandler :
        RequestHandler<GetAllPriceTypesQuery, ErrorOr<IEnumerable<PriceType>>>
    {
        public GetAllPriceTypesQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<PriceType>>> Handle(GetAllPriceTypesQuery query,
                                                                     CancellationToken cancellationToken = default)
        {
            try
            {
                var items = await _httpClient.GetFromJsonAsync<List<PriceType>>("articles/pricetypes",
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
