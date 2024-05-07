using DevExpress.Xpo;
using logicpos.datalayer.Xpo;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class erp_customerdiscountgroup : XPGuidObject
    {
        public erp_customerdiscountgroup() : base() { }
        public erp_customerdiscountgroup(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = XPOHelper.GetNextTableFieldID(nameof(erp_customerdiscountgroup), "Ord");
            Code = XPOHelper.GetNextTableFieldID(nameof(erp_customerdiscountgroup), "Code");
        }

        private uint fOrd;
        public uint Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        private uint fCode;
        [Indexed(Unique = true)]
        public uint Code
        {
            get { return fCode; }
            set { SetPropertyValue<UInt32>("Code", ref fCode, value); }
        }

        private string fDesignation;
        [Indexed(Unique = true)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

//CustomerDiscountGroup One <> Many Family
[Association(@"CustomerDiscountGroupReferencesFamily", typeof(fin_articlefamily))]
public XPCollection<fin_articlefamily> Family
{
    get { return GetCollection<fin_articlefamily>("Family"); }
}

//CustomerDiscountGroup One <> Many SubFamily
[Association(@"CustomerDiscountGroupReferencesSubFamily", typeof(fin_articlesubfamily))]
public XPCollection<fin_articlesubfamily> SubFamily
{
    get { return GetCollection<fin_articlesubfamily>("SubFamily"); }
}

//CustomerDiscountGroup One <> Many Article
[Association(@"CustomerDiscountGroupReferencesArticle", typeof(fin_article))]
public XPCollection<fin_article> Article
{
    get { return GetCollection<fin_article>("Article"); }
}

        //CustomerDiscountGroup One <> Many Customer
        [Association(@"CustomerDiscountGroupReferencesCustomer", typeof(erp_customer))]
        public XPCollection<erp_customer> Customer
        {
            get { return GetCollection<erp_customer>("Customer"); }
        }
    }
}