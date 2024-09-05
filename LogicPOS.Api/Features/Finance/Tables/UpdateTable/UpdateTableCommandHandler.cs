using ErrorOr;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Tables.UpdateTable
{
    public class UpdateTableCommandHandler
         : RequestHandler<UpdateTableCommand, ErrorOr<Unit>>
    {
        public UpdateTableCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(UpdateTableCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommand($"tables/{command.Id}", command, cancellationToken);
        }
    }
}
