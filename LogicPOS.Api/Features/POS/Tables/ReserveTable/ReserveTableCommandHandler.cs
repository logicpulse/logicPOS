using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Tables.ReserveTable
{
    public class ReserveTableCommandHandler :
        RequestHandler<ReserveTableCommand, ErrorOr<Unit>>
    {
        public ReserveTableCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(ReserveTableCommand command,
                                                         CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"tables/{command.TableId}/reserve",
                                                  command,
                                                  cancellationToken);
        }
    }
}
