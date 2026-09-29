using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.System.Monitor.CreateUpdateSignal
{
    public class CreateUpdateSignalCommandHandler : RequestHandler<CreateUpdateSignalCommand, ErrorOr<Success>>
    {
        public CreateUpdateSignalCommandHandler(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }

        public override async Task<ErrorOr<Success>> Handle(CreateUpdateSignalCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleGetCommandAsync("system/monitor/create-update-signal", cancellationToken);
        }
    }
}
