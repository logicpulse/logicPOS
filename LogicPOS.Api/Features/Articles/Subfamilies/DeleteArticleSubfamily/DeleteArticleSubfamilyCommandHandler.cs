using ErrorOr;
using LogicPOS.Api.Features.Common;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.Subfamilies.DeleteArticleSubfamily
{
    public class DeleteArticleSubfamilyCommandHandler :
        RequestHandler<DeleteArticleSubfamilyCommand, ErrorOr<bool>>
    {
        public DeleteArticleSubfamilyCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<bool>> Handle(DeleteArticleSubfamilyCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"articles/subfamilies/{command.Id}", cancellationToken);
        }
    }
}
