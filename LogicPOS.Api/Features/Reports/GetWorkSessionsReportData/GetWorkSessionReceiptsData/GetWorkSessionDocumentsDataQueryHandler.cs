using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Reports.WorkSession.Common;
using System.Net.Http;
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
            return await HandleGetEntityQueryAsync<WorkSessionData>($"reports/worksession/receipts/{request.Id}",
                                                                    cancellationToken);
        }
    }
}
