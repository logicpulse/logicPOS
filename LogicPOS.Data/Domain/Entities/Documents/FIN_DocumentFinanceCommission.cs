using System;
using DevExpress.Xpo;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class fin_documentfinancecommission : Entity
    {
        public fin_documentfinancecommission() : base() { }
        public fin_documentfinancecommission(Session session) : base(session) { }

        private uint fOrd;
        public uint Ord
        {
            get { return fOrd; }
            set { SetPropertyValue("Ord", ref fOrd, value); }
        }

        private DateTime fDate;
        public DateTime Date
        {
            get { return fDate; }
            set { SetPropertyValue<DateTime>("Date", ref fDate, value); }
        }

        private decimal fCommission;
        public decimal Commission
        {
            get { return fCommission; }
            set { SetPropertyValue<decimal>("Commission", ref fCommission, value); }
        }

        private decimal fTotal;
        public decimal Total
        {
            get { return fTotal; }
            set { SetPropertyValue<decimal>("Total", ref fTotal, value); }
        }

        private pos_usercommissiongroup fCommissionGroup;
        public pos_usercommissiongroup CommissionGroup
        {
            get { return fCommissionGroup; }
            set { SetPropertyValue("CommissionGroup", ref fCommissionGroup, value); }
        }

        private fin_documentfinancemaster fFinanceMaster;
        public fin_documentfinancemaster FinanceMaster
        {
            get { return fFinanceMaster; }
            set { SetPropertyValue("FinanceMaster", ref fFinanceMaster, value); }
        }

        private fin_documentfinancedetail fFinanceDetail;
        public fin_documentfinancedetail FinanceDetail
        {
            get { return fFinanceDetail; }
            set { SetPropertyValue("FinanceDetail", ref fFinanceDetail, value); }
        }

        private sys_userdetail fUserDetail;
        public sys_userdetail UserDetail
        {
            get { return fUserDetail; }
            set { SetPropertyValue("UserDetail", ref fUserDetail, value); }
        }

        private pos_configurationplaceterminal fTerminal;
        public pos_configurationplaceterminal Terminal
        {
            get { return fTerminal; }
            set { SetPropertyValue("Terminal", ref fTerminal, value); }
        }
    }
}