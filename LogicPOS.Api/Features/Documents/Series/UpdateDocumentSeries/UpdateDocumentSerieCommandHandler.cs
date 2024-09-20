using ErrorOr;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Documents.Series.UpdateDocumentSerie
{
    public class UpdateDocumentSerieCommandHandler :
        RequestHandler<UpdateDocumentSerieCommand, ErrorOr<Unit>>
    {
        public UpdateDocumentSerieCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(UpdateDocumentSerieCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"document/series/{command.Id}", command, cancellationToken);
        }
    }
}
