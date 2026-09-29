using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Users.CommissionGroups.DeleteCommissionGroup
{
    public class DeleteCommissionGroupCommandHandler :
        RequestHandler<DeleteCommissionGroupCommand, ErrorOr<bool>>
    {
        public DeleteCommissionGroupCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<bool>> Handle(DeleteCommissionGroupCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"commission-groups/{command.Id}", cancellationToken);
        }
    }
}
