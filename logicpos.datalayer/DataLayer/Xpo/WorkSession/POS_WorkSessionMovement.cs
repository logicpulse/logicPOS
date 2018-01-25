using System;
using DevExpress.Xpo;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class POS_WorkSessionMovement : XPGuidObject
    {
        public POS_WorkSessionMovement() : base() { }
        public POS_WorkSessionMovement(Session session) : base(session) { }

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

        FIN_DocumentFinanceMaster fDocumentFinanceMaster;
        public FIN_DocumentFinanceMaster DocumentFinanceMaster
        {
            get { return fDocumentFinanceMaster; }
            set { SetPropertyValue<FIN_DocumentFinanceMaster>("DocumentFinanceMaster", ref fDocumentFinanceMaster, value); }
        }

        FIN_DocumentFinancePayment fDocumentFinancePayment;
        public FIN_DocumentFinancePayment DocumentFinancePayment
        {
            get { return fDocumentFinancePayment; }
            set { SetPropertyValue<FIN_DocumentFinancePayment>("DocumentFinancePayment", ref fDocumentFinancePayment, value); }
        }

        FIN_DocumentFinanceType fDocumentFinanceType;
        public FIN_DocumentFinanceType DocumentFinanceType
        {
            get { return fDocumentFinanceType; }
            set { SetPropertyValue<FIN_DocumentFinanceType>("DocumentFinanceType", ref fDocumentFinanceType, value); }
        }

        FIN_ConfigurationPaymentMethod fPaymentMethod;
        public FIN_ConfigurationPaymentMethod PaymentMethod
        {
            get { return fPaymentMethod; }
            set { SetPropertyValue<FIN_ConfigurationPaymentMethod>("PaymentMethod", ref fPaymentMethod, value); }
        }

        //WorkSessionPeriod One <> Many WorkSessionMovement
        POS_WorkSessionPeriod fWorkSessionPeriod;
        [Association(@"WorkSessionPeriodReferencesWorkSessionMovement")]
        public POS_WorkSessionPeriod WorkSessionPeriod
        {
            get { return fWorkSessionPeriod; }
            set { SetPropertyValue<POS_WorkSessionPeriod>("WorkSessionPeriod", ref fWorkSessionPeriod, value); }
        }

        //WorkSessionMovementType One <> Many WorkSessionMovement
        POS_WorkSessionMovementType fWorkSessionMovementType;
        [Association(@"WorkSessionMovementTypeReferencesWorkSessionMovement")]
        public POS_WorkSessionMovementType WorkSessionMovementType
        {
            get { return fWorkSessionMovementType; }
            set { SetPropertyValue<POS_WorkSessionMovementType>("WorkSessionMovementType", ref fWorkSessionMovementType, value); }
        }
    }
}
