using LogicPOS.Api.Features.Common.Caching;
using System;

namespace LogicPOS.Api.Features.Finance.Documents.Types
{
    public static class DocumentTypesCache
    {
        public static void Clear(IKeyedMemoryCache keyedMemoryCache)
        {
            keyedMemoryCache.Remove(key => string.Equals(key, "documents/types"));
            keyedMemoryCache.Remove(key => string.Equals(key, "documents/types/active"));
            keyedMemoryCache.Remove(key => key.StartsWith("documents/types?", StringComparison.OrdinalIgnoreCase));
        }
    }
}
