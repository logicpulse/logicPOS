using System.Collections.Generic;

/* USED QUERY 
SELECT 
      fpaOid AS Oid,  
      ftpDesignation AS DocumentTypeDesignation,
      ftpResourceString AS DocumentTypeResourceString,
      ftpResourceStringReport AS DocumentTypeResourceStringReport,  
      fpaPaymentRefNo AS PaymentRefNo,
      fpaPaymentStatus AS PaymentStatus,
      fpaPaymentAmount AS PaymentAmount,
      fpaDocumentDate AS DocumentDate,
      fpaPaymentDate AS PaymentDate,
      fpaExtendedValue AS ExtendedValue,
      cusCode AS EntityCode, 
      cusName AS EntityName, 
      cusAddress AS EntityAddress, 
      cusZipCode AS EntityZipCode, 
      cusCity AS EntityCity, 
      ccoDesignation AS EntityCountry, 
      cusFiscalNumber as EntityFiscalNumber,
      cpmCode AS MethodCode,
      cpmDesignation as MethodDesignation,
      curDesignation as CurrencyDesignation,
      curAcronym as CurrencyAcronym,
      curSymbol as CurrencySymbol,
      fpaExchangeRate AS ExchangeRate,
      fpaNotes AS Notes
FROM 
	view_documentfinancepayment
WHERE 
    fpaOid = '71db09a8-a685-492a-a30c-d410599a0aa1'
;
*/

namespace logicpos.financial.library.Classes.Reports.BOs.Documents
{
    [FRBO(Entity = "view_documentfinancepayment", Limit = 1)]
    public class FRBODocumentFinancePaymentView : FRBOBaseObject
    {
        [FRBO(Field = "fpaOid")]
        override public string Oid { get; set; }                                        //fpaOid AS Oid,  

        [FRBO(Field = "ftpDesignation")]
        public string DocumentTypeDesignation { get; set; }                             //ftpDesignation AS DocumentTypeDesignation,

        [FRBO(Field = "ftpResourceString")]
        public string DocumentTypeResourceString { get; set; }                          //ftpResourceString AS DocumentTypeResourceString,

        [FRBO(Field = "ftpResourceStringReport")]
        public string DocumentTypeResourceStringReport { get; set; }                    //ftpResourceStringReport AS DocumentTypeResourceStringReport,  

        [FRBO(Field = "fpaPaymentRefNo")]
        public string PaymentRefNo { get; set; }                                        //fpaPaymentRefNo AS PaymentRefNo,

        [FRBO(Field = "fpaPaymentStatus")]
        public string PaymentStatus { get; set; }                                       //fpaPaymentStatus AS PaymentStatus,

        [FRBO(Field = "fpaPaymentAmount")]
        public decimal PaymentAmount { get; set; }                                      //fpaPaymentAmount AS PaymentAmount,
		//TK016319 - Certificação Angola - Alterações para teste da AGT
        [FRBO(Field = "fpaTaxPayable")]
        public decimal TaxPayable { get; set; }                                         //fmaTotalTax AS TaxPayable,

        [FRBO(Field = "fpaPaymentDate")]
        public string PaymentDate { get; set; }                                         //fpaPaymentDate AS PaymentDate,

        [FRBO(Field = "fpaDocumentDate")]
        public string DocumentDate { get; set; }                                        //fpaDocumentDate AS DocumentDate,

        [FRBO(Field = "fpaExtendedValue")]
        public string ExtendedValue { get; set; }                                       //fpaExtendedValue AS ExtendedValue,

        [FRBO(Field = "cusCode")]
        public uint EntityCode { get; set; }                                            //cusCode AS EntityCode, 

        [FRBO(Field = "cusName")]
        public string EntityName { get; set; }                                          //cusName AS EntityName, 

        [FRBO(Field = "cusAddress")]
        public string EntityAddress { get; set; }                                       //cusAddress AS EntityAddress, 

        [FRBO(Field = "cusZipCode")]
        public string EntityZipCode { get; set; }                                       //cusZipCode AS EntityZipCode, 

        [FRBO(Field = "cusCity")]
        public string EntityCity { get; set; }                                          //cusCity AS EntityCity,

        [FRBO(Field = "cusLocality")]
        public string EntityLocality { get; set; }                                          //cusCity AS EntityCity, 

        [FRBO(Field = "ccoDesignation")]
        public string EntityCountry { get; set; }                                       //ccoDesignation AS EntityCountry, 

        [FRBO(Field = "cusFiscalNumber")]
        public string EntityFiscalNumber { get; set; }                                  //cusFiscalNumber as EntityFiscalNumber,

        [FRBO(Field = "cpmCode")]
        public uint MethodCode { get; set; }                                            //cpmCode AS MethodCode,

        [FRBO(Field = "cpmDesignation")]
        public string PaymentMethodDesignation { get; set; }                            //cpmDesignation as MethodDesignation,

        [FRBO(Field = "curDesignation")]
        public string CurrencyDesignation { get; set; }                                 //curDesignation as CurrencyDesignation,

        [FRBO(Field = "curAcronym")]
        public string CurrencyAcronym { get; set; }                                     //curAcronym as CurrencyAcronym,

        [FRBO(Field = "curSymbol")]
        public string CurrencySymbol { get; set; }                                      //curSymbol as CurrencySymbol 

        [FRBO(Field = "fpaExchangeRate")]
        public decimal ExchangeRate { get; set; }                                       //fpaExchangeRate AS ExchangeRate,

        [FRBO(Field = "fpaNotes")]
        public string Notes { get; set; }                                               //fpaNotes AS Notes

        //Chield FRBOs Objects
        public List<FRBODocumentFinancePaymentDocumentView> DocumentFinancePaymentDocument { get; set; }
    }
}