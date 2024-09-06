using ErrorOr;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.VatExemptionReasons.UpdateVatExemptionReason
{
    public class UpdateVatExemptionReasonCommandHandler :
        RequestHandler<UpdateVatExemptionReasonCommand, ErrorOr<Unit>>
    {
        public UpdateVatExemptionReasonCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(UpdateVatExemptionReasonCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"/vatexemptionreasons/{command.Id}", command, cancellationToken);
        }
    }
}
