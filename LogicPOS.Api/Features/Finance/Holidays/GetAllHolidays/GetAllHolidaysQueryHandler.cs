using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Holidays.GetAllHolidays
{
   public class GetAllHolidaysQueryHandler :
        RequestHandler<GetAllHolidaysQuery, ErrorOr<IEnumerable<Holiday>>>
    {
        public GetAllHolidaysQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<Holiday>>> Handle(GetAllHolidaysQuery request,
                                                                              CancellationToken cancellationToken = default)
        {
            try
            {
                var holidays = await _httpClient.GetFromJsonAsync<List<Holiday>>("/holidays", cancellationToken);

                return holidays;
            }
            catch (HttpRequestException)
            {
                return ApiErrors.CommunicationError;
            }
        }
    }
}
