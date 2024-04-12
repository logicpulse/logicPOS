using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

////Artigos Compostos [IN:016522]
namespace logicpos.datalayer.DataLayer.Xpo.Articles
{
    [DeferredDeletion(false)]
    public class fin_articlecomposition : XPGuidObject
    {
        public fin_articlecomposition() : base() { }
        public fin_articlecomposition(Session session) : base(session) { }

        //Guid fArticleParent;
        //public Guid ArticleParent
        //{
        //    get { return fArticleParent; }
        //    set { SetPropertyValue<Guid>("Article", ref fArticleParent, value); }
        //}

        //Article One <> Many ArticleCompositions
        private fin_article fArticle;
        [Association(@"ArticleReferencesArticleComposition")]
        public fin_article Article
        {
            get { return fArticle; }
            set { SetPropertyValue<fin_article>("Article", ref fArticle, value); }
        }

        private fin_article fArticleChild;
        public fin_article ArticleChild
        {
            get { return fArticleChild; }
            set { SetPropertyValue<fin_article>("ArticleChild", ref fArticleChild, value); }
        }

        //Guid fArticleChild;
        //public Guid ArticleChild
        //{
        //    get { return fArticleChild; }
        //    set { SetPropertyValue<Guid>("ArticleChild", ref fArticleChild, value); }
        //}


        private Decimal fQuantity;
        public Decimal Quantity
        {
            get { return fQuantity; }
            set { SetPropertyValue<Decimal>("Quantity", ref fQuantity, value); }
        }

        //Guid fArticleChild;
        //public Guid ArticleChild
        //{
        //    get { return fArticleChild; }
        //    set { SetPropertyValue<Guid>("ArticleChild", ref fArticleChild, value); }
        //}
    }
}
