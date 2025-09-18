using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.PrinterTypes.UpdatePrinterType
{
    public class UpdatePrinterTypeCommandHandler
         : RequestHandler<UpdatePrinterTypeCommand, ErrorOr<Success>>
    {
        public UpdatePrinterTypeCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Success>> Handle(UpdatePrinterTypeCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"printers/types/{command.Id}", command, cancellationToken);
        }
    }
}
