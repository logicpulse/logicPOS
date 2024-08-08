﻿using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Countries.AddCountry
{
    public class AddCountryCommandHandler : IRequestHandler<AddCountryCommand, ErrorOr<Guid>>
    {
        private readonly HttpClient _httpClient;


        public AddCountryCommandHandler(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("Default");
        }

        public async Task<ErrorOr<Guid>> Handle(
            AddCountryCommand command,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var httpResponse = await _httpClient.PostAsJsonAsync("countries",
                                                                     command,
                                                                     cancellationToken);
                var response = await httpResponse.Content.ReadAsStringAsync();

                return await HandleResponseAsync(httpResponse);

            }
            catch (HttpRequestException)
            {
                return Error.Unexpected(description: "Erro ao se comunicar com a API.");
            }
        }

        private async Task<ErrorOr<Guid>> HandleResponseAsync(HttpResponseMessage httpResponse)
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
                    return Error.Unexpected();
            }
        }
    }
}