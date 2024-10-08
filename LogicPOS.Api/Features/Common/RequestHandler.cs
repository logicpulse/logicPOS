﻿using ErrorOr;
using LogicPOS.Api.Errors;
using MediatR;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Common
{
    public abstract class RequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        protected readonly HttpClient _httpClient;
        public RequestHandler(IHttpClientFactory factory) => _httpClient = factory.CreateClient("Default");

        public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);

        protected async Task<ErrorOr<IEnumerable<TEntity>>> HandleGetAllQueryAsync<TEntity>(
            string endpoint,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var items = await _httpClient.GetFromJsonAsync<List<TEntity>>(endpoint, cancellationToken);
                return items;
            }
            catch (HttpRequestException)
            {
                return ApiErrors.CommunicationError;
            }
        }

        protected async Task<ErrorOr<TEntity>> HandleGetByIdQueryAsync<TEntity>(
           string endpoint,
           CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _httpClient.GetFromJsonAsync<TEntity>(endpoint, cancellationToken);
                return entity;
            }
            catch (HttpRequestException)
            {
                return ApiErrors.CommunicationError;
            }
        }

        protected async Task<ErrorOr<Guid>> HandleAddCommandAsync(
            string endpoint,
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
                return ApiErrors.CommunicationError;
            }
        }

        protected async Task<ErrorOr<Unit>> HandleUpdateCommandAsync(
            string endpoint,
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
                return ApiErrors.CommunicationError;
            }
        }

        protected async Task<ErrorOr<Unit>> HandleDeleteCommandAsync(string endpoint, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(endpoint, cancellationToken);
                return await HandleDeleteEntityHttpResponseAsync(response);
            }
            catch (HttpRequestException)
            {
                return ApiErrors.CommunicationError;
            }
        }

        protected async Task<ErrorOr<Guid>> HandleAddEntityHttpResponseAsync(HttpResponseMessage httpResponse)
        {
            switch (httpResponse.StatusCode)
            {
                case HttpStatusCode.Created:
                    var response = await httpResponse.Content.ReadFromJsonAsync<AddEntityResponse>();
                    return response.Id;
                case HttpStatusCode.BadRequest:
                    return await GetProblemDetailsErrorAsync(httpResponse);
                default:
                    return ApiErrors.CommunicationError;
            }
        }

        private async Task<Error> GetProblemDetailsErrorAsync(HttpResponseMessage httpResponse)
        {
            var problemDetails = await httpResponse.Content.ReadFromJsonAsync<ProblemDetails>();
            return Error.Validation(metadata: new Dictionary<string, object> { { "problem", problemDetails } });
        }

        protected async Task<ErrorOr<Unit>> HandleHttpResponseAsync(HttpResponseMessage httpResponse)
        {
            switch (httpResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.Created:
                case HttpStatusCode.NoContent:
                    return Unit.Value;
                case HttpStatusCode.BadRequest:
                    return await GetProblemDetailsErrorAsync(httpResponse);
                default:
                    return ApiErrors.CommunicationError;
            }
        }

        protected async Task<ErrorOr<Unit>> HandleDeleteEntityHttpResponseAsync(HttpResponseMessage httpResponse)
        {
            switch (httpResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    return Unit.Value;
                case HttpStatusCode.BadRequest:
                    return await GetProblemDetailsErrorAsync(httpResponse);
                default:
                    return ApiErrors.CommunicationError;
            }
        }
    }
}
