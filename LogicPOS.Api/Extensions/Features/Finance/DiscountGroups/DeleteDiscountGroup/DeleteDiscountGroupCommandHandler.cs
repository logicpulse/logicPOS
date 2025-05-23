using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.DiscountGroups.DeleteDiscountGroup
{
    public class DeleteDiscountGroupCommandHandler :
        RequestHandler<DeleteDiscountGroupCommand, ErrorOr<bool>>
    {
        public DeleteDiscountGroupCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<bool>> Handle(DeleteDiscountGroupCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"discountgroups/{command.Id}", cancellationToken);
        }
    }
}
