using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Customers.DiscountGroups.UpdateDiscountGroup
{
    public class UpdateDiscountGroupCommandHandler :
        RequestHandler<UpdateDiscountGroupCommand, ErrorOr<Unit>>
    {
        public UpdateDiscountGroupCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(UpdateDiscountGroupCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommand($"discountgroups/{command.Id}", command, cancellationToken);
        }
    }
}
