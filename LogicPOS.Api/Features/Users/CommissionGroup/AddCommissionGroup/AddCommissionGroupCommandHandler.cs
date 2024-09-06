using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.CommissionGroups.AddCommissionGroup
{
    public class AddCommissionGroupCommandHandler :
        RequestHandler<AddCommissionGroupCommand, ErrorOr<Guid>>
    {
        public AddCommissionGroupCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddCommissionGroupCommand command,
                                                                CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("users/commission-groups", command, cancellationToken);
        }
    }
}
