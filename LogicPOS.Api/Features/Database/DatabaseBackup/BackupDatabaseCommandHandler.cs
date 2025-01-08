using ErrorOr;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Database.DatabaseBackup
{
    public class BackupDatabaseCommandHandler :
        RequestHandler<BackupDatabaseCommand, ErrorOr<Unit>>
    {
        public BackupDatabaseCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(BackupDatabaseCommand command, CancellationToken cancellationToken = default)
        {
            return  await HandleGetCommandAsync("database/backup", cancellationToken);
        }
    }
}
