using LogicPOS.Domain.Entities;

namespace LogicPOS.Finance.Dtos
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
