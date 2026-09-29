using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Documents.Series.HasActiveSeries
{
    public class HasActiveDocumentSeriesQueryHandler
        : RequestHandler<HasActiveDocumentSeriesQuery, ErrorOr<HasActiveDocumentSeriesResponse>>
    {
        public HasActiveDocumentSeriesQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<HasActiveDocumentSeriesResponse>> Handle(
            HasActiveDocumentSeriesQuery request,
            CancellationToken cancellationToken = default)
        {
            return await HandleGetQueryAsync<HasActiveDocumentSeriesResponse>(
                $"documents/series/has-active{request.GetUrlQuery()}",
                cancellationToken);
        }
    }
}
