using ErrorOr;
using MediatR;

namespace LogicPOS.Api.Features.Database
{
    public class RestoreDatabaseCommand : IRequest<ErrorOr<Unit>>
    {
    }
}
