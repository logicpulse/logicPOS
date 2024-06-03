using DevExpress.Xpo;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class fin_articlewarehouse : Entity
    {
        public fin_articlewarehouse() : base() { }
        public fin_articlewarehouse(Session session) : base(session) { }

        private fin_article fArticle;
        [Association(@"ArticleReferencesArticleWareHouse")]
        public fin_article Article
        {
            get { return fArticle; }
            set { SetPropertyValue("Article", ref fArticle, value); }
        }

        private fin_warehouse fWarehouse;
        [Association(@"ArticleWarehouseReferencesWareHouse")]
        public fin_warehouse Warehouse
        {
            get { return fWarehouse; }
            set { SetPropertyValue("Warehouse", ref fWarehouse, value); }
        }

        private fin_warehouselocation fLocation;
        [Association(@"WarehouseLocationReferencesArticleWarehouse")]
        public fin_warehouselocation Location
        {
            get { return fLocation; }
            set { SetPropertyValue("Location", ref fLocation, value); }
        }

        private fin_articleserialnumber fArticleSerialNumber;
        public fin_articleserialnumber ArticleSerialNumber
        {
            get { return fArticleSerialNumber; }
            set { SetPropertyValue("ArticleSerialNumber", ref fArticleSerialNumber, value); }
        }

        private decimal fQuantity;
        public decimal Quantity
        {
            get { return fQuantity; }
            set { SetPropertyValue<decimal>("Quantity", ref fQuantity, value); }
        }

    }
}