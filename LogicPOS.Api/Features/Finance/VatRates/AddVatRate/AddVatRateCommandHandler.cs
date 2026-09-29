using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.VatRates;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.VatRates.AddVatRate
{
    public class AddVatRateCommandHandler : RequestHandler<AddVatRateCommand, ErrorOr<Guid>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;
        public AddVatRateCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public override async Task<ErrorOr<Guid>> Handle(AddVatRateCommand command, CancellationToken cancellationToken = default)
        {
            var result= await HandleAddCommandAsync("vatrates", command, cancellationToken);
            if(result.IsError==false)
            {
                VatRatesCache.Clear(_keyedMemoryCache);
            }
            return result;
        }
    }
}
