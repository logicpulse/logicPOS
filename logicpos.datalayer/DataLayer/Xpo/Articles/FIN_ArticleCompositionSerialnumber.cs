using DevExpress.Xpo;

////Artigos Compostos [IN:016522]
namespace logicpos.datalayer.DataLayer.Xpo.Articles
{
    [DeferredDeletion(false)]
    public class fin_articlecompositionserialnumber : XPGuidObject
    {
        public fin_articlecompositionserialnumber() : base() { }
        public fin_articlecompositionserialnumber(Session session) : base(session) { }


        //Article One <> Many ArticleCompositions
        private fin_articleserialnumber fArticleSerialNumber;
        [Association(@"ArticleCompositionReferencesArticleSerialNumber")]
        public fin_articleserialnumber ArticleSerialNumber
        {
            get { return fArticleSerialNumber; }
            set { SetPropertyValue<fin_articleserialnumber>("ArticleSerialNumber", ref fArticleSerialNumber, value); }
        }

        private fin_articleserialnumber fArticleSerialNumberhild;
        public fin_articleserialnumber ArticleSerialNumberChild
        {
            get { return fArticleSerialNumberhild; }
            set { SetPropertyValue<fin_articleserialnumber>("ArticleSerialNumberChild", ref fArticleSerialNumberhild, value); }
        }
    }
}
