using ErrorOr;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.WorkSessions.CloseWorkSessionPeriodDay
{
    public class CloseWorkSessionPeriodDayCommandHandler :
        RequestHandler<CloseWorkSessionPeriodDayCommand, ErrorOr<Unit>>
    {
        public CloseWorkSessionPeriodDayCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(CloseWorkSessionPeriodDayCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync("worksession/periods/close-day", command, cancellationToken);
       
        }
    }
}
