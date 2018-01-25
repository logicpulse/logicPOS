using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class ERP_CustomerDiscountGroup : XPGuidObject
    {
        public ERP_CustomerDiscountGroup() : base() { }
        public ERP_CustomerDiscountGroup(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID("ERP_CustomerDiscountGroup", "Ord");
            Code = FrameworkUtils.GetNextTableFieldID("ERP_CustomerDiscountGroup", "Code");
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
[Association(@"CustomerDiscountGroupReferencesFamily", typeof(FIN_ArticleFamily))]
public XPCollection<FIN_ArticleFamily> Family
{
    get { return GetCollection<FIN_ArticleFamily>("Family"); }
}

//CustomerDiscountGroup One <> Many SubFamily
[Association(@"CustomerDiscountGroupReferencesSubFamily", typeof(FIN_ArticleSubFamily))]
public XPCollection<FIN_ArticleSubFamily> SubFamily
{
    get { return GetCollection<FIN_ArticleSubFamily>("SubFamily"); }
}

//CustomerDiscountGroup One <> Many Article
[Association(@"CustomerDiscountGroupReferencesArticle", typeof(FIN_Article))]
public XPCollection<FIN_Article> Article
{
    get { return GetCollection<FIN_Article>("Article"); }
}

        //CustomerDiscountGroup One <> Many Customer
        [Association(@"CustomerDiscountGroupReferencesCustomer", typeof(ERP_Customer))]
        public XPCollection<ERP_Customer> Customer
        {
            get { return GetCollection<ERP_Customer>("Customer"); }
        }
    }
}