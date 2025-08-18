using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.VatExemptionReasons.AddVatExemptionReason
{
    public class AddVatExceptionReasonCommandHandler : RequestHandler<AddVatExemptionReasonCommand, ErrorOr<Guid>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;
        public AddVatExceptionReasonCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public override async Task<ErrorOr<Guid>> Handle(AddVatExemptionReasonCommand command, CancellationToken cancellationToken = default)
        {
            var result = await HandleAddCommandAsync("vatexemptionreasons", command, cancellationToken);
            if (result.IsError == false)
            {
                VatExemptionReasonCache.Clear(_keyedMemoryCache);
            }
            return result;
        }
    }
}
