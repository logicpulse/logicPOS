using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Documents.Series.UpdateDocumentSerie
{
    public class UpdateDocumentSeriesCommandHandler :
        RequestHandler<UpdateDocumentSeriesCommand, ErrorOr<Success>>
    {
        public UpdateDocumentSeriesCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Success>> Handle(UpdateDocumentSeriesCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"documents/series/{command.Id}", command, cancellationToken);
        }
    }
}
