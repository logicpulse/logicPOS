using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Documents.Documents.Prints.GetPrintingModel
{
    public class GetDocumentPrintingModelQueryHandler : RequestHandler<GetDocumentPrintingModelQuery, ErrorOr<DocumentPrintingModel>>
    {
        public GetDocumentPrintingModelQueryHandler(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }

        public override async Task<ErrorOr<DocumentPrintingModel>> Handle(GetDocumentPrintingModelQuery request, CancellationToken ct = default)
        {
            return await HandleGetQueryAsync<DocumentPrintingModel>($"documents/{request.Id}/printing-model", ct);
        }
    }
}
