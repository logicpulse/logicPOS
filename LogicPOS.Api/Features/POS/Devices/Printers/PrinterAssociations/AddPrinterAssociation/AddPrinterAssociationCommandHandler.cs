using ErrorOr;
using LogicPOS.Api.Features.Common;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Printers.PrinterAssociations.AddPrinterAssociations
{
    public class AddPrinterAssociationCommandHandler
        : RequestHandler<AddPrinterAssociationCommand, ErrorOr<Guid>>
    {
        public AddPrinterAssociationCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddPrinterAssociationCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("printers/associations", command, cancellationToken);
        }
    }
}
