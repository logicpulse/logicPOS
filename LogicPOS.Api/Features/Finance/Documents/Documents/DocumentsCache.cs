using LogicPOS.Api.Features.Common.Caching;
using System;

namespace LogicPOS.Api.Features.Finance.Documents.Documents
{
    public static class DocumentsCache
    {
        public static void Clear(IKeyedMemoryCache keyedMemoryCache)
        {
            keyedMemoryCache.Remove(key => string.Equals(key, "documents"));
            keyedMemoryCache.Remove(key => key.StartsWith("documents?", StringComparison.OrdinalIgnoreCase));
            keyedMemoryCache.Remove(key => key.StartsWith("documents/relations", StringComparison.OrdinalIgnoreCase));
            keyedMemoryCache.Remove(key => key.StartsWith("documents/totals", StringComparison.OrdinalIgnoreCase));
        }
    }
}
