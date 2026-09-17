using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.WorkSessions.Movements.AddCashDrawerInMovement
{
    public class AddCashDrawerInMovementCommandHandler :
        RequestHandler<AddCashDrawerInMovementCommand, ErrorOr<Guid>>
    {
        public AddCashDrawerInMovementCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddCashDrawerInMovementCommand command,
                                                   CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("worksessions/movements/cash-drawer-in", command, cancellationToken);
        }
    }
}
