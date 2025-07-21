using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.SizeUnits.GetAllSizeUnits
{
    public class GetAllSizeUnitsQueryHandler :
        RequestHandler<GetAllSizeUnitsQuery, ErrorOr<IEnumerable<SizeUnit>>>
    {
        public GetAllSizeUnitsQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<SizeUnit>>> Handle(GetAllSizeUnitsQuery query,
                                                                     CancellationToken cancellationToken = default)
        {
            return await HandleGetListQueryAsync<SizeUnit>("articles/sizeunits", cancellationToken);
        }
    }
}
