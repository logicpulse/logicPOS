using ErrorOr;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.PaymentMethods.UpdatePaymentMethod
{
    public class UpdatePaymentMethodCommandHandler :
        RequestHandler<UpdatePaymentMethodCommand, ErrorOr<Unit>>
    {
        public UpdatePaymentMethodCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(UpdatePaymentMethodCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"/payment/methods/{command.Id}", command, cancellationToken);
        }
    }
}
