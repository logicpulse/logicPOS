using ErrorOr;
using LogicPOS.Api.Features.Common;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.PaymentConditions.AddPaymentCondition
{
    public class AddPaymentConditionCommandHandler : RequestHandler<AddPaymentConditionCommand, ErrorOr<Guid>>
    {
        public AddPaymentConditionCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddPaymentConditionCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("payment/conditions", command, cancellationToken);
        }
    }
}
