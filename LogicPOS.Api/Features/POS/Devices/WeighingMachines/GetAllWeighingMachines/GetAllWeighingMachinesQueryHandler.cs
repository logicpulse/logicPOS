using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.WeighingMachines.GetAllWeighingMachines
{
    public class GetAllWeighingMachinesQueryHandler :
        RequestHandler<GetAllWeighingMachinesQuery, ErrorOr<IEnumerable<WeighingMachine>>>
    {
        public GetAllWeighingMachinesQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<WeighingMachine>>> Handle(GetAllWeighingMachinesQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetEntitiesQueryAsync<WeighingMachine>("weighingmachines", cancellationToken);
        }
    }
}
