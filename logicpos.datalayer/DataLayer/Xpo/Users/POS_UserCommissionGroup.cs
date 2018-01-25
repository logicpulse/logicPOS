using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class POS_UserCommissionGroup : XPGuidObject
    {
        public POS_UserCommissionGroup() : base() { }
        public POS_UserCommissionGroup(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID("POS_UserCommissionGroup", "Ord");
            Code = FrameworkUtils.GetNextTableFieldID("POS_UserCommissionGroup", "Code");
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

        decimal fCommission;
        public decimal Commission
        {
            get { return fCommission; }
            set { SetPropertyValue<decimal>("Commission", ref fCommission, value); }
        }

//UserCommissionGroup One <> Many Family
[Association(@"UserCommissionGroupReferencesFamily", typeof(FIN_ArticleFamily))]
public XPCollection<FIN_ArticleFamily> Family
{
    get { return GetCollection<FIN_ArticleFamily>("Family"); }
}

//UserCommissionGroup One <> Many SubFamily
[Association(@"UserCommissionGroupReferencesSubFamily", typeof(FIN_ArticleSubFamily))]
public XPCollection<FIN_ArticleSubFamily> SubFamily
{
    get { return GetCollection<FIN_ArticleSubFamily>("SubFamily"); }
}

//UserCommissionGroup One <> Many Article
[Association(@"UserCommissionGroupReferencesArticle", typeof(FIN_Article))]
public XPCollection<FIN_Article> Article
{
    get { return GetCollection<FIN_Article>("Article"); }
}

        //CommissionGroup One <> Many User
        [Association(@"UserCommissionGroupReferencesUserDetail", typeof(SYS_UserDetail))]
        public XPCollection<SYS_UserDetail> Users
        {
            get { return GetCollection<SYS_UserDetail>("Users"); }
        }
    }
}