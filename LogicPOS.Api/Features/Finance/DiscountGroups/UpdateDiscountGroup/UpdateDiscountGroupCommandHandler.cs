using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Customers.DiscountGroups.UpdateDiscountGroup
{
    public class UpdateDiscountGroupCommandHandler :
        RequestHandler<UpdateDiscountGroupCommand, ErrorOr<Success>>
    {
        public UpdateDiscountGroupCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Success>> Handle(UpdateDiscountGroupCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"discountgroups/{command.Id}", command, cancellationToken);
        }
    }
}
