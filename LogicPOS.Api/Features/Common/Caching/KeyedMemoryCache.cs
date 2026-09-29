using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;

namespace LogicPOS.Api.Features.Common.Caching
{
    public sealed class KeyedMemoryCache : IKeyedMemoryCache
    {
        private readonly MemoryCache _cache;

        public KeyedMemoryCache(IMemoryCache cache)
        {
            _cache = cache as MemoryCache;
        }

        public void Remove(Predicate<string> keyPredicate)
        {
            var keys = _cache.Keys.Where(key => keyPredicate((string)key)).ToList();

            keys.ForEach(key => _cache.Remove(key));
        }
    }
}
