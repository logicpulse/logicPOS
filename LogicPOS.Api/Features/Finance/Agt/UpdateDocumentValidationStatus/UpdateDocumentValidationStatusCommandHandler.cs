using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.Documents.Documents;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Agt.UpdateDocumentValidationStatus
{
    public class UpdateDocumentValidationStatusCommandHandler : RequestHandler<UpdateDocumentValidationStatusCommand, ErrorOr<Success>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;

        public UpdateDocumentValidationStatusCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }


        public override async Task<ErrorOr<Success>> Handle(UpdateDocumentValidationStatusCommand request, CancellationToken cancellationToken = default)
        {
            var result = await HandleUpdateCommandAsync($"agt/documents/{request.DocumentId}/validation-status", request, cancellationToken);

            if (result.IsError == false)
            {
                DocumentsCache.Clear(_keyedMemoryCache);
            }

            return result;
        }
    }
}
