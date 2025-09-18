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
                var httpResponse = await _httpClient.PostAsJsonAsync(endpoint, command, cancellationToken);
                var addEntityResponse = await HandlePostHttpResponseAsync<AddEntityResponse>(httpResponse);
                
                if(addEntityResponse.IsError)
                {
                    return addEntityResponse.Errors;
                }

                return addEntityResponse.Value.Id;
            }
            catch (HttpRequestException)
            {
                return ApiErrors.APICommunication;
            }
        }

        protected async Task<ErrorOr<Success>> HandleNoResponsePostCommandAsync(string endpoint,
                                                                   TRequest command,
                                                                   CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(endpoint, command, cancellationToken);
                return await HandleNoContentHttpResponseAsync(response);
            }
            catch (HttpRequestException)
            {
                return ApiErrors.APICommunication;
            }
        }

        protected async Task<ErrorOr<T>> HandlePostCommandAsync<T>(string endpoint,
                                                                   TRequest command,
                                                                   CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(endpoint, command, cancellationToken);
                return await HandlePostHttpResponseAsync<T>(response);
            }
            catch (HttpRequestException)
            {
                return ApiErrors.APICommunication;
            }
        }

        protected async Task<ErrorOr<Success>> HandleUpdateCommandAsync(string endpoint,
                                                                     TRequest command,
                                                                     CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(endpoint, command, cancellationToken);
                return await HandleNoContentHttpResponseAsync(response);
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

        protected async Task<ErrorOr<Success>> HandleGetCommandAsync(string endpoint,
                                                                CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint, cancellationToken);

                if (response.IsSuccessStatusCode == false)
                {
                    return Error.Failure();
                }

                return Result.Success;
            }
            catch (HttpRequestException)
            {
                return ApiErrors.APICommunication;
            }
        }

    }

}