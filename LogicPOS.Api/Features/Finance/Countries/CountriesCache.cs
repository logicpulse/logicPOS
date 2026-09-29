using LogicPOS.Api.Features.Common.Caching;
using System;

namespace LogicPOS.Api.Features.Finance.Countries
{
    public static class CountriesCache
    {

        public static void Clear(IKeyedMemoryCache keyedMemoryCache)
        {
            keyedMemoryCache.Remove(key => string.Equals(key, "countries"));
            keyedMemoryCache.Remove(key => key.StartsWith("countries?", StringComparison.OrdinalIgnoreCase));
        }

    }
}
