using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.Agt.RegisterDocument;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Agt.UpdateDocumentValidationStatus
{
    public class UpdateDocumentValidationStatusCommandHandler : RequestHandler<UpdateDocumentValidationStatusCommand, ErrorOr<Success>>
    {
        public UpdateDocumentValidationStatusCommandHandler(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }

        public override async Task<ErrorOr<Success>> Handle(UpdateDocumentValidationStatusCommand request, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"agt/documents/{request.DocumentId}/validation-status", request, cancellationToken);
        }
    }
}
