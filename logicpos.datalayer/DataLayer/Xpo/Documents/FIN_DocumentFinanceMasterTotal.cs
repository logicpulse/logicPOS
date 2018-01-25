using System;
using DevExpress.Xpo;

namespace logicpos.datalayer.DataLayer.Xpo
{
    public enum FinanceMasterTotalType
    {
        Tax, Discount
    }

    [DeferredDeletion(false)]
    public class FIN_DocumentFinanceMasterTotal : XPGuidObject
    {
        public FIN_DocumentFinanceMasterTotal() : base() { }
        public FIN_DocumentFinanceMasterTotal(Session session) : base(session) { }

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
        FIN_DocumentFinanceMaster fDocumentMaster;
        [Association(@"DocumentFinanceMasterReferencesDocumentFinanceMasterTotal")]
        public FIN_DocumentFinanceMaster DocumentMaster
        {
            get { return fDocumentMaster; }
            set { SetPropertyValue<FIN_DocumentFinanceMaster>("DocumentMaster", ref fDocumentMaster, value); }
        }
    }
}