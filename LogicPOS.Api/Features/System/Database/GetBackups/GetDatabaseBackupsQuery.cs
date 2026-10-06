using ErrorOr;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Database
{
    public class GetDatabaseBackupsQuery : IRequest<ErrorOr<IEnumerable<DatabaseBackup>>>
    {
    }
}
