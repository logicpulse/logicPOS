using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Printers.PrinterAssociations.GetEntityAssociatedPrinterById
{
    public class GetEntityAssociatedPrinterByIdQueryHandler : RequestHandler<GetEntityAssociatedPrinterByIdQuery, ErrorOr<Printer>>
    {
        public GetEntityAssociatedPrinterByIdQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<Printer>> Handle(GetEntityAssociatedPrinterByIdQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetEntityQueryAsync<Printer>($"printers/associations/{query.Id}", cancellationToken);
        }
    }
}
