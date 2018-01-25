using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class ERP_CustomerType : XPGuidObject
    {
        public ERP_CustomerType() : base() { }
        public ERP_CustomerType(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID("ERP_CustomerType", "Ord");
            Code = FrameworkUtils.GetNextTableFieldID("ERP_CustomerType", "Code");
        }

        UInt32 fOrd;
        public UInt32 Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        UInt32 fCode;
        [Indexed(Unique = true)]
        public UInt32 Code
        {
            get { return fCode; }
            set { SetPropertyValue<UInt32>("Code", ref fCode, value); }
        }

        //CustomerType One <> Many Customer
        [Association(@"CustomerTypeReferencesCustomer", typeof(ERP_Customer))]
        public XPCollection<ERP_Customer> Customer
        {
            get { return GetCollection<ERP_Customer>("Customer"); }
        }

        string fDesignation;
        [Indexed(Unique = true)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }
    }
}