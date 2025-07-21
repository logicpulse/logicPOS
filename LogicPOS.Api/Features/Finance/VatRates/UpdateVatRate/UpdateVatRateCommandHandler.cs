using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.VatRates.UpdateVatRate
{
    public class UpdateVatRateCommandHandler :
        RequestHandler<UpdateVatRateCommand, ErrorOr<Unit>>
    {
        public UpdateVatRateCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(UpdateVatRateCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"/vatrates/{command.Id}", command, cancellationToken);
        }
    }
}
