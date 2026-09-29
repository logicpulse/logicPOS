using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Database
{
    public class BackupDatabaseCommandHandler :
        RequestHandler<BackupDatabaseCommand, ErrorOr<Success>>
    {
        public BackupDatabaseCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Success>> Handle(BackupDatabaseCommand command, CancellationToken cancellationToken = default)
        {
            return  await HandleGetCommandAsync("database/backup", cancellationToken);
        }
    }
}
