using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.MovementTypes.GetAllMovementTypes
{
    public class GetAllMovementTypesQueryHandler :
        RequestHandler<GetAllMovementTypesQuery, ErrorOr<IEnumerable<MovementType>>>
    {
        public GetAllMovementTypesQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<MovementType>>> Handle(GetAllMovementTypesQuery request,
                                                                              CancellationToken cancellationToken = default)
        {
            return await HandleGetListQueryAsync<MovementType>("movementtypes", cancellationToken);
        }
    }
}
