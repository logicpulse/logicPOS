using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.WorkSessions.Movements.AddCashDrawerOutMovement
{
    public class AddCashDrawerOutMovementCommandHandler :
        RequestHandler<AddCashDrawerOutMovementCommand, ErrorOr<Guid>>
    {
        public AddCashDrawerOutMovementCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<Guid>> Handle(AddCashDrawerOutMovementCommand command,
                                                   CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("worksessions/movements/cash-drawer-out", command, cancellationToken);
        }
    }
}
