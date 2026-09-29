using ErrorOr;
using MediatR;

namespace LogicPOS.Api.Features.WorkSessions.CloseWorkSessionPeriodDay
{
    public class CloseWorkSessionPeriodDayCommand : IRequest<ErrorOr<Success>>
    {
        public string Notes { get; set; }
    }
}
