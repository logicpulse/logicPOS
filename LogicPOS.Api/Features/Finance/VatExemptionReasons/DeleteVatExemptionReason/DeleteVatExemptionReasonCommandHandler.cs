using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.VatExemptionReasons;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.VatExcemptionReasons.DeleteVatExcemptionReason
{
    public class DeleteVatExemptionReasonCommandHandler :
        RequestHandler<DeleteVatExemptionReasonCommand, ErrorOr<bool>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;
        public DeleteVatExemptionReasonCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public async override Task<ErrorOr<bool>> Handle(DeleteVatExemptionReasonCommand request, CancellationToken cancellationToken = default)
        {
            var result = await HandleDeleteCommandAsync($"vat-exemption-reasons/{request.Id}", cancellationToken);
            if (result.IsError == false)
            {
                VatExemptionReasonCache.Clear(_keyedMemoryCache);
            }
            return result;
        }
    }
}
