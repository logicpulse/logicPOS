using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Database.GetBackup
{
    public class GetSqliteBackupQueryHandler :
        RequestHandler<GetSqliteBackupQuery, ErrorOr<string>>
    {
        public GetSqliteBackupQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<string>> Handle(GetSqliteBackupQuery request, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync("database/sqlite/backup", cancellationToken);
        }
    }
}
