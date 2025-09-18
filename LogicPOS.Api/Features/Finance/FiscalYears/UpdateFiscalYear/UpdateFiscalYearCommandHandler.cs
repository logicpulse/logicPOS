using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.FiscalYears.UpdateFiscalYear
{
    public class UpdateFiscalYearCommandHandler :
        RequestHandler<UpdateFiscalYearCommand, ErrorOr<Success>>
    {
        public UpdateFiscalYearCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Success>> Handle(UpdateFiscalYearCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"fiscalyears/{command.Id}", command, cancellationToken);
        }
    }
}
