using System;
using DevExpress.Xpo;
using logicpos.datalayer.DataLayer.Xpo.Articles;
using logicpos.datalayer.DataLayer.Xpo.Documents;
using logicpos.datalayer.Enums;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class fin_articlewarehouse : XPGuidObject
    {
        public fin_articlewarehouse() : base() { }
        public fin_articlewarehouse(Session session) : base(session) { }

        fin_article fArticle;
        [Association(@"ArticleReferencesArticleWareHouse")]
        public fin_article Article
        {
            get { return fArticle; }
            set { SetPropertyValue<fin_article>("Article", ref fArticle, value); }
        }

        fin_warehouse fWarehouse;
        [Association(@"ArticleWarehouseReferencesWareHouse")]
        public fin_warehouse Warehouse
        {
            get { return fWarehouse; }
            set { SetPropertyValue<fin_warehouse>("Warehouse", ref fWarehouse, value); }
        }

        fin_warehouselocation fLocation;
        [Association(@"WarehouseLocationReferencesArticleWarehouse")]
        public fin_warehouselocation Location
        {
            get { return fLocation; }
            set { SetPropertyValue<fin_warehouselocation>("Location", ref fLocation, value); }
        }

        fin_articleserialnumber fArticleSerialNumber;
        public fin_articleserialnumber ArticleSerialNumber
        {
            get { return fArticleSerialNumber; }
            set { SetPropertyValue<fin_articleserialnumber>("ArticleSerialNumber", ref fArticleSerialNumber, value); }
        }

        decimal fQuantity;
        public decimal Quantity
        {
            get { return fQuantity; }
            set { SetPropertyValue<decimal>("Quantity", ref fQuantity, value); }
        }

    }
}