using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
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
            return await HandlePostCommandAsync($"fiscalyears/{command.Id}/series", command, cancellationToken);
        }
    }
}
