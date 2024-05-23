using logicpos.datalayer.DataLayer.Xpo;

namespace logicpos.financial.library.Classes.Finance
{
    public class DocumentReference
    {
        public fin_documentfinancemaster Reference { get; set; }

        public string Reason { get; set; }

        public DocumentReference(
            fin_documentfinancemaster pDocumentFinanceMaster, 
            string pReason)
        {
            Reference = pDocumentFinanceMaster;
            Reason = pReason;
        }
    }
}
