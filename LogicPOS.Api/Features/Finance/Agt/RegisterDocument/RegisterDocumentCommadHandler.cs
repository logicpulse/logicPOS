using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Agt.RegisterDocument
{
    public class RegisterDocumentCommadHandler : RequestHandler<RegisterDocumentCommand, ErrorOr<Guid>>
    {
        public RegisterDocumentCommadHandler(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(RegisterDocumentCommand request, CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("agt/documents", request, cancellationToken);
        }
    }
}
