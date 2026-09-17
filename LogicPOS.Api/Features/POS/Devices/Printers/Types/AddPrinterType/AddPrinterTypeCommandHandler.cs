using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.PrinterTypes.AddPrinterType
{
    public class AddPrinterTypeCommandHandler
        : RequestHandler<AddPrinterTypeCommand, ErrorOr<Guid>>
    {
        public AddPrinterTypeCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddPrinterTypeCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("printers/types", command, cancellationToken);
        }
    }
}
