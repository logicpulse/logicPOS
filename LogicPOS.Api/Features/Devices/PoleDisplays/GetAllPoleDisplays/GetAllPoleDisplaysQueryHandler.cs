using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.PoleDisplays.GetAllPoleDisplays
{
    public class GetAllPoleDisplaysQueryHandler :
        RequestHandler<GetAllPoleDisplaysQuery, ErrorOr<IEnumerable<PoleDisplay>>>
    {
        public GetAllPoleDisplaysQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<PoleDisplay>>> Handle(GetAllPoleDisplaysQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetAllQueryAsync<PoleDisplay>("poledisplays", cancellationToken);
        }
    }
}
