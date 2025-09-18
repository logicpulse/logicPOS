using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.Types.UpdateArticleType
{
    public class UpdateArticleTypeCommandHandler :
        RequestHandler<UpdateArticleTypeCommand, ErrorOr<Success>>
    {
        public UpdateArticleTypeCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Success>> Handle(UpdateArticleTypeCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"/articles/types/{command.Id}", command, cancellationToken);
        }
    }
}
