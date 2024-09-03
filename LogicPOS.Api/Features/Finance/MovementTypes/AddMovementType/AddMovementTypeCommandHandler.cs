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

namespace LogicPOS.Api.Features.MovementTypes.AddMovementType
{
    public class AddMovementTypeCommandHandler : RequestHandler<AddMovementTypeCommand, ErrorOr<Guid>>
    {
        public AddMovementTypeCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(
            AddMovementTypeCommand command, 
            CancellationToken cancellationToken = default)
        {
            try {     
                    var httpResponse = await _httpClient.PostAsJsonAsync("/movementtypes", command, cancellationToken);
                    return await HandleAddEntityHttpResponseAsync(httpResponse);
                }

            catch (HttpRequestException)
                {
                    return ApiErrors.CommunicationError;
                }
}
    }
}
