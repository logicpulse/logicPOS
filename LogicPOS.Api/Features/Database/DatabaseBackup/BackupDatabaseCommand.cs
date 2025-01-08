using ErrorOr;
using MediatR;

namespace LogicPOS.Api.Features.Database.DatabaseBackup
{
    public class BackupDatabaseCommand : IRequest<ErrorOr<Unit>>
    {
    }
}
