using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Customers.Types.UpdateCustomerType
{
    public class UpdateCustumerTypeCommandHandler :
        RequestHandler<UpdateCustomerTypeCommand, ErrorOr<Unit>>
    {
        public UpdateCustumerTypeCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(UpdateCustomerTypeCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"customers/types/{command.Id}", command, cancellationToken);
                return await HandleUpdateEntityHttpResponseAsync(response);
            }
            catch (HttpRequestException)
            {
                return ApiErrors.CommunicationError;
            }
        }
    }
}
