using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.Documents.Documents;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Agt.UpdateDocumentsValidationStatus
{
    public class UpdateDocumentsValidationStatusCommandHandler : RequestHandler<UpdateDocumentsValidationStatusCommand, ErrorOr<Success>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;

        public UpdateDocumentsValidationStatusCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }


        public override async Task<ErrorOr<Success>> Handle(UpdateDocumentsValidationStatusCommand request, CancellationToken cancellationToken = default)
        {
            var result = await HandleUpdateCommandAsync($"agt/fe/documents/validation-status/bulk", request, cancellationToken);

            if (result.IsError == false)
            {
                DocumentsCache.Clear(_keyedMemoryCache);
                ReceiptsCache.Clear(_keyedMemoryCache);
            }

            return result;
        }
    }
}
