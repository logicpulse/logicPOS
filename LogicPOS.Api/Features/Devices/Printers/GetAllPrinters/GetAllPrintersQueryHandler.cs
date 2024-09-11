using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Printers.GetAllPrinters
{
    public class GetAllPrintersQueryHandler :
        RequestHandler<GetAllPrintersQuery, ErrorOr<IEnumerable<Printer>>>
    {
        public GetAllPrintersQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<Printer>>> Handle(GetAllPrintersQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetAllQueryAsync<Printer>("printers", cancellationToken);
        }
    }
}
