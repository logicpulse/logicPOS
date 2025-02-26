using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.VatExemptionReasons.GetAllVatExemptionReasons
{
    public class GetAllVatExemptionReasonsQueryHandler :
        RequestHandler<GetAllVatExemptionReasonsQuery, ErrorOr<IEnumerable<VatExemptionReason>>>
    {
        public GetAllVatExemptionReasonsQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<VatExemptionReason>>> Handle(GetAllVatExemptionReasonsQuery query,
                                                                     CancellationToken cancellationToken = default)
        {
            return await HandleGetEntitiesQueryAsync<VatExemptionReason>("VatExemptionReasons", cancellationToken);
        }
    }
}
