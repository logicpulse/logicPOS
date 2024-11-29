using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.VatExcemptionReasons.DeleteVatExcemptionReason
{
    public class DeleteVatExemptionReasonCommandHandler :
        RequestHandler<DeleteVatExemptionReasonCommand, ErrorOr<bool>>
    {
        public DeleteVatExemptionReasonCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<bool>> Handle(DeleteVatExemptionReasonCommand request, CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"vat-exemption-reasons/{request.Id}", cancellationToken);
        }
    }
}
