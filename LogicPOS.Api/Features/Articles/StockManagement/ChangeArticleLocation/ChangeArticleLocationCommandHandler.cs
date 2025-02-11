using ErrorOr;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.StockManagement.ChangeArticleLocation
{
    public class ChangeArticleLocationCommandHandler :
        RequestHandler<ChangeArticleLocationCommand, ErrorOr<Unit>>
    {
        public ChangeArticleLocationCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(ChangeArticleLocationCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"articles/stocks/{command.WarehouseArticleId}/location", command, cancellationToken);
        }
    }
}
