using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Warehouses.DeleteWarehouse
{
    public class DeleteWarehouseCommandHandler :
        RequestHandler<DeleteWarehouseCommand, ErrorOr<bool>>
    {
        public DeleteWarehouseCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<bool>> Handle(DeleteWarehouseCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"articles/stocks/warehouses/{command.Id}", cancellationToken);
        }
    }
}
