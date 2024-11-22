using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Reports.WorkSession.Common;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.WorkSession.GetWorkSessionData
{
    public class GetWorkSessionReceiptsDataQueryHandler :
        RequestHandler<GetWorkSessionReceiptsDataQuery, ErrorOr<WorkSessionData>>
    {
        public GetWorkSessionReceiptsDataQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<WorkSessionData>> Handle(GetWorkSessionReceiptsDataQuery request,
                                                                    CancellationToken cancellationToken = default)
        {
            try
            {
                var data = await _httpClient.GetFromJsonAsync<WorkSessionData>($"reports/worksession/receipts/{request.Id}",
                                                                              cancellationToken);
                return data;
            }
            catch (HttpRequestException)
            {
                return ApiErrors.CommunicationError;
            }
        }
    }
}
