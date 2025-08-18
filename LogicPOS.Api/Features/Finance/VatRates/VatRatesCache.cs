using LogicPOS.Api.Features.Common.Caching;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogicPOS.Api.Features.Finance.VatRates
{
    public  class VatRatesCache
    {
        public static void Clear(IKeyedMemoryCache keyedMemoryCache)
        {
            keyedMemoryCache.Remove(key => string.Equals(key, "vatrates"));
            keyedMemoryCache.Remove(key => key.StartsWith("vatrates?", StringComparison.OrdinalIgnoreCase));
        }
    }
}
