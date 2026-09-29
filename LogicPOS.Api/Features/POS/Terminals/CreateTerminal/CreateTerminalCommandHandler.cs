using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Terminals.CreateTerminal
{
    public class CreateTerminalCommandHandler : 
        RequestHandler<CreateTerminalCommand, ErrorOr<Guid>>
    {
        public CreateTerminalCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(CreateTerminalCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("terminals/create", command, cancellationToken);
        }
    }
}
