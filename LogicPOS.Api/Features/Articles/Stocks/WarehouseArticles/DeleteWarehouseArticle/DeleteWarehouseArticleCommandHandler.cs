using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.Stocks.WarehouseArticles.DeleteWarehouseArticle
{
    public class DeleteWarehouseArticleCommandHandler :
        RequestHandler<DeleteWarehouseArticleCommand, ErrorOr<bool>>
    {
        public DeleteWarehouseArticleCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<bool>> Handle(DeleteWarehouseArticleCommand command,
                                                         CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"articles/warehouse-articles/{command.Id}", cancellationToken);
        }
    }
}
