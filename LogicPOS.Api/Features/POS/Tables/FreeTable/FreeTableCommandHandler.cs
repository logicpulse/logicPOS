using ErrorOr;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Tables.FreeTable
{
    public class FreeTableCommandHandler :
        RequestHandler<FreeTableCommand, ErrorOr<Unit>>
    {
        public FreeTableCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(FreeTableCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"tables/{command.TableId}/free",
                                                  command,
                                                  cancellationToken);
        }
    }
}
