using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.Classes.AddArticleClass
{
    public class AddArticleClassCommandHandler : RequestHandler<AddArticleClassCommand, ErrorOr<Guid>>
    {
        public AddArticleClassCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddArticleClassCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("article/classes", command, cancellationToken);
                return await HandleAddEntityHttpResponseAsync(response);
            }
            catch (HttpRequestException)
            {
                return ApiErrors.CommunicationError;
            }
        }
    }
}
