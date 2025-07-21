using ErrorOr;
using LogicPOS.Api.Errors;
using MediatR;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Common.Requests
{
    public abstract partial class RequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {

        protected async Task<ErrorOr<Guid>> HandleAddCommandAsync(string endpoint,
                                                                  TRequest command,
                                                                  CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(endpoint, command, cancellationToken);
                return await HandleAddEntityHttpResponseAsync(response);
            }
            catch (HttpRequestException)
            {
                return ApiErrors.APICommunication;
            }
        }

        protected async Task<ErrorOr<Unit>> HandlePostCommandAsync(string endpoint,
                                                                   TRequest command,
                                                                   CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(endpoint, command, cancellationToken);
                return await HandleHttpResponseAsync(response);
            }
            catch (HttpRequestException)
            {
                return ApiErrors.APICommunication;
            }
        }

        protected async Task<ErrorOr<Unit>> HandleUpdateCommandAsync(string endpoint,
                                                                     TRequest command,
                                                                     CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(endpoint, command, cancellationToken);
                return await HandleHttpResponseAsync(response);
            }
            catch (HttpRequestException)
            {
                return ApiErrors.APICommunication;
            }
        }

        protected async Task<ErrorOr<bool>> HandleDeleteCommandAsync(string endpoint, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(endpoint, cancellationToken);
                return HandleDeleteEntityHttpResponse(response);
            }
            catch (HttpRequestException)
            {
                return ApiErrors.APICommunication;
            }
        }


    }

}