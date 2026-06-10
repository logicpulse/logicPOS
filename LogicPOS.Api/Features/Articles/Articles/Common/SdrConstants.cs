namespace LogicPOS.Api.Features.Articles.Common
{
    public static class SdrConstants
    {
        public const string SdrArticleCode = "SDRVDEP";

        public static bool IsDepositArticle(string code) =>
            string.Equals(code, SdrArticleCode, global::System.StringComparison.OrdinalIgnoreCase);
    }
}
