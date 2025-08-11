using LogicPOS.Api.Features.Common.Caching;
using System;

namespace LogicPOS.Api.Features.Articles.Subfamilies
{
    public static class SubfamiliesCache
    {
        public static void Clear(IKeyedMemoryCache keyedMemoryCache)
        {
            keyedMemoryCache.Remove(key => key.StartsWith("articles/subfamilies", StringComparison.OrdinalIgnoreCase));
        }
    }
}
