using ErrorOr;
using LogicPOS.Api.Features.Common;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Terminals.AddTerminal
{
    public class AddTerminalCommandHandler : 
        RequestHandler<AddTerminalCommand, ErrorOr<Guid>>
    {
        public AddTerminalCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddTerminalCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("terminals", command, cancellationToken);
        }
    }
}
