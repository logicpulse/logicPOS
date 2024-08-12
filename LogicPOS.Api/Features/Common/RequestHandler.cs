using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Common
{
    public abstract class RequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        protected readonly HttpClient _httpClient;
        public RequestHandler(IHttpClientFactory factory) => _httpClient = factory.CreateClient("Default");

        public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);
    }
}
