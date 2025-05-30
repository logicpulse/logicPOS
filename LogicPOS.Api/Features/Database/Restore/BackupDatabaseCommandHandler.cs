using ErrorOr;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Database
{
    public class RestoreDatabaseCommandHandler :
        RequestHandler<RestoreDatabaseCommand, ErrorOr<Unit>>
    {
        public RestoreDatabaseCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(RestoreDatabaseCommand command, CancellationToken cancellationToken = default)
        {
            return  await HandleGetCommandAsync("database/restore", cancellationToken);
        }
    }
}
