using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Customers.Types.AddCustomerType
{
    public class AddCustomerTypeCommandHandler : RequestHandler<AddCustomerTypeCommand, ErrorOr<Guid>>
    {
        public AddCustomerTypeCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(
            AddCustomerTypeCommand request, 
            CancellationToken cancellationToken = default)
        {
            try {     
                    var httpResponse = await _httpClient.PostAsJsonAsync("customers/types", request, cancellationToken);
                    return await HandleAddEntityHttpResponseAsync(httpResponse);
                }

            catch (HttpRequestException)
                {
                    return ApiErrors.CommunicationError;
                }
}
    }
}
