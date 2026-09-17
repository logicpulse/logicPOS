using LogicPOS.Api.Features.Common.Caching;
using System;

namespace LogicPOS.Api.Features.Articles.Families
{
    public static class ArticleFamiliesCache
    {
        public static void Clear(IKeyedMemoryCache keyedMemoryCache)
        {
            keyedMemoryCache.Remove(key => key.StartsWith("articles/families", StringComparison.OrdinalIgnoreCase));
        }
    }
}
