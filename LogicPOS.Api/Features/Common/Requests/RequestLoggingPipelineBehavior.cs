using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Common
{
    public sealed class RequestLoggingPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> _logger;
        public RequestLoggingPipelineBehavior(ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {

            _logger.LogInformation("Handling request {Request}", request);

            var response = await next();

            _logger.LogInformation("Handled request {Request} with response {Response}", request, response);

            return response;
        }
    }
}
