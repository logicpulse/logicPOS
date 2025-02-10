using ErrorOr;
using LogicPOS.Api.Features.Common;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Warehouses.AddWarehouse
{
    public class AddWarehouseCommandHandler :
        RequestHandler<AddWarehouseCommand, ErrorOr<Guid>>
    {
        public AddWarehouseCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddWarehouseCommand command,
                                                         CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("articles/stocks/warehouses", command, cancellationToken);
        }
    }
}
