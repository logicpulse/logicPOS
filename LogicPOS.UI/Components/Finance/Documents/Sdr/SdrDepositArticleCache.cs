using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.UI.Components.Articles;

namespace LogicPOS.UI.Components.Finance.Documents.Sdr
{
    /// <summary>
    /// Session cache for the Volta deposit article (SDRVDEP). Avoids repeated
    /// GET /articles/code/SDRVDEP on every ticket/label enrich in the POS.
    /// </summary>
    public static class SdrDepositArticleCache
    {
        private static readonly object Sync = new object();
        private static ArticleViewModel _cached;
        private static bool _loaded;

        public static ArticleViewModel GetOrLoad()
        {
            lock (Sync)
            {
                if (_loaded)
                {
                    return _cached;
                }

                _cached = ArticlesService.GetArticleByCode(SdrConstants.SdrArticleCode);
                _loaded = true;
                return _cached;
            }
        }

        public static void Invalidate()
        {
            lock (Sync)
            {
                _cached = null;
                _loaded = false;
            }
        }
    }
}
