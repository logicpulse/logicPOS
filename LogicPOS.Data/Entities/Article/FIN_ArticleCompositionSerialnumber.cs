using DevExpress.Xpo;
using logicpos.datalayer.DataLayer.Xpo;


////Artigos Compostos [IN:016522]
namespace LogicPOS.Domain.Entities
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
            set { SetPropertyValue("ArticleSerialNumber", ref fArticleSerialNumber, value); }
        }

        private fin_articleserialnumber fArticleSerialNumberhild;
        public fin_articleserialnumber ArticleSerialNumberChild
        {
            get { return fArticleSerialNumberhild; }
            set { SetPropertyValue("ArticleSerialNumberChild", ref fArticleSerialNumberhild, value); }
        }
    }
}
