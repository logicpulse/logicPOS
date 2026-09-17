using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Authentication;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Common.Requests
{
    public abstract partial class RequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        protected readonly HttpClient _httpClient;
        protected  IMemoryCache _cache;

        public RequestHandler(IHttpClientFactory httpFactory)
        {
            _httpClient = httpFactory.CreateClient("Default");
            if (AuthenticationData.Token != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new global::System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AuthenticationData.Token);
            }
        }

        public RequestHandler(IHttpClientFactory httpFactory, IMemoryCache cache) : this(httpFactory) 
        { 
            _cache = cache;
        }

        public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);

       
    }
}
