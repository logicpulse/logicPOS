using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Printers.PrinterAssociations.RemoveEntityAssociatedPrinter
{
    public class RemoveEntityAssociatedPrinterCommandHandler :
        RequestHandler<RemoveEntityAssociatedPrinterCommand, ErrorOr<bool>>
    {
        public RemoveEntityAssociatedPrinterCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<bool>> Handle(RemoveEntityAssociatedPrinterCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"printers/associations/{command.Id}", cancellationToken);
        }
    }
}
