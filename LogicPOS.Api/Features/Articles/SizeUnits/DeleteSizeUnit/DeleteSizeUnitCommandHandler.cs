using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.SizeUnits.DeleteSizeUnit
{
    public class DeleteSizeUnitCommandHandler :
        RequestHandler<DeleteSizeUnitCommand, ErrorOr<bool>>
    {
        public DeleteSizeUnitCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<bool>> Handle(DeleteSizeUnitCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"articles/sizeunits/{command.Id}", cancellationToken);
        }
    }
}
