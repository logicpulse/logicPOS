using ErrorOr;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.PaymentConditions.UpdatePaymentCondition
{
    public class UpdatePaymentConditionCommandHandler :
        RequestHandler<UpdatePaymentConditionCommand, ErrorOr<Unit>>
    {
        public UpdatePaymentConditionCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(UpdatePaymentConditionCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"/payment/conditions/{command.Id}", command, cancellationToken);
        }
    }
}
