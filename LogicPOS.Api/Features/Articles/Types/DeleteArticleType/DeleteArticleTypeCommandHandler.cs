using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.Types.DeleteArticleType
{
    public class DeleteArticleTypeCommandHandler :
        RequestHandler<DeleteArticleTypeCommand, ErrorOr<bool>>
    {
        public DeleteArticleTypeCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<bool>> Handle(DeleteArticleTypeCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"articles/types/{command.Id}", cancellationToken);
        }
    }
}
