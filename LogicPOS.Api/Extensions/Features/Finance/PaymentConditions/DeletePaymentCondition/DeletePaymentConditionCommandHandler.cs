using ErrorOr;
using LogicPOS.Api.Features.Common;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.PaymentConditions.DeletePaymentCondition
{
    public class DeletePaymentConditionCommandHandler :
        RequestHandler<DeletePaymentConditionCommand, ErrorOr<bool>>
    {
        public DeletePaymentConditionCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<bool>> Handle(DeletePaymentConditionCommand command,
                                                   CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"payment-conditions/{command.Id}", cancellationToken);
        }
    }
}
