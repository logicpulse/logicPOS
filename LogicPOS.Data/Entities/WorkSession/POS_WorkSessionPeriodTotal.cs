using System;
using DevExpress.Xpo;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class pos_worksessionperiodtotal : XPGuidObject
    {
        public pos_worksessionperiodtotal() : base() { }
        public pos_worksessionperiodtotal(Session session) : base(session) { }

        private uint fOrd;
        public uint Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        private fin_configurationpaymentmethod fPaymentMethod;
        public fin_configurationpaymentmethod PaymentMethod
        {
            get { return fPaymentMethod; }
            set { SetPropertyValue<fin_configurationpaymentmethod>("PaymentMethod", ref fPaymentMethod, value); }
        }

        private decimal fTotal;
        public decimal Total
        {
            get { return fTotal; }
            set { SetPropertyValue<decimal>("Total", ref fTotal, value); }
        }

        //WorkSessionPeriod One <> Many WorkSessionPeriodTotal
        private pos_worksessionperiod fPeriod;
        [Association(@"WorkSessionPeriodReferencesWorkSessionPeriodTotal")]
        public pos_worksessionperiod Period
        {
            get { return fPeriod; }
            set { SetPropertyValue<pos_worksessionperiod>("Period", ref fPeriod, value); }
        }
    }
}