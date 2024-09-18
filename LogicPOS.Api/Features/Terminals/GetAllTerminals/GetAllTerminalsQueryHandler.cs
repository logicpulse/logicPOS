using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Terminals.GetAllTerminals
{
    public class GetAllTerminalsQueryHandler :
        RequestHandler<GetAllTerminalsQuery, ErrorOr<IEnumerable<Terminal>>>
    {
        public GetAllTerminalsQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<Terminal>>> Handle(GetAllTerminalsQuery request, CancellationToken cancellationToken = default)
        {
            return await HandleGetAllQueryAsync<Terminal>("terminals",cancellationToken);
        }
    }
}
