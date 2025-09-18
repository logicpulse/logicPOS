using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Tables.FreeTable
{
    public class FreeTableCommandHandler :
        RequestHandler<FreeTableCommand, ErrorOr<Success>>
    {
        public FreeTableCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Success>> Handle(FreeTableCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"tables/{command.TableId}/free",
                                                  command,
                                                  cancellationToken);
        }
    }
}
