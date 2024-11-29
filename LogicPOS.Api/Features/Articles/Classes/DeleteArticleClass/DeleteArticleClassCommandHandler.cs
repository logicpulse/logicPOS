using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.Classes.DeleteArticleClass
{
    public class DeleteArticleClassCommandHandler : RequestHandler<DeleteArticleClassCommand, ErrorOr<bool>>
    {
        public DeleteArticleClassCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<bool>> Handle(DeleteArticleClassCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"articles/classes/{command.Id}", cancellationToken);
        }
    }

}
