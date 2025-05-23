using ErrorOr;
using LogicPOS.Api.Features.Common;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Printers.AddPrinter
{
    public class AddPrinterCommandHandler
        : RequestHandler<AddPrinterCommand, ErrorOr<Guid>>
    {
        public AddPrinterCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddPrinterCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("printers", command, cancellationToken);
        }
    }
}
