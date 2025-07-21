using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
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
            return await HandleUpdateCommandAsync($"documents/series/{command.Id}", command, cancellationToken);
        }
    }
}
