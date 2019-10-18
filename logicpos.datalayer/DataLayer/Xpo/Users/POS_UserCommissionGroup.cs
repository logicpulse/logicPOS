using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class pos_usercommissiongroup : XPGuidObject
    {
        public pos_usercommissiongroup() : base() { }
        public pos_usercommissiongroup(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(pos_usercommissiongroup), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(pos_usercommissiongroup), "Code");
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
[Association(@"UserCommissionGroupReferencesFamily", typeof(fin_articlefamily))]
public XPCollection<fin_articlefamily> Family
{
    get { return GetCollection<fin_articlefamily>("Family"); }
}

//UserCommissionGroup One <> Many SubFamily
[Association(@"UserCommissionGroupReferencesSubFamily", typeof(fin_articlesubfamily))]
public XPCollection<fin_articlesubfamily> SubFamily
{
    get { return GetCollection<fin_articlesubfamily>("SubFamily"); }
}

//UserCommissionGroup One <> Many Article
[Association(@"UserCommissionGroupReferencesArticle", typeof(fin_article))]
public XPCollection<fin_article> Article
{
    get { return GetCollection<fin_article>("Article"); }
}

        //CommissionGroup One <> Many User
        [Association(@"UserCommissionGroupReferencesUserDetail", typeof(sys_userdetail))]
        public XPCollection<sys_userdetail> Users
        {
            get { return GetCollection<sys_userdetail>("Users"); }
        }
    }
}