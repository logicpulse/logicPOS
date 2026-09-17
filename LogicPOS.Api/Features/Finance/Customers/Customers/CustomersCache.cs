using LogicPOS.Api.Features.Common.Caching;

namespace LogicPOS.Api.Features.Finance.Customers.Customers
{
    public static class CustomersCache
    {
        public static void Clear(IKeyedMemoryCache keyedMemoryCache)
        {
            keyedMemoryCache.Remove(key => string.Equals(key, "customers"));
        }
    }
}
