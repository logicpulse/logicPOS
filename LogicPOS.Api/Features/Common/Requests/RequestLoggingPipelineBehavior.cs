using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Reflection;
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
            var response = await next();

            if (response is IErrorOr)
            {
                HandleErrorOrResponse(request, response);
            }

            return response;
        }

        private void HandleErrorOrResponse(TRequest request, TResponse response)
        {
            var requestTypeName = request.GetType().Name;
            var result = (IErrorOr)response;

            if (result.IsError)
            {
                _logger.LogError("Request {Request} failed with errors {Errors}", requestTypeName, result.Errors);
                return;
            }

            PropertyInfo valueProperty = typeof(TResponse).GetProperty("Value");
            var valueType = valueProperty.GetValue(response)?.GetType();

            _logger.LogInformation("Request {Request} succeeded with response {Response}", requestTypeName, valueType);
        }
    }
}
