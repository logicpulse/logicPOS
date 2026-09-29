using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.PoleDisplays.DeletePoleDisplay
{
    public class DeletePoleDisplayCommandHandler :
        RequestHandler<DeletePoleDisplayCommand, ErrorOr<bool>>
    {
        public DeletePoleDisplayCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<bool>> Handle(DeletePoleDisplayCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"poledisplays/{command.Id}", cancellationToken);
        }
    }
}
