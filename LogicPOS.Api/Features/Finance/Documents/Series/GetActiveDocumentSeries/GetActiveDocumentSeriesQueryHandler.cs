using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Documents.Series.GetAllDocumentSeries
{
    public class GetActiveDocumentSeriesQueryHandler :
        RequestHandler<GetActiveDocumentSeriesQuery, ErrorOr<IEnumerable<DocumentSeries>>>
    {
        public GetActiveDocumentSeriesQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<DocumentSeries>>> Handle(GetActiveDocumentSeriesQuery request, CancellationToken cancellationToken = default)
        {
            return await HandleGetListQueryAsync<DocumentSeries>("documents/series/active",cancellationToken);
        }
    }
}
