using DevExpress.Xpo;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class viewdocumentfinancepaymenttotals : XPGuidObject
    {
        public viewdocumentfinancepaymenttotals() : base() { }
        public viewdocumentfinancepaymenttotals(Session session) : base(session) { }

        string pDocumentOid;
        public string DocumentOid
        {
            get { return pDocumentOid; }
            set { SetPropertyValue<string>("DocumentOid", ref pDocumentOid, value); }
        }

        Decimal pTotal;
        public Decimal Total
        {
            get { return pTotal; }
            set { SetPropertyValue<Decimal>("Total", ref pTotal, value); }
        }
    }
}