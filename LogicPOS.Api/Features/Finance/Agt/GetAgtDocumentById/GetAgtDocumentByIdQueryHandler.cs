using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.Agt.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Agt.GetAgtDocumentById
{
    public class GetAgtDocumentByIdQueryHandler : RequestHandler<GetAgtDocumentByIdQuery, ErrorOr<AgtDocument>>
    {
        public GetAgtDocumentByIdQueryHandler(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }

        public override async Task<ErrorOr<AgtDocument>> Handle(GetAgtDocumentByIdQuery request, CancellationToken ct = default)
        {
            return await HandleGetQueryAsync<AgtDocument>($"agt/fe/documents/{request.DocumentId}", ct);
        }
    }
}
