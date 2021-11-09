using System;
using DevExpress.Xpo;
using logicpos.datalayer.DataLayer.Xpo.Articles;
using logicpos.datalayer.Enums;


namespace logicpos.datalayer.DataLayer.Xpo.Documents
{
    [DeferredDeletion(false)]
    public class fin_warehouselocation : XPGuidObject
    {
        public fin_warehouselocation() : base() { }
        public fin_warehouselocation(Session session) : base(session) { }

        string fOrd;
        [Indexed(Unique = true)]
        public string Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<string>("Ord", ref fOrd, value); }
        }

        string fCode;
        [Indexed(Unique = true)]
        public string Code
        {
            get { return fCode; }
            set { SetPropertyValue<string>("Code", ref fCode, value); }
        }

        string fDesignation;
        [Indexed(Unique = true)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        fin_warehouse fWarehouse;
        [Association(@"WarehouseReferencesWareHouseLocation")]
        public fin_warehouse Warehouse
        {
            get { return fWarehouse; }
            set { SetPropertyValue<fin_warehouse>("Warehouse", ref fWarehouse, value); }
        }

        [Association(@"WarehouseLocationReferencesArticleWarehouse", typeof(fin_articlewarehouse))]
        public XPCollection<fin_articlewarehouse> ArticleWarehouseLocation
        {
            get { return GetCollection<fin_articlewarehouse>("ArticleWarehouseLocation"); }
        }

        //[Association(@"WarehouseLocationReferencesWarehouseSerialNumber", typeof(fin_articleserialnumber))]
        //public XPCollection<fin_articleserialnumber> ArticleSerialNumberLocation
        //{
        //    get { return GetCollection<fin_articleserialnumber>("ArticleSerialNumberLocation"); }
        //}

        
    }
}
