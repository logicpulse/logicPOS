using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.FiscalYears.CreateDefaultSeries
{
    public class CreateDefaultSeriesCommandHandler :
        RequestHandler<CreateDefaultSeriesCommand, ErrorOr<Unit>>
    {
        public CreateDefaultSeriesCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(CreateDefaultSeriesCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"fiscalyears/{command.Id}/series", command, cancellationToken);
                return await HandleHttpResponseAsync(response);
            }
            catch (HttpRequestException)
            {
                return ApiErrors.CommunicationError;
            }
        }
    }
}
