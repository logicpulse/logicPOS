using ErrorOr;
using LogicPOS.Api.Features.Common;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Documents.AddDocument
{
    public class AddDocumentCommandHandler :
        RequestHandler<AddDocumentCommand, ErrorOr<Guid>>
    {
        public AddDocumentCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddDocumentCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("documents",command, cancellationToken);
        }
    }
}
