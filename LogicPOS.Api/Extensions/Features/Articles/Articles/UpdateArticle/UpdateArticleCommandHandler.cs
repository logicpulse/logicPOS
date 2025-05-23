using ErrorOr;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.UpdateArticle
{
    public class UpdateArticleCommandHandler :
        RequestHandler<UpdateArticleCommand, ErrorOr<Unit>>
    {
        public UpdateArticleCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(UpdateArticleCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"articles/{command.Id}", command, cancellationToken);
        }
    }
}
