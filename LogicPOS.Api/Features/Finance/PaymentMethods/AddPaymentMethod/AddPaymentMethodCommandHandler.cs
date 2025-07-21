using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.PaymentMethods.AddPaymentMethod
{
    public class AddPaymentMethodCommandHandler : RequestHandler<AddPaymentMethodCommand, ErrorOr<Guid>>
    {
        public AddPaymentMethodCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddPaymentMethodCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("payment/methods", command, cancellationToken);
        }
    }
}
