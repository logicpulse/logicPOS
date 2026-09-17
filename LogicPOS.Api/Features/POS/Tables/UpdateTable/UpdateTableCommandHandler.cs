using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Tables.UpdateTable
{
    public class UpdateTableCommandHandler
         : RequestHandler<UpdateTableCommand, ErrorOr<Success>>
    {
        public UpdateTableCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Success>> Handle(UpdateTableCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"tables/{command.Id}", command, cancellationToken);
        }
    }
}
