using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Common.Responses;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Documents.Documents.GetDocumentPreviewPdf
{
    public class GetDocumentPreviewPdfQueryHandler :
        RequestHandler<GetDocumentPreviewPdfQuery, ErrorOr<TempFile>>
    {
        public GetDocumentPreviewPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<TempFile>> Handle(GetDocumentPreviewPdfQuery query, CancellationToken cancellationToken = default)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"documents/preview/pdf",query, cancellationToken);
                response.EnsureSuccessStatusCode();

                var contentDisposition = response.Content.Headers.ContentDisposition;
                var fileName = contentDisposition?.FileName?.Trim('"') ?? null;

                var fileContent = await response.Content.ReadAsByteArrayAsync();
                var filePath = Path.GetTempFileName();
                File.WriteAllBytes(filePath, fileContent);

                var tempFile = new TempFile
                {
                    Name = fileName,
                    Path = filePath
                };

                response.Dispose();

                return tempFile;
            }
            catch (HttpRequestException)
            {
                return ApiErrors.APICommunication;
            }
        }
    }
}
