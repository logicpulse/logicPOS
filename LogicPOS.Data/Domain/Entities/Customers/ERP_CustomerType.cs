using DevExpress.Xpo;
using LogicPOS.Data.XPO.Utility;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class erp_customertype : Entity
    {
        public erp_customertype() : base() { }
        public erp_customertype(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = XPOUtility.GetNextTableFieldID(nameof(erp_customertype), "Ord");
            Code = XPOUtility.GetNextTableFieldID(nameof(erp_customertype), "Code");
        }

        private uint fOrd;
        public uint Ord
        {
            get { return fOrd; }
            set { SetPropertyValue("Ord", ref fOrd, value); }
        }

        private uint fCode;
        [Indexed(Unique = true)]
        public uint Code
        {
            get { return fCode; }
            set { SetPropertyValue("Code", ref fCode, value); }
        }

        private string fDesignation;
        [Indexed(Unique = true)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        //CustomerType One <> Many Customer
        [Association(@"CustomerTypeReferencesCustomer", typeof(erp_customer))]
        public XPCollection<erp_customer> Customer
        {
            get { return GetCollection<erp_customer>("Customer"); }
        }
    }
}