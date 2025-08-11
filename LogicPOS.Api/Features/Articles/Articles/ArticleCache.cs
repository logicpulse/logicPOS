using LogicPOS.Api.Features.Common.Caching;
using System;

namespace LogicPOS.Api.Features.Articles.Articles
{
    public static class ArticleCache
    {
        public static void Clear(IKeyedMemoryCache keyedMemoryCache)
        {
            keyedMemoryCache.Remove(key => string.Equals(key, "articles"));
            keyedMemoryCache.Remove(key => key.StartsWith("articles?", StringComparison.OrdinalIgnoreCase));
        }
    }
}
