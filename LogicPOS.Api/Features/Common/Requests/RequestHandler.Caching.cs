using ErrorOr;
using LogicPOS.Api.Errors;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Common.Requests
{
    public abstract partial class RequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private bool UseCache(MemoryCacheEntryOptions options) => _cache != null && options != null;

        protected async Task<ErrorOr<IEnumerable<TEntity>>> HandleGetListQueryAsync<TEntity>(string endpoint,
                                                                                             CancellationToken cancellationToken = default,
                                                                                             MemoryCacheEntryOptions cacheOptions = null)
        {
            var result = await HandleGetQueryAsync<IEnumerable<TEntity>>(endpoint, cancellationToken, cacheOptions);
            
            if(result.IsError == false && result.Value == null)
            {
                return Enumerable.Empty<TEntity>().ToList();
            }

            return result;
        }

        protected async Task<ErrorOr<TEntity>> HandleGetQueryAsync<TEntity>(string endpoint,
                                                                                  CancellationToken cancellationToken = default,
                                                                                  MemoryCacheEntryOptions cacheOptions = null)
        {
            bool useCache = UseCache(cacheOptions);

            if (useCache && _cache.TryGetValue(endpoint, out TEntity entity))
            {
                return entity;
            }

            try
            {
                var response = await _httpClient.GetAsync(endpoint, cancellationToken);

                if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.NoContent)
                {
                    return default(TEntity);
                }

                if (response.IsSuccessStatusCode == false)
                {
                    return await HandleNotSuccessfulHttpResponseAsync(response);
                }

                entity = await response.Content.ReadFromJsonAsync<TEntity>(cancellationToken);

                if (useCache)
                {
                    _cache.Set(endpoint, entity, cacheOptions);
                }

                return entity;
            }
            catch (HttpRequestException)
            {
                return ApiErrors.APICommunication;
            }

        }

    }
}