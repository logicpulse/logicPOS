using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Database
{
    public class RestoreDatabaseCommandHandler :
        RequestHandler<RestoreDatabaseCommand, ErrorOr<Success>>
    {
        public RestoreDatabaseCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Success>> Handle(RestoreDatabaseCommand command, CancellationToken cancellationToken = default)
        {
            return  await HandleGetCommandAsync("database/restore", cancellationToken);
        }
    }
}
