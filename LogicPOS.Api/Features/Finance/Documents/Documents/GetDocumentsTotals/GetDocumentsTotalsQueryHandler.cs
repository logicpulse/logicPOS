using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Documents.GetDocumentsTotals
{
    public class GetDocumentsTotalsQueryHandler :
        RequestHandler<GetDocumentsTotalsQuery, ErrorOr<IEnumerable<DocumentTotals>>>
    {
        public GetDocumentsTotalsQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<DocumentTotals>>> Handle(GetDocumentsTotalsQuery query,
                                                                                CancellationToken cancellationToken = default)
        {
            var queryUrl = query.GetUrlQuery();
            return await HandleGetListQueryAsync<DocumentTotals>($"documents/totals{queryUrl}", cancellationToken);
        }
    }
}
