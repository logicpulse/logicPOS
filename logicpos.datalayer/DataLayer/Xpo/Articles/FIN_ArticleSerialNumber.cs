using DevExpress.Xpo;
using logicpos.datalayer.DataLayer.Xpo.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

////Artigos Compostos [IN:016522]
namespace logicpos.datalayer.DataLayer.Xpo.Articles
{
    [DeferredDeletion(false)]
    public class fin_articleserialnumber : XPGuidObject
    {
        public fin_articleserialnumber() : base() { }
        public fin_articleserialnumber(Session session) : base(session) { }

        //Article One <> Many ArticleSerialNumber
        private fin_article fArticle;
        [Association(@"ArticleReferencesArticleSerialNumber")]
        public fin_article Article
        {
            get { return fArticle; }
            set { SetPropertyValue<fin_article>("Article", ref fArticle, value); }
        }

        //fin_warehouse fWarehouse;
        //[Association(@"ArticleReferencesWarehouseSerialNumber")]
        //public fin_warehouse Warehouse
        //{
        //    get { return fWarehouse; }
        //    set { SetPropertyValue<fin_warehouse>("Warehouse", ref fWarehouse, value); }
        //}

        //fin_warehouselocation fLocation;
        //[Association(@"WarehouseLocationReferencesWarehouseSerialNumber")]
        //public fin_warehouselocation Location
        //{
        //    get { return fLocation; }
        //    set { SetPropertyValue<fin_warehouselocation>("Location", ref fLocation, value); }
        //}

        private fin_articlewarehouse fArticleWarehouse;
        public fin_articlewarehouse ArticleWarehouse
        {
            get { return fArticleWarehouse; }
            set { SetPropertyValue<fin_articlewarehouse>("ArticleWarehouse", ref fArticleWarehouse, value); }
        }

        private fin_articlestock fStockMovimentIn;
        public fin_articlestock StockMovimentIn
        {
            get { return fStockMovimentIn; }
            set { SetPropertyValue<fin_articlestock>("StockMovimentIn", ref fStockMovimentIn, value); }
        }

        private fin_articlestock fStockMovimentOut;
        public fin_articlestock StockMovimentOut
        {
            get { return fStockMovimentOut; }
            set { SetPropertyValue<fin_articlestock>("StockMovimentOut", ref fStockMovimentOut, value); }
        }

        private string fSerialNumber;
        public string SerialNumber
        {
            get { return fSerialNumber; }
            set { SetPropertyValue<string>("SerialNumber", ref fSerialNumber, value); }
        }

        private bool fIsSold;
        public bool IsSold
        {
            get { return fIsSold; }
            set { SetPropertyValue<bool>("IsSold", ref fIsSold, value); }
        }

        private int fStatus;
        public int Status
        {
            get { return fStatus; }
            set { SetPropertyValue<int>("Status", ref fStatus, value); }
        }

        [Association(@"ArticleCompositionReferencesArticleSerialNumber", typeof(fin_articlecompositionserialnumber))]
        public XPCollection<fin_articlecompositionserialnumber> ArticleComposition
        {
            get { return GetCollection<fin_articlecompositionserialnumber>("ArticleComposition"); }
        }

    }
}
