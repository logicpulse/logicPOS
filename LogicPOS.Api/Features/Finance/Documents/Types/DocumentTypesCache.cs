using LogicPOS.Api.Features.Common.Caching;
using System;

namespace LogicPOS.Api.Features.Finance.DocumentTypes
{
    public static class DocumentTypesCache
    {
        public static void Clear(IKeyedMemoryCache keyedMemoryCache)
        {
            keyedMemoryCache.Remove(key => string.Equals(key, "document/types"));
            keyedMemoryCache.Remove(key => key.StartsWith("document/types?", StringComparison.OrdinalIgnoreCase));
        }
    }
}
