using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Tables.DeleteTable
{
    public class DeleteTableCommandHandler :
        RequestHandler<DeleteTableCommand, ErrorOr<bool>>
    {
        public DeleteTableCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<bool>> Handle(DeleteTableCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"tables/{command.Id}", cancellationToken);
        }
    }
}
