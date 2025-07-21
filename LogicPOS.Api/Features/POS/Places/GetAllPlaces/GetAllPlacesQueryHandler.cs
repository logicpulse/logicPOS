using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Places.GetAllPlaces
{
    public class GetAllPlacesQueryHandler :
        RequestHandler<GetAllPlacesQuery, ErrorOr<IEnumerable<Place>>>
    {
        public GetAllPlacesQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<Place>>> Handle(GetAllPlacesQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetListQueryAsync<Place>("places", cancellationToken);
        }
    }
}
