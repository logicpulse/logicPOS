using System;
using DevExpress.Xpo;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class pos_worksessionmovement : XPGuidObject
    {
        public pos_worksessionmovement() : base() { }
        public pos_worksessionmovement(Session session) : base(session) { }

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

        Decimal fMovementAmount;
        public Decimal MovementAmount
        {
            get { return fMovementAmount; }
            set { SetPropertyValue<Decimal>("MovementAmount", ref fMovementAmount, value); }
        }

        string fDescription;
        [Size(255)]
        public string Description
        {
            get { return fDescription; }
            set { SetPropertyValue<string>("Description", ref fDescription, value); }
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

        fin_documentfinancemaster fDocumentFinanceMaster;
        public fin_documentfinancemaster DocumentFinanceMaster
        {
            get { return fDocumentFinanceMaster; }
            set { SetPropertyValue<fin_documentfinancemaster>("DocumentFinanceMaster", ref fDocumentFinanceMaster, value); }
        }

        fin_documentfinancepayment fDocumentFinancePayment;
        public fin_documentfinancepayment DocumentFinancePayment
        {
            get { return fDocumentFinancePayment; }
            set { SetPropertyValue<fin_documentfinancepayment>("DocumentFinancePayment", ref fDocumentFinancePayment, value); }
        }

        fin_documentfinancetype fDocumentFinanceType;
        public fin_documentfinancetype DocumentFinanceType
        {
            get { return fDocumentFinanceType; }
            set { SetPropertyValue<fin_documentfinancetype>("DocumentFinanceType", ref fDocumentFinanceType, value); }
        }

        fin_configurationpaymentmethod fPaymentMethod;
        public fin_configurationpaymentmethod PaymentMethod
        {
            get { return fPaymentMethod; }
            set { SetPropertyValue<fin_configurationpaymentmethod>("PaymentMethod", ref fPaymentMethod, value); }
        }

        //WorkSessionPeriod One <> Many WorkSessionMovement
        pos_worksessionperiod fWorkSessionPeriod;
        [Association(@"WorkSessionPeriodReferencesWorkSessionMovement")]
        public pos_worksessionperiod WorkSessionPeriod
        {
            get { return fWorkSessionPeriod; }
            set { SetPropertyValue<pos_worksessionperiod>("WorkSessionPeriod", ref fWorkSessionPeriod, value); }
        }

        //WorkSessionMovementType One <> Many WorkSessionMovement
        pos_worksessionmovementtype fWorkSessionMovementType;
        [Association(@"WorkSessionMovementTypeReferencesWorkSessionMovement")]
        public pos_worksessionmovementtype WorkSessionMovementType
        {
            get { return fWorkSessionMovementType; }
            set { SetPropertyValue<pos_worksessionmovementtype>("WorkSessionMovementType", ref fWorkSessionMovementType, value); }
        }
    }
}
