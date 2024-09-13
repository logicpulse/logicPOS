using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.FiscalYears.GetAllFiscalYears
{
    public class GetAllFiscalYearsQueryHandler :
        RequestHandler<GetAllFiscalYearsQuery, ErrorOr<IEnumerable<FiscalYear>>>
    {
        public GetAllFiscalYearsQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<FiscalYear>>> Handle(GetAllFiscalYearsQuery query,
                                                                     CancellationToken cancellationToken = default)
        {
            try
            {
                var items = await _httpClient.GetFromJsonAsync<List<FiscalYear>>("finance/fiscalyears",
                                                                                cancellationToken);
                return items;
            }
            catch (HttpRequestException)
            {
                return ApiErrors.CommunicationError;
            }
        }
    }
}
