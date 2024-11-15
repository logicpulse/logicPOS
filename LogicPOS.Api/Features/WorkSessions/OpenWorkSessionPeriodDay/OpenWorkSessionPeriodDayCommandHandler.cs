using ErrorOr;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.WorkSessions.OpenWorkSessionPeriodDay
{
    public class OpenWorkSessionPeriodDayCommandHandler :
        RequestHandler<OpenWorkSessionPeriodDayCommand, ErrorOr<Unit>>
    {
        public OpenWorkSessionPeriodDayCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(OpenWorkSessionPeriodDayCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync("worksession/periods/open-day", command, cancellationToken);
       
        }
    }
}
