using System;

namespace LogicPOS.Api.Features.Common.Caching
{
    public interface IKeyedMemoryCache
    {
        void Remove(Predicate<string> keyPredicate);
    }
}
