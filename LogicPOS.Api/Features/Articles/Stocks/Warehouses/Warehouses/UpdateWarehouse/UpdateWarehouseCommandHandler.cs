using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Warehouses.UpdateWarehouse
{
    public class UpdateWarehouseCommandHandler :
        RequestHandler<UpdateWarehouseCommand, ErrorOr<Success>>
    {
        public UpdateWarehouseCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Success>> Handle(UpdateWarehouseCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"articles/stocks/warehouses/{command.Id}", command, cancellationToken);
        }
    }
}
