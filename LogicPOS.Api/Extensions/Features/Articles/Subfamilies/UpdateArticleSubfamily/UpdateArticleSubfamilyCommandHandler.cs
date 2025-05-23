using ErrorOr;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.Subfamilies.UpdateArticleSubfamily
{
    public class UpdateArticleSubfamilyCommandHandler :
        RequestHandler<UpdateArticleSubfamilyCommand, ErrorOr<Unit>>
    {
        public UpdateArticleSubfamilyCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(UpdateArticleSubfamilyCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"articles/subfamilies/{command.Id}", command, cancellationToken);
        }
    }
}
