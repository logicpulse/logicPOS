using LogicPOS.Reporting.Common;

namespace LogicPOS.Reporting.Reports.Documents
{
    // Extended FRBODocumentFinanceMasterView, Must redeclare [FRBO(Entity = "view_documentfinance")] we disable Inherited in FRBOAttribute

    [Report(Entity = "view_documentfinance")]
    public class FinanceMasterDetailViewReportData : FinanceMasterViewReport
    {
        //ArticleFamily
        [Report(Field = "afFamily")]                                                      //af.Oid AS afFamily,
        public string ArticleFamily { get; set; }

        [Report(Field = "afFamilyOrd")]                                                   //af.Ord AS afOrd,
        public uint ArticleFamilyOrd { get; set; }

        [Report(Field = "afFamilyCode")]                                                  //af.Code AS afFamilyCode,
        public uint ArticleFamilyCode { get; set; }

        [Report(Field = "afFamilyDesignation")]                                           //af.Designation AS afFamilyDesignation,
        public string ArticleFamilyDesignation { get; set; }

        //ArticleSubFamily
        [Report(Field = "sfSubFamily")]                                                   //sf.Oid AS sfSubFamily,
        public string ArticleSubFamily { get; set; }

        [Report(Field = "sfSubFamilyOrd")]                                                //sf.SubFamilyOrd AS sfSubFamilyOrd,
        public uint ArticleSubFamilyOrd { get; set; }

        [Report(Field = "sfSubFamilyCode")]                                               //sf.Code AS sfSubFamilyCode,
        public uint ArticleSubFamilyCode { get; set; }

        [Report(Field = "sfSubFamilyDesignation")]                                        //sf.Designation AS sfSubFamilyDesignation,
        public string ArticleSubFamilyDesignation { get; set; }

        // UserDetail
        [Report(Field = "udUserDetail")]                                                  //udUserDetail AS UserDetail,
        public string UserDetail { get; set; }

        [Report(Field = "udUserDetailOrd")]                                               //udUserDetailOrd AS UserDetailOrd,
        public uint UserDetailOrd { get; set; }

        [Report(Field = "udUserDetailCode")]                                              //udUserDetailCode AS UserDetailCode,
        public uint UserDetailCode { get; set; }

        [Report(Field = "udUserDetailName")]                                              //udUserDetailName AS UserDetailName,
        public string UserDetailName { get; set; }

        // Terminal
        [Report(Field = "trTerminal")]                                                    //trTerminal AS Terminal,
        public string Terminal { get; set; }

        [Report(Field = "trTerminalOrd")]                                                 //trTerminalOrd AS TerminalOrd,
        public uint TerminalOrd { get; set; }

        [Report(Field = "trTerminalCode")]                                                //trTerminalCode AS TerminalCode,
        public uint TerminalCode { get; set; }

        [Report(Field = "trTerminalDesignation")]                                         //trTerminalDesignation AS TerminalDesignation,
        public string TerminalDesignation { get; set; }

        // Place
        [Report(Field = "cpPlace")]                                                       //cpPlace AS Place,
        public string Place { get; set; }

        [Report(Field = "cpPlaceOrd")]                                                    //cpPlaceOrd AS PlaceOrd,
        public uint PlaceOrd { get; set; }

        [Report(Field = "cpPlaceCode")]                                                   //cpPlaceCode AS PlaceCode,
        public uint PlaceCode { get; set; }

        [Report(Field = "cpPlaceDesignation")]                                            //cpPlaceDesignation AS PlaceDesignation,
        public string PlaceDesignation { get; set; }

        // PlaceTable
        [Report(Field = "dmPlaceTable")]                                                  //ctPlaceTable AS PlaceTable,
        public string PlaceTable { get; set; }

        [Report(Field = "ctPlaceTableOrd")]                                               //ctPlaceTableOrd AS PlaceTableOrd,
        public uint PlaceTableOrd { get; set; }

        [Report(Field = "ctPlaceTableCode")]                                              //ctPlaceTableCode AS PlaceTableCode,
        public uint PlaceTableCode { get; set; }

        [Report(Field = "ctPlaceTableDesignation")]                                       //ctPlaceTableDesignation AS PlaceTableDesignation,
        public string PlaceTableDesignation { get; set; }

        //Article
        [Report(Field = "fdArticle")]                                                     //fd.Article AS fdArticle,
        public string Article { get; set; }

        [Report(Field = "fdOrd")]                                                         //fd.Ord AS fdOrd,
        public uint ArticleOrd { get; set; }

        [Report(Field = "fdCode")]                                                        //fd.Code AS fdCode,
        public string ArticleCode { get; set; }

        [Report(Field = "fdDesignation")]                                                 //fd.Designation AS fdDesignation,
        public string ArticleDesignation { get; set; }

        [Report(Field = "fdQuantity")]                                                    //fd.Quantity AS fdQuantity,
        public decimal ArticleQuantity { get; set; }

        [Report(Field = "fdUnitMeasure")]                                                 //fd.UnitMeasure AS fdUnitMeasure,
        public string ArticleUnitMeasure { get; set; }

        [Report(Field = "fdPrice")]                                                       //fd.Price AS fdPrice,
        public decimal ArticlePrice { get; set; }

        [Report(Field = "fdPriceWithDiscount")]                                           //(fd.Price - ((fd.Price * fd.Discount) / 100)) AS fdPriceWithDiscount,
        public decimal ArticlePriceWithDiscount { get; set; }

        [Report(Field = "fdVat")]                                                         //fd.Vat AS fdVat,
        public decimal ArticleVat { get; set; }

        [Report(Field = "fdDiscount")]                                                    //fd.Discount AS fdDiscount,
        public decimal ArticleDiscount { get; set; }

        [Report(Field = "arPriceWithVat")]                                                //ar.PriceWithVat AS arPriceWithVat,
        public bool ArticlePriceWithVat { get; set; }

        [Report(Field = "fdTotalNet")]                                                    //fd.TotalNet AS fdTotalNet,
        public decimal ArticleTotalNet { get; set; }

        [Report(Field = "fdTotalGross")]                                                  //fd.TotalGross AS fdTotalGross,
        public decimal ArticleTotalGross { get; set; }

        [Report(Field = "fdTotalDiscount")]                                               //fd.TotalDiscount AS fdTotalDiscount,
        public decimal ArticleTotalDiscount { get; set; }

        [Report(Field = "fdTotalTax")]                                                    //fd.TotalTax AS fdTotalTax,
        public decimal ArticleTotalTax { get; set; }

        [Report(Field = "fdTotalFinal")]                                                  //fd.TotalFinal AS fdTotalFinal,
        public decimal ArticleTotalFinal { get; set; }

        [Report(Field = "fdVatExemptionReason")]                                          //fd.VatExemptionReason AS fdVatExemptionReason,
        public string ArticleVatExemptionReason { get; set; }

        [Report(Field = "fdVatExemptionReasonDesignation")]                               //fd.VatExemptionReasonDesignation AS fdVatExemptionReasonDesignation,
        public string ArticleVatExemptionReasonDesignation { get; set; }

        [Report(Field = "cfVatCode")]
        public string ArticleVatDesignation { get; set; }

        [Report(Field = "cvTaxType")]
        public string ArticleVatCode { get; set; }
    }
}
