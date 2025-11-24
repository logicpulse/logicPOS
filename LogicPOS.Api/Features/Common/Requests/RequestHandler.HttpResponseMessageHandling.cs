using ErrorOr;
using LogicPOS.Api.Errors;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Common.Requests
{
    public abstract partial class RequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        protected async Task<ErrorOr<T>> HandlePostHttpResponseAsync<T>(HttpResponseMessage httpResponse)
        {
            switch (httpResponse.StatusCode)
            {
                case HttpStatusCode.Created:
                case HttpStatusCode.OK:
                    var response = await httpResponse.Content.ReadFromJsonAsync<T>();
                    return response;
                case HttpStatusCode.NoContent:
                    return default(T);
                default:
                    return await HandleNotSuccessfulHttpResponseAsync(httpResponse);
            }
        }

        private async Task<Error> HandleNotSuccessfulHttpResponseAsync(HttpResponseMessage httpResponse)
        {
            if (httpResponse.StatusCode == HttpStatusCode.InternalServerError)
            {
                var internalServerError = await httpResponse.Content.ReadFromJsonAsync<InternalServerError>();
                var problem = new ProblemDetails
                {
                    Type = internalServerError.Status.ToString(),
                    Title = "Erro interno do servidor",
                    Status = (int)httpResponse.StatusCode,
                    Detail = internalServerError.Detail,
                    Instance = internalServerError.Instance,
                    TraceId = internalServerError.TraceId,
                    Errors = internalServerError.Errors.Select(x => new ProblemDetailsError
                    {
                        Name = x.Name,
                        Reason = x.Reason
                    }).ToList()
                };
                return Error.Custom(internalServerError.Status,
                                    httpResponse.StatusCode.ToString(),
                                    internalServerError.Detail,
                                    new Dictionary<string, object> { { "problem", problem } });
            }

            var problemDetails = await httpResponse.Content.ReadFromJsonAsync<ProblemDetails>();
            return Error.Validation(metadata: new Dictionary<string, object> { { "problem", problemDetails } });
        }

        private async Task<ErrorOr<Success>> HandleNoContentHttpResponseAsync(HttpResponseMessage httpResponse)
        {
            switch (httpResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.Created:
                case HttpStatusCode.NoContent:
                    return Result.Success;
                default:
                    return await HandleNotSuccessfulHttpResponseAsync(httpResponse);
            }
        }

        protected ErrorOr<bool> HandleDeleteEntityHttpResponse(HttpResponseMessage httpResponse)
        {
            switch (httpResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    return true;
                default:
                    return false;
            }
        }

    }
}
