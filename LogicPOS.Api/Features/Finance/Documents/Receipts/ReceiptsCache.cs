using LogicPOS.Api.Features.Common.Caching;
using System;

namespace LogicPOS.Api.Features.Finance.Documents.Documents
{
    public static class ReceiptsCache
    {
        public static void Clear(IKeyedMemoryCache keyedMemoryCache)
        {
            keyedMemoryCache.Remove(key => string.Equals(key, "receipts"));
            keyedMemoryCache.Remove(key => key.StartsWith("receipts?", StringComparison.OrdinalIgnoreCase));
        }
    }
}
