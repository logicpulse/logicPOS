using DevExpress.Xpo;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class viewdocumentfinancepaymenttotals : Entity
    {
        public viewdocumentfinancepaymenttotals() : base() { }
        public viewdocumentfinancepaymenttotals(Session session) : base(session) { }

        private string pDocumentOid;
        public string DocumentOid
        {
            get { return pDocumentOid; }
            set { SetPropertyValue<string>("DocumentOid", ref pDocumentOid, value); }
        }

        private decimal pTotal;
        public decimal Total
        {
            get { return pTotal; }
            set { SetPropertyValue<decimal>("Total", ref pTotal, value); }
        }
    }
}