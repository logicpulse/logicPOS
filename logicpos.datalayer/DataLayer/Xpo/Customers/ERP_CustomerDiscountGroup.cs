using DevExpress.Xpo;
using logicpos.datalayer.App;
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
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(erp_customerdiscountgroup), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(erp_customerdiscountgroup), "Code");
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

        string fDesignation;
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