using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Customers.Types.GetAllCustomerTypes
{
    public class GetAllCustomerTypesQueryHandler :
        RequestHandler<GetAllCustomerTypesQuery, ErrorOr<IEnumerable<CustomerType>>>
    {
        public GetAllCustomerTypesQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<CustomerType>>> Handle(GetAllCustomerTypesQuery request,
                                                                              CancellationToken cancellationToken = default)
        {
            try
            {
                var customerTypes = await _httpClient.GetFromJsonAsync<List<CustomerType>>("customers/types", cancellationToken);

                return customerTypes;
            }
            catch (HttpRequestException)
            {
                return ApiErrors.CommunicationError;
            }
        }
    }
}
