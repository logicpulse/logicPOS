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

namespace LogicPOS.Api.Features.Holidays.AddHoliday
{
    public class AddHolidayCommandHandler : RequestHandler<AddHolidayCommand, ErrorOr<Guid>>
    {
        public AddHolidayCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(
            AddHolidayCommand command, 
            CancellationToken cancellationToken = default)
        {
            try {     
                    var httpResponse = await _httpClient.PostAsJsonAsync("/holidays", command, cancellationToken);
                    return await HandleAddEntityHttpResponseAsync(httpResponse);
                }

            catch (HttpRequestException)
                {
                    return ApiErrors.CommunicationError;
                }
}
    }
}
