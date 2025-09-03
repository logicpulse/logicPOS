using ErrorOr;
using MediatR;

namespace LogicPOS.Api.Features.Database
{
    public class BackupDatabaseCommand : IRequest<ErrorOr<Unit>>
    {
    }
}
