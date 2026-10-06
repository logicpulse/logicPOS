using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Database
{
    public class GetDatabaseBackupsQueryHandler :
        RequestHandler<GetDatabaseBackupsQuery, ErrorOr<IEnumerable<DatabaseBackup>>>
    {
        public GetDatabaseBackupsQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<DatabaseBackup>>> Handle(GetDatabaseBackupsQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetListQueryAsync<DatabaseBackup>("database/backups", cancellationToken);
        }
    }
}
