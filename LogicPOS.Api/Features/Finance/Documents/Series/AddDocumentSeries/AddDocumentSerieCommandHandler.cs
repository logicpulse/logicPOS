using ErrorOr;
using LogicPOS.Api.Features.Common;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Documents.Series.AddDocumentSerie
{
    public class AddDocumentSerieCommandHandler : 
        RequestHandler<AddDocumentSerieCommand, ErrorOr<Guid>>
    {
        public AddDocumentSerieCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddDocumentSerieCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("documents/series", command, cancellationToken);
        }
    }
}
