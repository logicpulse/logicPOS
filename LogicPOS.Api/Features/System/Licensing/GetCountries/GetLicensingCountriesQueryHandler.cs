using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.System.Licensing.GetCountries
{
    public class GetLicensingCountriesQueryHandler :
        RequestHandler<GetLicensingCountriesQuery, ErrorOr<IEnumerable<string>>>
    {
        public GetLicensingCountriesQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<string>>> Handle(GetLicensingCountriesQuery query,
                                                                  CancellationToken ct = default)
        {
            return await HandleGetListQueryAsync<string>("licensing/countries", ct);
        }
    }
}
