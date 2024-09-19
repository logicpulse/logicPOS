using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Documents.Series.GetAllDocumentSeries
{
    public class GetAllDocumentSeriesQueryHandler :
        RequestHandler<GetAllDocumentSeriesQuery, ErrorOr<IEnumerable<DocumentSerie>>>
    {
        public GetAllDocumentSeriesQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<DocumentSerie>>> Handle(GetAllDocumentSeriesQuery request, CancellationToken cancellationToken = default)
        {
            return await HandleGetAllQueryAsync<DocumentSerie>("document/series",cancellationToken);
        }
    }
}
