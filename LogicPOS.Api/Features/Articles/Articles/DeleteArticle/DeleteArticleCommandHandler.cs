using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.DeleteArticle
{
    public class DeleteArticleCommandHandler :
        RequestHandler<DeleteArticleCommand, ErrorOr<bool>>
    {
        public DeleteArticleCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<bool>> Handle(DeleteArticleCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"articles/{command.Id}", cancellationToken);
        }
    }

}
