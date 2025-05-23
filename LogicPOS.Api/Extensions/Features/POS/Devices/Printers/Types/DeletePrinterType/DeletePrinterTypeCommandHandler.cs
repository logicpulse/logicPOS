using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.PrinterTypes.DeletePrinterType
{
    public class DeletePrinterTypeCommandHandler :
        RequestHandler<DeletePrinterTypeCommand, ErrorOr<bool>>
    {
        public DeletePrinterTypeCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<bool>> Handle(DeletePrinterTypeCommand request, CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"printers/types/{request.Id}", cancellationToken);
        }
    }
}
