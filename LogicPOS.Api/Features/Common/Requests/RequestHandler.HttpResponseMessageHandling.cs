using ErrorOr;
using LogicPOS.Api.Errors;
using MediatR;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Common.Requests
{
    public abstract partial class RequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        protected async Task<ErrorOr<Guid>> HandleAddEntityHttpResponseAsync(HttpResponseMessage httpResponse)
        {
            switch (httpResponse.StatusCode)
            {
                case HttpStatusCode.Created:
                    var response = await httpResponse.Content.ReadFromJsonAsync<AddEntityResponse>();
                    return response.Id;
                case HttpStatusCode.BadRequest:
                    return await GetProblemDetailsFromResponseAsync(httpResponse);
                default:
                    return ApiErrors.UnknownAPIResponse;
            }
        }

        private async Task<Error> GetProblemDetailsFromResponseAsync(HttpResponseMessage httpResponse)
        {
            var problemDetails = await httpResponse.Content.ReadFromJsonAsync<ProblemDetails>();
            return Error.Validation(metadata: new Dictionary<string, object> { { "problem", problemDetails } });
        }

        private async Task<ErrorOr<Unit>> HandleHttpResponseAsync(HttpResponseMessage httpResponse)
        {
            switch (httpResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.Created:
                case HttpStatusCode.NoContent:
                    return Unit.Value;
                case HttpStatusCode.BadRequest:
                    return await GetProblemDetailsFromResponseAsync(httpResponse);
                default:
                    return ApiErrors.UnknownAPIResponse;
            }
        }

        protected ErrorOr<bool> HandleDeleteEntityHttpResponse(HttpResponseMessage httpResponse)
        {
            switch (httpResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    return true;
                case HttpStatusCode.BadRequest:
                    return false;
                case HttpStatusCode.NotFound:
                    return false;
                default:
                    return ApiErrors.UnknownAPIResponse;
            }
        }

    }
}
