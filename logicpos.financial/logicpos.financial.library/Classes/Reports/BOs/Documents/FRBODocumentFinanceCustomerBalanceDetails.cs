/* 
VIEW [dbo].[view_documentfinancecustomerbalancedetails]  AS
	SELECT 
		ft.Oid AS DocumentTypeOid, 
		ft.Ord AS DocumentTypeOrd, 
		ft.Code AS DocumentTypeCode, 
		ft.Designation AS DocumentType, 
		fm.EntityOid AS EntityOid,
		fm.EntityName AS EntityName,
		fm.EntityFiscalNumber AS EntityFiscalNumber,
		fm.DocumentDate AS DocumentDate,
		fm.Date AS Date,
		fm.DocumentNumber AS DocumentNumber,
		fm.TotalFinal AS DocumentAmount,
		fm.DocumentStatusStatus AS DocumentStatus,
		ft.CreditDebit AS CreditDebit, 
		(SELECT fm.TotalFinal WHERE ft.CreditDebit = 1 OR ft.CreditDebit = 0) AS Credit,
		(SELECT fm.TotalFinal WHERE ft.CreditDebit = -1 OR ft.CreditDebit = 0) AS Debit,
		(SELECT fmaster.Payed FROM fin_documentfinancemaster AS fmaster WHERE fm.DocumentChild = fmaster.Oid) AS IsPayed,
		(' ') as DocumentPayedNumber
	FROM 
		(fin_documentfinancemaster as fm
		LEFT JOIN fin_documentfinancetype AS ft ON ((fm.DocumentType = ft.Oid))
		LEFT JOIN fin_documentfinancemasterpayment AS DocFinMasterPay ON ((fm.Oid = DocFinMasterPay.DocumentFinanceMaster))
		LEFT JOIN fin_documentfinancepayment AS DocFinPay ON ((DocFinMasterPay.DocumentFinancePayment = DocFinPay.Oid)))
	WHERE 
	  (
		fm.DocumentType = '7af04618-74a6-42a3-aaba-454b7076f5a6' OR
		fm.DocumentType = 'f8878cf5-0f88-4270-8a55-1fc2488d81a2' OR
		fm.DocumentType = '2c69b109-318a-4375-a573-28e5984b6503' OR
		fm.DocumentType = '09b6aa6e-dc0e-41fd-8dbe-8678a3d11cbc' OR
		fm.DocumentType = '3942d940-ed13-4a62-a352-97f1ce006d8a' OR
		fm.DocumentType = 'fa924162-beed-4f2f-938d-919deafb7d47' OR
		fm.DocumentType = 'b8554d36-642a-4083-b608-8f1da35f0fec' 
	  )
	  AND (
		fm.DocumentStatusStatus <> 'A' AND (fm.Disabled = 0 OR fm.Disabled IS NULL)
	  )
	UNION
	SELECT 
		ft.Oid AS DocumentTypeOid,
		ft.Ord AS DocumentTypeOrd,
		ft.Code AS DocumentTypeCode,
		ft.Designation AS DocumentType,
		cu.Oid AS EntityOid, 
		cu.Name AS EntityName,
		cu.FiscalNumber AS EntityFiscalNumber,
		fp.DocumentDate AS DocumentDate, 
		fp.CreatedAt AS Date,
		fp.PaymentRefNo AS DocumentNumber, 
		fp.PaymentAmount AS DocumentAmount, 
		fp.PaymentStatus AS DocumentStatus,
		ft.CreditDebit AS CreditDebit, 
		(fp.PaymentAmount) AS Credit,
		(0) AS Debit,
		(0) AS IsPayed,
		docFinMaster.DocumentNumber as DocumentPayedNumber
	FROM 
		(fin_documentfinancepayment AS fp
		LEFT JOIN fin_documentfinancetype AS ft ON (fp.DocumentType = ft.Oid)
		LEFT JOIN erp_customer AS cu ON (cu.Oid = fp.EntityOid)
		LEFT JOIN fin_documentfinancemasterpayment AS docFinMasterPayment on (fp.Oid = docFinMasterPayment.DocumentFinancePayment)
		LEFT JOIN fin_documentfinancemaster AS docFinMaster on (docFinMasterPayment.DocumentFinanceMaster = docFinMaster.Oid)
		)
	WHERE   
		fp.PaymentStatus <> 'A' AND (fp.Disabled = 0 OR fp.Disabled IS NULL)
;
*/
using System;

namespace logicpos.financial.library.Classes.Reports.BOs.Documents
{
    [FRBO(Entity = "view_documentfinancecustomerbalancedetails")]    
    class FRBODocumentFinanceCustomerBalanceDetails: FRBOBaseObject
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
        public decimal DocumentAmountRound { get; set; }/* IN009206 */
        public decimal TotalGross { get; set; }/* IN009206 */
        public decimal TotalNet { get; set; }/* IN009206 */
        public string DocumentStatus { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public int CreditDebit { get; set; }/* IN009206 */
        public string PaymentDocumentReference { get; set; }
        public decimal TotalTax { get; set; }/* IN009206 */
        [FRBO(Hide = true)]
        public decimal Balance { get; set; }// SUM(Credit) - SUM(Debit) AS BALANCE
        [FRBO(Hide = true)]
        public DateTime CustomerSinceDate { get; set; }// MIN([DocumentDate]) as CustomerSinceDate
    }
}