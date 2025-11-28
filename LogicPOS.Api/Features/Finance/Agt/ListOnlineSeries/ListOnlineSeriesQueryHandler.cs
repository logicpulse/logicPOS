using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Agt.ListOnlineSeries
{
    public class ListOnlineSeriesQueryHandler :
        RequestHandler<ListOnlineSeriesQuery, ErrorOr<IEnumerable<OnlineSeriesInfo>>>
    {
        public ListOnlineSeriesQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }
        public override async Task<ErrorOr<IEnumerable<OnlineSeriesInfo>>> Handle(ListOnlineSeriesQuery query,
                                                                     CancellationToken cancellationToken = default)
        {
            var result = await HandleGetListQueryAsync<OnlineSeriesInfo>("agt/online/documents/series", cancellationToken);
            return result;
        }
    }

}
