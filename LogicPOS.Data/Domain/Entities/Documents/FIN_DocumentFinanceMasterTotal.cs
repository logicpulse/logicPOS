using DevExpress.Xpo;

namespace LogicPOS.Domain.Entities
{
    public enum FinanceMasterTotalType
    {
        Tax, Discount
    }

    [DeferredDeletion(false)]
    public class fin_documentfinancemastertotal : Entity
    {
        public fin_documentfinancemastertotal() : base() { }
        public fin_documentfinancemastertotal(Session session) : base(session) { }

        private decimal fValue;
        public decimal Value
        {
            get { return fValue; }
            set { SetPropertyValue<decimal>("Value", ref fValue, value); }
        }

        private decimal fTotal;
        public decimal Total
        {
            get { return fTotal; }
            set { SetPropertyValue<decimal>("Total", ref fTotal, value); }
        }

        private decimal fTotalBase;
        public decimal TotalBase
        {
            get { return fTotalBase; }
            set { SetPropertyValue<decimal>("TotalBase", ref fTotalBase, value); }
        }

        private FinanceMasterTotalType fTotalType;
        public FinanceMasterTotalType TotalType
        {
            get { return fTotalType; }
            set { SetPropertyValue("TotalType", ref fTotalType, value); }
        }

        //DocumentFinanceMaster One <> Many DocumentFinanceMasterTotal
        private fin_documentfinancemaster fDocumentMaster;
        [Association(@"DocumentFinanceMasterReferencesDocumentFinanceMasterTotal")]
        public fin_documentfinancemaster DocumentMaster
        {
            get { return fDocumentMaster; }
            set { SetPropertyValue("DocumentMaster", ref fDocumentMaster, value); }
        }
    }
}