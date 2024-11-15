using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;

namespace LogicPOS.Api.Features.WorkSessions.GetLastWorkSessionPeriod
{
    public class GetLastWorkSessionPeriodQuery : IRequest<ErrorOr<WorkSessionPeriod>>
    {
        public bool TermnialSession { get; set; }

        public GetLastWorkSessionPeriodQuery(bool termnialSession = false)
        {
            TermnialSession = termnialSession;
        }
    }
}
