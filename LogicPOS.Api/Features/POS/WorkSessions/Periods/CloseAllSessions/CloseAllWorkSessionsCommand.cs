using ErrorOr;
using MediatR;

namespace LogicPOS.Api.Features.WorkSessions.CloseAllSessions
{
    public class CloseAllWorkSessionsCommand : IRequest<ErrorOr<Success>>
    {
    }
}
