using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.FiscalYears.UpdateFiscalYear
{
    public class UpdateFiscalYearCommandHandler :
        RequestHandler<UpdateFiscalYearCommand, ErrorOr<Unit>>
    {
        public UpdateFiscalYearCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(UpdateFiscalYearCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"/finance/fiscalyears/{command.Id}", command, cancellationToken);
        }
    }
}
