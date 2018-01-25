using System;
using DevExpress.Xpo;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class POS_WorkSessionPeriodTotal : XPGuidObject
    {
        public POS_WorkSessionPeriodTotal() : base() { }
        public POS_WorkSessionPeriodTotal(Session session) : base(session) { }

        UInt32 fOrd;
        public UInt32 Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        FIN_ConfigurationPaymentMethod fPaymentMethod;
        public FIN_ConfigurationPaymentMethod PaymentMethod
        {
            get { return fPaymentMethod; }
            set { SetPropertyValue<FIN_ConfigurationPaymentMethod>("PaymentMethod", ref fPaymentMethod, value); }
        }

        decimal fTotal;
        public decimal Total
        {
            get { return fTotal; }
            set { SetPropertyValue<decimal>("Total", ref fTotal, value); }
        }

        //WorkSessionPeriod One <> Many WorkSessionPeriodTotal
        POS_WorkSessionPeriod fPeriod;
        [Association(@"WorkSessionPeriodReferencesWorkSessionPeriodTotal")]
        public POS_WorkSessionPeriod Period
        {
            get { return fPeriod; }
            set { SetPropertyValue<POS_WorkSessionPeriod>("Period", ref fPeriod, value); }
        }
    }
}