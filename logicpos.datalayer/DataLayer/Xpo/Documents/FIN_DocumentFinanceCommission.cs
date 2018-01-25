using System;
using DevExpress.Xpo;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class FIN_DocumentFinanceCommission : XPGuidObject
    {
        public FIN_DocumentFinanceCommission() : base() { }
        public FIN_DocumentFinanceCommission(Session session) : base(session) { }

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

        POS_UserCommissionGroup fCommissionGroup;
        public POS_UserCommissionGroup CommissionGroup
        {
            get { return fCommissionGroup; }
            set { SetPropertyValue<POS_UserCommissionGroup>("CommissionGroup", ref fCommissionGroup, value); }
        }

        FIN_DocumentFinanceMaster fFinanceMaster;
        public FIN_DocumentFinanceMaster FinanceMaster
        {
            get { return fFinanceMaster; }
            set { SetPropertyValue<FIN_DocumentFinanceMaster>("FinanceMaster", ref fFinanceMaster, value); }
        }

        FIN_DocumentFinanceDetail fFinanceDetail;
        public FIN_DocumentFinanceDetail FinanceDetail
        {
            get { return fFinanceDetail; }
            set { SetPropertyValue<FIN_DocumentFinanceDetail>("FinanceDetail", ref fFinanceDetail, value); }
        }

        SYS_UserDetail fUserDetail;
        public SYS_UserDetail UserDetail
        {
            get { return fUserDetail; }
            set { SetPropertyValue<SYS_UserDetail>("UserDetail", ref fUserDetail, value); }
        }

        POS_ConfigurationPlaceTerminal fTerminal;
        public POS_ConfigurationPlaceTerminal Terminal
        {
            get { return fTerminal; }
            set { SetPropertyValue<POS_ConfigurationPlaceTerminal>("Terminal", ref fTerminal, value); }
        }
    }
}