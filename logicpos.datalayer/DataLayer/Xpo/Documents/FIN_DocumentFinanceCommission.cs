using System;
using DevExpress.Xpo;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class fin_documentfinancecommission : XPGuidObject
    {
        public fin_documentfinancecommission() : base() { }
        public fin_documentfinancecommission(Session session) : base(session) { }

        UInt32 fOrd;
        public UInt32 Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        DateTime fDate;
        public DateTime Date
        {
            get { return fDate; }
            set { SetPropertyValue<DateTime>("Date", ref fDate, value); }
        }

        decimal fCommission;
        public decimal Commission
        {
            get { return fCommission; }
            set { SetPropertyValue<decimal>("Commission", ref fCommission, value); }
        }

        decimal fTotal;
        public decimal Total
        {
            get { return fTotal; }
            set { SetPropertyValue<decimal>("Total", ref fTotal, value); }
        }

        pos_usercommissiongroup fCommissionGroup;
        public pos_usercommissiongroup CommissionGroup
        {
            get { return fCommissionGroup; }
            set { SetPropertyValue<pos_usercommissiongroup>("CommissionGroup", ref fCommissionGroup, value); }
        }

        fin_documentfinancemaster fFinanceMaster;
        public fin_documentfinancemaster FinanceMaster
        {
            get { return fFinanceMaster; }
            set { SetPropertyValue<fin_documentfinancemaster>("FinanceMaster", ref fFinanceMaster, value); }
        }

        fin_documentfinancedetail fFinanceDetail;
        public fin_documentfinancedetail FinanceDetail
        {
            get { return fFinanceDetail; }
            set { SetPropertyValue<fin_documentfinancedetail>("FinanceDetail", ref fFinanceDetail, value); }
        }

        sys_userdetail fUserDetail;
        public sys_userdetail UserDetail
        {
            get { return fUserDetail; }
            set { SetPropertyValue<sys_userdetail>("UserDetail", ref fUserDetail, value); }
        }

        pos_configurationplaceterminal fTerminal;
        public pos_configurationplaceterminal Terminal
        {
            get { return fTerminal; }
            set { SetPropertyValue<pos_configurationplaceterminal>("Terminal", ref fTerminal, value); }
        }
    }
}