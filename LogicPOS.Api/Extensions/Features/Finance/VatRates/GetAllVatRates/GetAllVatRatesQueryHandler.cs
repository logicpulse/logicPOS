using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.VatRates.GetAllVatRate
{
    public class GetAllVatRatesQueryHandler :
        RequestHandler<GetAllVatRatesQuery, ErrorOr<IEnumerable<VatRate>>>
    {
        public GetAllVatRatesQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<VatRate>>> Handle(GetAllVatRatesQuery query,
                                                                     CancellationToken cancellationToken = default)
        {
            return await HandleGetEntitiesQueryAsync<VatRate>("vat-rates", cancellationToken);
        }
    }
}
