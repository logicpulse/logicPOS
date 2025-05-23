using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Customers.DiscountGroups.AddDiscountGroup
{
    public class AddDiscountGroupCommandHandler : RequestHandler<AddDiscountGroupCommand, ErrorOr<Guid>>
    {
        public AddDiscountGroupCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(
            AddDiscountGroupCommand command,
            CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("discountgroups", command, cancellationToken);
        }
    }
}
