using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.VatExemptionReasons.UpdateVatExemptionReason
{
    public class UpdateVatExemptionReasonCommandHandler :
        RequestHandler<UpdateVatExemptionReasonCommand, ErrorOr<Unit>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;
        public UpdateVatExemptionReasonCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache Cache) : base(factory)
        {
            _keyedMemoryCache = Cache;
        }

        public override async Task<ErrorOr<Unit>> Handle(UpdateVatExemptionReasonCommand command, CancellationToken cancellationToken = default)
        {
            var result= await HandleUpdateCommandAsync($"/vatexemptionreasons/{command.Id}", command, cancellationToken);
            if (result.IsError == false)
            {
                VatExemptionReasonCache.Clear(_keyedMemoryCache);
            }
            return result;
        }
    }
}
