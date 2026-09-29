using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Terminals.GetTerminalById
{
    public class GetTerminalByIdQueryHandler :
        RequestHandler<GetTerminalByIdQuery, ErrorOr<Terminal>>
    {
        public GetTerminalByIdQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Terminal>> Handle(GetTerminalByIdQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetQueryAsync<Terminal>($"terminals/{query.Id}", cancellationToken);
        }
    }
}
