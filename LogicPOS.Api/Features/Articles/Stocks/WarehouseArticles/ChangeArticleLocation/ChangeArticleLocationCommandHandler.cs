using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.StockManagement.ChangeArticleLocation
{
    public class ChangeArticleLocationCommandHandler :
        RequestHandler<ChangeArticleLocationCommand, ErrorOr<Success>>
    {
        public ChangeArticleLocationCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Success>> Handle(ChangeArticleLocationCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"articles/stocks/{command.WarehouseArticleId}/location", command, cancellationToken);
        }
    }
}
