using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Printers.UpdatePrinter
{
    public class UpdatePrinterCommandHandler
         : RequestHandler<UpdatePrinterCommand, ErrorOr<Unit>>
    {
        public UpdatePrinterCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(UpdatePrinterCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"printers/{command.Id}", command, cancellationToken);
        }
    }
}
