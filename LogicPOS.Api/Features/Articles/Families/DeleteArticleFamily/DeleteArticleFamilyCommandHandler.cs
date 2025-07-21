using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.Families.DeleteArticleFamily
{
    public class DeleteArticleFamilyCommandHandler :
        RequestHandler<DeleteArticleFamilyCommand, ErrorOr<bool>>
    {
        public DeleteArticleFamilyCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<bool>> Handle(DeleteArticleFamilyCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"articles/families/{command.Id}", cancellationToken);
        }
    }

}
