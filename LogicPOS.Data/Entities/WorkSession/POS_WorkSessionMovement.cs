using System;
using DevExpress.Xpo;
using logicpos.datalayer.DataLayer.Xpo;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class pos_worksessionmovement : XPGuidObject
    {
        public pos_worksessionmovement() : base() { }
        public pos_worksessionmovement(Session session) : base(session) { }

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

        private decimal fMovementAmount;
        public decimal MovementAmount
        {
            get { return fMovementAmount; }
            set { SetPropertyValue<decimal>("MovementAmount", ref fMovementAmount, value); }
        }

        private string fDescription;
        [Size(255)]
        public string Description
        {
            get { return fDescription; }
            set { SetPropertyValue<string>("Description", ref fDescription, value); }
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

        private fin_documentfinancemaster fDocumentFinanceMaster;
        public fin_documentfinancemaster DocumentFinanceMaster
        {
            get { return fDocumentFinanceMaster; }
            set { SetPropertyValue("DocumentFinanceMaster", ref fDocumentFinanceMaster, value); }
        }

        private fin_documentfinancepayment fDocumentFinancePayment;
        public fin_documentfinancepayment DocumentFinancePayment
        {
            get { return fDocumentFinancePayment; }
            set { SetPropertyValue("DocumentFinancePayment", ref fDocumentFinancePayment, value); }
        }

        private fin_documentfinancetype fDocumentFinanceType;
        public fin_documentfinancetype DocumentFinanceType
        {
            get { return fDocumentFinanceType; }
            set { SetPropertyValue("DocumentFinanceType", ref fDocumentFinanceType, value); }
        }

        private fin_configurationpaymentmethod fPaymentMethod;
        public fin_configurationpaymentmethod PaymentMethod
        {
            get { return fPaymentMethod; }
            set { SetPropertyValue("PaymentMethod", ref fPaymentMethod, value); }
        }

        //WorkSessionPeriod One <> Many WorkSessionMovement
        private pos_worksessionperiod fWorkSessionPeriod;
        [Association(@"WorkSessionPeriodReferencesWorkSessionMovement")]
        public pos_worksessionperiod WorkSessionPeriod
        {
            get { return fWorkSessionPeriod; }
            set { SetPropertyValue("WorkSessionPeriod", ref fWorkSessionPeriod, value); }
        }

        //WorkSessionMovementType One <> Many WorkSessionMovement
        private pos_worksessionmovementtype fWorkSessionMovementType;
        [Association(@"WorkSessionMovementTypeReferencesWorkSessionMovement")]
        public pos_worksessionmovementtype WorkSessionMovementType
        {
            get { return fWorkSessionMovementType; }
            set { SetPropertyValue("WorkSessionMovementType", ref fWorkSessionMovementType, value); }
        }
    }
}
