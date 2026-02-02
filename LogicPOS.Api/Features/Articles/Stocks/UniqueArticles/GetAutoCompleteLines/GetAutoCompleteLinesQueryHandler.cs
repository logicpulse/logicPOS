using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Common.Responses;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.Stocks.UniqueArticles.GetAutoCompleteLines
{
    public class GetAutoCompleteLinesQueryHandler : RequestHandler<GetAutoCompleteLinesQuery, ErrorOr<IEnumerable<AutoCompleteLine>>>
    {
        public GetAutoCompleteLinesQueryHandler(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<AutoCompleteLine>>> Handle(GetAutoCompleteLinesQuery request, CancellationToken cancellationToken = default)
        {
            return await HandleGetListQueryAsync<AutoCompleteLine>("articles/uniques/autocomplete-lines", cancellationToken);  
        }
    }

}
