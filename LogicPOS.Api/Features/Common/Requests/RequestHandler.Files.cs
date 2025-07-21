using ErrorOr;
using LogicPOS.Api.Errors;
using MediatR;
using System.IO;
using System.Net.Http;
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
