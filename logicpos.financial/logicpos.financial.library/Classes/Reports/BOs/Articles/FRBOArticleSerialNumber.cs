namespace logicpos.financial.library.Classes.Reports.BOs.Articles
{
    [FRBO(Entity = "fin_articleserialnumber")]
    public class FRBOArticleSerialNumber : FRBOBaseObject
    {
        public string SerialNumber { get; set; }
        public string ArticleName { get; set; }
        public string ArticleRef { get; set; }
        public string footerText { get; set; }
    }
}
