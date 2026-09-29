using LogicPOS.Api.Features.Common.Caching;
using System;

namespace LogicPOS.Api.Features.Articles.Articles
{
    public static class ArticlesCache
    {
        public const string AutocompleteLinesEndpoint = "articles/autocomplete-lines";
        public const string UniqueAutocompleteLinesEndpoint = "articles/uniques/autocomplete-lines";

        public static void Clear(IKeyedMemoryCache keyedMemoryCache)
        {
            keyedMemoryCache.Remove(key => string.Equals(key, "articles"));
            keyedMemoryCache.Remove(key => key.StartsWith("articles?", StringComparison.OrdinalIgnoreCase));
            keyedMemoryCache.Remove(key => string.Equals(key, AutocompleteLinesEndpoint, StringComparison.OrdinalIgnoreCase));
            keyedMemoryCache.Remove(key => string.Equals(key, UniqueAutocompleteLinesEndpoint, StringComparison.OrdinalIgnoreCase));
        }
    }
}
