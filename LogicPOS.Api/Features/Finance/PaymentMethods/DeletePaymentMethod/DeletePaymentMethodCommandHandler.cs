using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.PaymentMethods.DeletePaymentMethod
{
    public class DeletePaymentMethodCommandHandler :
        RequestHandler<DeletePaymentMethodCommand, ErrorOr<bool>>
    {
        public DeletePaymentMethodCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<bool>> Handle(DeletePaymentMethodCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"payment-methods/{command.Id}", cancellationToken);
        }
    }
}
