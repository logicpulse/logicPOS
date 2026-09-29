using ErrorOr;
using MediatR;

namespace LogicPOS.Api.Features.WorkSessions.Periods.DayIsOpen
{
    public class DayIsOpenQuery : IRequest<ErrorOr<bool>>
    {

    }
}
