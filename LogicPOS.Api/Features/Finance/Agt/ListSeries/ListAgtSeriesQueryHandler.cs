using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Agt.ListSeries
{
    public class ListAgtSeriesQueryHandler :
        RequestHandler<ListAgtSeriesQuery, ErrorOr<IEnumerable<AgtSeriesInfo>>>
    {
        public ListAgtSeriesQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }
        public override async Task<ErrorOr<IEnumerable<AgtSeriesInfo>>> Handle(ListAgtSeriesQuery query,
                                                                     CancellationToken cancellationToken = default)
        {
            var result = await HandleGetListQueryAsync<AgtSeriesInfo>("agt/online/documents/series", cancellationToken);
            return result;
        }
    }

}
