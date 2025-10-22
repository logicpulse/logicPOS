using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common.Requests;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Documents.Documents.GetDocumentPreviewData
{
    public class GetDocumentPreviewDataQueryHandler :
        RequestHandler<GetDocumentPreviewDataQuery, ErrorOr<Document>>
    {
        public GetDocumentPreviewDataQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Document>> Handle(GetDocumentPreviewDataQuery query, CancellationToken cancellationToken = default)
        {
           return await HandlePostCommandAsync<Document>("documents/preview/data",query,cancellationToken);
        }
    }
}
