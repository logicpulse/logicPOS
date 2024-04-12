using System;
using DevExpress.Xpo;
using logicpos.datalayer.DataLayer.Xpo.Articles;
using logicpos.datalayer.DataLayer.Xpo.Documents;
using logicpos.datalayer.Enums;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class fin_warehouse : XPGuidObject
    {
        public fin_warehouse() : base() { }
        public fin_warehouse(Session session) : base(session) { }

        private string fOrd;
        [Indexed(Unique = true)]
        public string Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<string>("Ord", ref fOrd, value); }
        }

        private string fCode;
        [Indexed(Unique = true)]
        public string Code
        {
            get { return fCode; }
            set { SetPropertyValue<string>("Code", ref fCode, value); }
        }

        private string fDesignation;
        [Indexed(Unique = true)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        private bool fIsDefault;
        [Indexed(Unique = true)]
        public bool IsDefault
        {
            get { return fIsDefault; }
            set { SetPropertyValue<bool>("IsDefault", ref fIsDefault, value); }
        }

        [Association(@"WarehouseReferencesWareHouseLocation", typeof(fin_warehouselocation))]
        public XPCollection<fin_warehouselocation> WarehouseLocation
        {
            get { return GetCollection<fin_warehouselocation>("WarehouseLocation"); }
        }
   
        [Association(@"ArticleWarehouseReferencesWareHouse", typeof(fin_articlewarehouse))]
        public XPCollection<fin_articlewarehouse> ArticleWarehouse
        {
            get { return GetCollection<fin_articlewarehouse>("ArticleWarehouse"); }
        }

        //[Association(@"ArticleReferencesWarehouseSerialNumber", typeof(fin_articleserialnumber))]
        //public XPCollection<fin_articleserialnumber> ArticleSerialNumber
        //{
        //    get { return GetCollection<fin_articleserialnumber>("ArticleSerialNumber"); }
        //}

    }
}