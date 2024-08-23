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

namespace LogicPOS.Api.Features.Customers.DiscountGroups.AddDiscountGroup
{
    public class AddDiscountGroupCommandHandler : RequestHandler<AddDiscountGroupCommand, ErrorOr<Guid>>
    {
        public AddDiscountGroupCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(
            AddDiscountGroupCommand command, 
            CancellationToken cancellationToken = default)
        {
            try {     
                    var httpResponse = await _httpClient.PostAsJsonAsync("discountgroups", command, cancellationToken);
                    return await HandleAddEntityHttpResponseAsync(httpResponse);
                }

            catch (HttpRequestException)
                {
                    return ApiErrors.CommunicationError;
                }
}
    }
}
