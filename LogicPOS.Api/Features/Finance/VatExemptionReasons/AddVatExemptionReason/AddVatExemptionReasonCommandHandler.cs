using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.VatExemptionReasons.AddVatExemptionReason
{
    public class AddVatExceptionReasonCommandHandler : RequestHandler<AddVatExemptionReasonCommand, ErrorOr<Guid>>
    {
        public AddVatExceptionReasonCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddVatExemptionReasonCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("vatexemptionreasons", command, cancellationToken);
        }
    }
}
