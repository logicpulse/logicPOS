using LogicPOS.Api.Features.Common.Caching;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogicPOS.Api.Features.VatExemptionReasons
{
    public class VatExemptionReasonCache
    {
        public static void Clear(IKeyedMemoryCache keyedMemoryCache)
        {
            keyedMemoryCache.Remove(key => string.Equals(key, "vatexemptionreasons"));
            keyedMemoryCache.Remove(key => key.StartsWith("vatexemptionreasons?", StringComparison.OrdinalIgnoreCase));
        }
    }
}
