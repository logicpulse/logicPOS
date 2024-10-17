using ErrorOr;
using MediatR;

namespace LogicPOS.Api.Features.Database.GetBackup
{
    public class GetSqliteBackupQuery : IRequest<ErrorOr<string>>
    {

    }
}
