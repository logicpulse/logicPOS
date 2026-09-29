using LogicPOS.Api.Features.Common.Caching;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogicPOS.Api.Features.Finance.Currencies
{
    public class CurrenciesCache
    {
        public static void Clear(IKeyedMemoryCache keyedMemoryCache)
        {
            keyedMemoryCache.Remove(key => string.Equals(key, "currencies"));
            keyedMemoryCache.Remove(key => key.StartsWith("currencies?", StringComparison.OrdinalIgnoreCase));
        }
    }
}
