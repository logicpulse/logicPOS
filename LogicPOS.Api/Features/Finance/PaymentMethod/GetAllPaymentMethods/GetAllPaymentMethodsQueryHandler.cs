using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.PaymentMethods.GetAllPaymentMethods
{
    public class GetAllPaymentMethodsQueryHandler :
        RequestHandler<GetAllPaymentMethodsQuery, ErrorOr<IEnumerable<PaymentMethod>>>
    {
        public GetAllPaymentMethodsQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<PaymentMethod>>> Handle(GetAllPaymentMethodsQuery query,
                                                                     CancellationToken cancellationToken = default)
        {
            try
            {
                var items = await _httpClient.GetFromJsonAsync<List<PaymentMethod>>("payment/methods",
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
