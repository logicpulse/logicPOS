using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Documents.Documents.GetDocumentPdf
{
    public class GetDocumentPdfQueryHandler :
        RequestHandler<GetDocumentPdfQuery, ErrorOr<string>>
    {
        public GetDocumentPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<string>> Handle(GetDocumentPdfQuery query, CancellationToken cancellationToken = default)
        {
            try
            {
                var fileContent = await _httpClient.GetByteArrayAsync($"documents/{query.Id}/pdf");
                var fileName = Path.GetTempFileName();
                File.WriteAllBytes(fileName, fileContent);
                return fileName;
            }
            catch (HttpRequestException)
            {
                return ApiErrors.CommunicationError;
            }
        }
    }
}
