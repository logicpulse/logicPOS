using LogicPOS.Api.Features.Common.Caching;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogicPOS.Api.Features.Finance.PaymentConditions
{
    public class PaymentConditionsCache
    {
        public static void Clear(IKeyedMemoryCache keyedMemoryCache)
        {
            keyedMemoryCache.Remove(key => string.Equals(key, "payment/conditions"));
            keyedMemoryCache.Remove(key => key.StartsWith("payment/conditions?", StringComparison.OrdinalIgnoreCase));
        }
    }
}
