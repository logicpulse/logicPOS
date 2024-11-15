using ErrorOr;
using MediatR;

namespace LogicPOS.Api.Features.WorkSessions.CloseWorkSessionPeriodDay
{
    public class CloseWorkSessionPeriodDayCommand : IRequest<ErrorOr<Unit>>
    {
        public string Notes { get; set; }
    }
}
