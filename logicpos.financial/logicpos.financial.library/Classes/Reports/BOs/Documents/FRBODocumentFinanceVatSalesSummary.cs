/* 
 * /* USED QUERY 
SELECT        fdVat, SUM(fdTotalNet) AS TotalNet, SUM(fdTotalTax) AS TotalTax
FROM            dbo.view_documentfinance
WHERE  ((fmDocumentDate >= '2021-02-01 00:00:00' AND fmDocumentDate <= '2021-08-24 18:04:55') )
AND fdVat IS NOT NULL
GROUP BY fdVat
;

VIEW 
		[dbo].[view_documentfinancecustomerbalancesummary] 
	AS
	    SELECT        fdVat AS VAT, SUM(fdTotalNet) AS TotalNet, SUM(fdTotalTax) AS TotalTax
        FROM            dbo.view_documentfinance
        GROUP BY fdVat
*/

using System;

namespace logicpos.financial.library.Classes.Reports.BOs.Documents
{
    [FRBO(Entity = "view_documentfinance")]    
    class FRBODocumentFinanceVatSalesSummary : FRBOBaseObject
    {
        [FRBO(Field = "fdVat")]
        //Primary Oid (Required)
        override public string Oid { get; set; }
        public string DocumentTypeDesignation { get; set; }
        public decimal Vat { get; set; }
        public decimal TotalNet { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalFinal { get; set; }
        //[FRBO(Hide = true)]
        //public decimal Balance { get; set; }// SUM(Credit) - SUM(Debit) AS BALANCE
        //[FRBO(Hide = true)]
        //public DateTime CustomerSinceDate { get; set; }// MIN([DocumentDate]) as CustomerSinceDate
    }
}