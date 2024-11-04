using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Terminals.GetTerminalByHardwareId
{
    public class GetTerminalByHardwareIdQueryHandler :
        RequestHandler<GetTerminalByHardwareIdQuery, ErrorOr<Terminal>>
    {
        public GetTerminalByHardwareIdQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<Terminal>> Handle(GetTerminalByHardwareIdQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetEntityQueryAsync<Terminal>($"terminals/hardwareid/{query.HardwareId}", cancellationToken);
        }
    }
}
