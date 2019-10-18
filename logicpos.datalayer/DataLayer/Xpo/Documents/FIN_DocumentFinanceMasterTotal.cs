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

        Decimal fValue;
        public Decimal Value
        {
            get { return fValue; }
            set { SetPropertyValue<Decimal>("Value", ref fValue, value); }
        }

        Decimal fTotal;
        public Decimal Total
        {
            get { return fTotal; }
            set { SetPropertyValue<Decimal>("Total", ref fTotal, value); }
        }

        Decimal fTotalBase;
        public Decimal TotalBase
        {
            get { return fTotalBase; }
            set { SetPropertyValue<Decimal>("TotalBase", ref fTotalBase, value); }
        }

        FinanceMasterTotalType fTotalType;
        public FinanceMasterTotalType TotalType
        {
            get { return fTotalType; }
            set { SetPropertyValue<FinanceMasterTotalType>("TotalType", ref fTotalType, value); }
        }

        //DocumentFinanceMaster One <> Many DocumentFinanceMasterTotal
        fin_documentfinancemaster fDocumentMaster;
        [Association(@"DocumentFinanceMasterReferencesDocumentFinanceMasterTotal")]
        public fin_documentfinancemaster DocumentMaster
        {
            get { return fDocumentMaster; }
            set { SetPropertyValue<fin_documentfinancemaster>("DocumentMaster", ref fDocumentMaster, value); }
        }
    }
}