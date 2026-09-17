using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common.Responses;
using MediatR;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Common.Requests
{
    public abstract partial class RequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        protected async Task<ErrorOr<TempFile>> HandleGetFileQueryAsync(string endpoint, CancellationToken cancellationToken = default)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(endpoint, cancellationToken);
                response.EnsureSuccessStatusCode();

                var contentDisposition = response.Content.Headers.ContentDisposition;
                var fileName = contentDisposition?.FileName?.Trim('"') ?? null;

                var fileContent = await response.Content.ReadAsByteArrayAsync();
                var filePath = Path.GetTempFileName();

                if (fileName != null && Path.HasExtension(fileName)){
                    filePath = Path.ChangeExtension(filePath,Path.GetExtension(fileName));
                }

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

        protected async Task<ErrorOr<T>> HandlePostFileCommandAsync<T>(
            string endpoint,
            string filePath,
            string formFieldName = "File",
            CancellationToken cancellationToken = default)
        {
            try
            {
                using (var content = new MultipartFormDataContent())
                {
                    var fileBytes = File.ReadAllBytes(filePath);
                    var fileContent = new ByteArrayContent(fileBytes);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                    content.Add(fileContent, formFieldName, Path.GetFileName(filePath));

                    var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
                    return await HandlePostHttpResponseAsync<T>(response);
                }
            }
            catch (HttpRequestException)
            {
                return ApiErrors.APICommunication;
            }
        }
    }
}
