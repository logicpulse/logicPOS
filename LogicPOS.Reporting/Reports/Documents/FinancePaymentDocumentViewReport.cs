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

using LogicPOS.Reporting.Common;

namespace LogicPOS.Reporting.Reports.Documents
{
    [Report(Entity = "view_documentfinancepayment")]
    public class FinancePaymentDocumentViewReport : ReportData
    {
        [Report(Field = "fpaOid", Hide = true)]
        override public string Oid { get; set; }                                        //fpaOid AS Oid,  

        [Report(Field = "ftmDesignation")]
        public string DocumentTypeDesignation { get; set; }                             //ftmDesignation AS DocumentTypeDesignation,

        [Report(Field = "fmaDocumentNumber")]
        public string DocumentNumber { get; set; }                                      //fmaDocumentNumber AS DocumentNumber,

        [Report(Field = "fmaDocumentDate")]
        public string DocumentDate { get; set; }                                        //fmaDocumentDate AS DocumentDate,

        [Report(Field = "fmaTotalFinal")]
        public decimal DocumentTotal { get; set; }                                      //fmaTotalFinal AS DocumentTotal,
                                                                                        //TK016319 - Certificação Angola - Alterações para teste da AGT 
        [Report(Field = "fmaTotalTax")]
        public decimal TotalTax { get; set; }                                           //fmaTotalTax AS TotalTax,

        //[FRBO(Field = "fmaPayedDate")]
        //public DateTime PayedDate { get; set; }                                       //fmaPayedDate AS PayedDate,

        [Report(Field = "fmpCreditAmount")]
        public decimal CreditAmount { get; set; }                                       //fmpCreditAmount AS CreditAmount,

        [Report(Field = "fmpDebitAmount")]
        public decimal DebitAmount { get; set; }                                        //fmpDebitAmount AS DebitAmount,

        [Report(Field = "fmaPayed")]
        public bool Payed { get; set; }                                                 //fmaPayed AS Payed
    }
}
