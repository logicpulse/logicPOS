using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.At.RegisterSeries
{
    public class RegisterSeriesCommandHandler : RequestHandler<RegisterSeriesCommand, ErrorOr<AtSeriesInfo>>
    {
        public RegisterSeriesCommandHandler(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }

        public override async Task<ErrorOr<AtSeriesInfo>> Handle(RegisterSeriesCommand request, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync<AtSeriesInfo>("at/series/register", request, cancellationToken);
        }
    }
}
