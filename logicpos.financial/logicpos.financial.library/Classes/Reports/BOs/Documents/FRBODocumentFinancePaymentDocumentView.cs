using System;

/* USED QUERY 
SELECT 
      fpaOid AS Oid,  
      ftmDesignation AS DocumentTypeDesignation,
      fmaDocumentNumber AS DocumentNumber,
      fmaDocumentDate AS DocumentDate,
      fmaTotalFinal AS DocumentTotal,
      fmpCreditAmount AS CreditAmount,
      fmpDebitAmount AS DebitAmount,
      #fmaPayedDate AS PayedDate,
      fmaPayed AS Payed
FROM 
      view_documentfinancepayment 
WHERE 
      fpaOid = '71db09a8-a685-492a-a30c-d410599a0aa1' 
ORDER BY
      ftpCode, fmaDocumentNumber
;
*/

namespace logicpos.financial.library.Classes.Reports.BOs.Documents
{
    [FRBO(Entity = "view_documentfinancepayment")]
    public class FRBODocumentFinancePaymentDocumentView : FRBOBaseObject
    {
        [FRBO(Field = "fpaOid", Hide = true)]
        override public string Oid { get; set; }                                        //fpaOid AS Oid,  

        [FRBO(Field = "ftmDesignation")]
        public string DocumentTypeDesignation { get; set; }                             //ftmDesignation AS DocumentTypeDesignation,

        [FRBO(Field = "fmaDocumentNumber")]
        public string DocumentNumber { get; set; }                                      //fmaDocumentNumber AS DocumentNumber,

        [FRBO(Field = "fmaDocumentDate")]
        public string DocumentDate { get; set; }                                        //fmaDocumentDate AS DocumentDate,

        [FRBO(Field = "fmaTotalFinal")]
        public decimal DocumentTotal { get; set; }                                      //fmaTotalFinal AS DocumentTotal,
		//TK016319 - Certificação Angola - Alterações para teste da AGT 
        [FRBO(Field = "fmaTotalTax")]
        public decimal TotalTax { get; set; }                                           //fmaTotalTax AS TotalTax,

        //[FRBO(Field = "fmaPayedDate")]
        //public DateTime PayedDate { get; set; }                                       //fmaPayedDate AS PayedDate,

        [FRBO(Field = "fmpCreditAmount")]
        public decimal CreditAmount { get; set; }                                       //fmpCreditAmount AS CreditAmount,

        [FRBO(Field = "fmpDebitAmount")]
        public decimal DebitAmount { get; set; }                                        //fmpDebitAmount AS DebitAmount,

        [FRBO(Field = "fmaPayed")]
        public bool Payed { get; set; }                                                 //fmaPayed AS Payed
    }
}
