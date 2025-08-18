using LogicPOS.Api.Features.Common.Caching;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogicPOS.Api.Features.Finance.PaymentMethods
{
    public class PaymentMethodsCache
    {
        public static void Clear(IKeyedMemoryCache keyedMemoryCache)
        {
            keyedMemoryCache.Remove(key => string.Equals(key, "payment/methods"));
            keyedMemoryCache.Remove(key => key.StartsWith("payment/methods?", StringComparison.OrdinalIgnoreCase));
        }
    }
}
