using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Printers.DeletePrinter
{
    public class DeletePrinterCommandHandler :
        RequestHandler<DeletePrinterCommand, ErrorOr<bool>>
    {
        public DeletePrinterCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<bool>> Handle(DeletePrinterCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"printers/{command.Id}", cancellationToken);
        }
    }
}
