using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Documents.PayDocuments
{
    public class PayDocumentsCommandHandler :
        RequestHandler<PayDocumentsCommand, ErrorOr<Guid>>
    {
        public PayDocumentsCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(PayDocumentsCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("documents/pay",command, cancellationToken);
        }
    }
}
