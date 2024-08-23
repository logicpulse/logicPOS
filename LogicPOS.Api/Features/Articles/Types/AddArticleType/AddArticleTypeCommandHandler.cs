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

namespace LogicPOS.Api.Features.Articles.Types.AddArticleType
{
    public class AddArticleTypeCommandHandler : RequestHandler<AddArticleTypeCommand, ErrorOr<Guid>>
    {
        public AddArticleTypeCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(
            AddArticleTypeCommand command, 
            CancellationToken cancellationToken = default)
        {
            try {     
                    var httpResponse = await _httpClient.PostAsJsonAsync("article/types", command, cancellationToken);
                    return await HandleAddEntityHttpResponseAsync(httpResponse);
                }

            catch (HttpRequestException)
                {
                    return ApiErrors.CommunicationError;
                }
}
    }
}
