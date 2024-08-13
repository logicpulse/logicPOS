using ErrorOr;
using LogicPOS.Api.Errors;
using MediatR;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Common
{
    public abstract class RequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        protected readonly HttpClient _httpClient;
        public RequestHandler(IHttpClientFactory factory) => _httpClient = factory.CreateClient("Default");

        public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);

        protected async Task<ErrorOr<Guid>> HandleHttpResponseAsync(HttpResponseMessage httpResponse)
        {
            switch (httpResponse.StatusCode)
            {
                case HttpStatusCode.Created:
                    var response = await httpResponse.Content.ReadFromJsonAsync<AddEntityResponse>();
                    return response.Id;
                case HttpStatusCode.BadRequest:
                    var problemDetails = await httpResponse.Content.ReadFromJsonAsync<ProblemDetails>();
                    return Error.Validation(metadata: new Dictionary<string, object> { { "problem", problemDetails } });
                default:
                    return ApiErrors.CommunicationError;
            }
        }
    }
}
