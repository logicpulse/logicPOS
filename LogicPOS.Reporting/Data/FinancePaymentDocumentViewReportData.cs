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

using LogicPOS.Reporting.Data.Common;

namespace LogicPOS.Reporting.Reports.Data
{
    [ReportData(Entity = "view_documentfinancepayment")]
    public class FinancePaymentDocumentViewReportData : ReportData
    {
        [ReportData(Field = "fpaOid", Hide = true)]
        override public string Oid { get; set; }                                        //fpaOid AS Oid,  

        [ReportData(Field = "ftmDesignation")]
        public string DocumentTypeDesignation { get; set; }                             //ftmDesignation AS DocumentTypeDesignation,

        [ReportData(Field = "fmaDocumentNumber")]
        public string DocumentNumber { get; set; }                                      //fmaDocumentNumber AS DocumentNumber,

        [ReportData(Field = "fmaDocumentDate")]
        public string DocumentDate { get; set; }                                        //fmaDocumentDate AS DocumentDate,

        [ReportData(Field = "fmaTotalFinal")]
        public decimal DocumentTotal { get; set; }                                      //fmaTotalFinal AS DocumentTotal,
                                                                                        //TK016319 - Certificação Angola - Alterações para teste da AGT 
        [ReportData(Field = "fmaTotalTax")]
        public decimal TotalTax { get; set; }                                           //fmaTotalTax AS TotalTax,

        //[FRBO(Field = "fmaPayedDate")]
        //public DateTime PayedDate { get; set; }                                       //fmaPayedDate AS PayedDate,

        [ReportData(Field = "fmpCreditAmount")]
        public decimal CreditAmount { get; set; }                                       //fmpCreditAmount AS CreditAmount,

        [ReportData(Field = "fmpDebitAmount")]
        public decimal DebitAmount { get; set; }                                        //fmpDebitAmount AS DebitAmount,

        [ReportData(Field = "fmaPayed")]
        public bool Payed { get; set; }                                                 //fmaPayed AS Payed
    }
}
