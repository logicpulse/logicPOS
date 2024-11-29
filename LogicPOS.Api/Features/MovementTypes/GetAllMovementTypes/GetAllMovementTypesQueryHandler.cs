using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.MovementTypes.GetAllMovementTypes
{
   public class GetAllMovementTypesQueryHandler :
        RequestHandler<GetAllMovementTypesQuery, ErrorOr<IEnumerable<MovementType>>>
    {
        public GetAllMovementTypesQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<MovementType>>> Handle(GetAllMovementTypesQuery request,
                                                                              CancellationToken cancellationToken = default)
        {
            try
            {
                var holidays = await _httpClient.GetFromJsonAsync<List<MovementType>>("/movementtypes", cancellationToken);

                return holidays;
            }
            catch (HttpRequestException)
            {
                return ApiErrors.CommunicationError;
            }
        }
    }
}
