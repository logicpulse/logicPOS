/* Report Fields
SELECT 
    DocumentTypeOid, 
    DocumentTypeOrd, 
    DocumentTypeCode, 
    DocumentType, 
    EntityOid, 
    EntityName, 
    EntityFiscalNumber, 
    DocumentDate, 
    Date, 
    DocumentNumber, 
    DocumentAmount, 
    DocumentStatus, 
    CreditDebit, 
    Credit, 
    Debit,
    PaymentDocumentNumber,
    PaymentDate,
    IsPayed
FROM 
    view_documentfinancecurrentaccount
;
;*/

using System;

namespace logicpos.financial.library.Classes.Reports.BOs.Customers
{
    [FRBO(Entity = "view_documentfinancecurrentaccount")]    
    class FRBODocumentFinanceCurrentAccount : FRBOBaseObject
    {
        [FRBO(Field = "DocumentTypeOid")]
        //Primary Oid (Required)
        override public string Oid { get; set; }            //DocumentTypeOid AS Oid,

        public uint DocumentTypeOrd { get; set; }
        public uint DocumentTypeCode { get; set; }
        public string DocumentType { get; set; }
        public string EntityOid { get; set; }
        public string EntityName { get; set; }
        public string EntityFiscalNumber { get; set; }
        public string DocumentDate { get; set; }
        public DateTime Date { get; set; }
        public string DocumentNumber { get; set; }
        public decimal DocumentAmount { get; set; }
        public string DocumentStatus { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }

        /* IN005997 and IN005998 */
        public string PaymentDocumentNumber { get; set; }
        public string PaymentDate { get; set; }
        public bool IsPayed { get; set; }
    }
}
