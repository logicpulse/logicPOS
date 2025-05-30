using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.VatRates.DeleteVatRate
{
    public class DeleteVatRateCommandHandler :
        RequestHandler<DeleteVatRateCommand, ErrorOr<bool>>
    {
        public DeleteVatRateCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<bool>> Handle(DeleteVatRateCommand commmand, CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"vatrates/{commmand.Id}", cancellationToken);
        }
    }
}
