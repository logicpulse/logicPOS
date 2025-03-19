using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.WorkSessions.Periods.TerminalIsOpen
{
    public class TerminalIsOpenQueryHandler :
        RequestHandler<TerminalIsOpenQuery, ErrorOr<bool>>
    {
        public TerminalIsOpenQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<bool>> Handle(TerminalIsOpenQuery request, CancellationToken cancellationToken = default)
        {
            return await HandleGetEntityQueryAsync<bool>($"worksessions/periods/terminal-is-open?terminalId={request.TerminalId}", cancellationToken);
        }
    }
}
