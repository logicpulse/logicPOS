using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.PrinterTypes.GetAllPrinterTypes
{
    public class GetAllPrinterTypesQueryHandler :
        RequestHandler<GetAllPrinterTypesQuery, ErrorOr<IEnumerable<PrinterType>>>
    {
        public GetAllPrinterTypesQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<PrinterType>>> Handle(GetAllPrinterTypesQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetEntitiesQueryAsync<PrinterType>("printers/types", cancellationToken);
        }
    }
}
