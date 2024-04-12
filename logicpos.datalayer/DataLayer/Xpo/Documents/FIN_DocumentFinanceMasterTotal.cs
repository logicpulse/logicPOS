using System;
using DevExpress.Xpo;

namespace logicpos.datalayer.DataLayer.Xpo
{
    public enum FinanceMasterTotalType
    {
        Tax, Discount
    }

    [DeferredDeletion(false)]
    public class fin_documentfinancemastertotal : XPGuidObject
    {
        public fin_documentfinancemastertotal() : base() { }
        public fin_documentfinancemastertotal(Session session) : base(session) { }

        private Decimal fValue;
        public Decimal Value
        {
            get { return fValue; }
            set { SetPropertyValue<Decimal>("Value", ref fValue, value); }
        }

        private Decimal fTotal;
        public Decimal Total
        {
            get { return fTotal; }
            set { SetPropertyValue<Decimal>("Total", ref fTotal, value); }
        }

        private Decimal fTotalBase;
        public Decimal TotalBase
        {
            get { return fTotalBase; }
            set { SetPropertyValue<Decimal>("TotalBase", ref fTotalBase, value); }
        }

        private FinanceMasterTotalType fTotalType;
        public FinanceMasterTotalType TotalType
        {
            get { return fTotalType; }
            set { SetPropertyValue<FinanceMasterTotalType>("TotalType", ref fTotalType, value); }
        }

        //DocumentFinanceMaster One <> Many DocumentFinanceMasterTotal
        private fin_documentfinancemaster fDocumentMaster;
        [Association(@"DocumentFinanceMasterReferencesDocumentFinanceMasterTotal")]
        public fin_documentfinancemaster DocumentMaster
        {
            get { return fDocumentMaster; }
            set { SetPropertyValue<fin_documentfinancemaster>("DocumentMaster", ref fDocumentMaster, value); }
        }
    }
}