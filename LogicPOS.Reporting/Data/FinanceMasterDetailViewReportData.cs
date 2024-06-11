using LogicPOS.Reporting.Data.Common;

namespace LogicPOS.Reporting.Reports.Data
{
    // Extended FRBODocumentFinanceMasterView, Must redeclare [FRBO(Entity = "view_documentfinance")] we disable Inherited in FRBOAttribute

    [ReportData(Entity = "view_documentfinance")]
    public class FinanceMasterDetailViewReportData : FinanceMasterViewReportData
    {
        //ArticleFamily
        [ReportData(Field = "afFamily")]                                                      //af.Oid AS afFamily,
        public string ArticleFamily { get; set; }

        [ReportData(Field = "afFamilyOrd")]                                                   //af.Ord AS afOrd,
        public uint ArticleFamilyOrd { get; set; }

        [ReportData(Field = "afFamilyCode")]                                                  //af.Code AS afFamilyCode,
        public uint ArticleFamilyCode { get; set; }

        [ReportData(Field = "afFamilyDesignation")]                                           //af.Designation AS afFamilyDesignation,
        public string ArticleFamilyDesignation { get; set; }

        //ArticleSubFamily
        [ReportData(Field = "sfSubFamily")]                                                   //sf.Oid AS sfSubFamily,
        public string ArticleSubFamily { get; set; }

        [ReportData(Field = "sfSubFamilyOrd")]                                                //sf.SubFamilyOrd AS sfSubFamilyOrd,
        public uint ArticleSubFamilyOrd { get; set; }

        [ReportData(Field = "sfSubFamilyCode")]                                               //sf.Code AS sfSubFamilyCode,
        public uint ArticleSubFamilyCode { get; set; }

        [ReportData(Field = "sfSubFamilyDesignation")]                                        //sf.Designation AS sfSubFamilyDesignation,
        public string ArticleSubFamilyDesignation { get; set; }

        // UserDetail
        [ReportData(Field = "udUserDetail")]                                                  //udUserDetail AS UserDetail,
        public string UserDetail { get; set; }

        [ReportData(Field = "udUserDetailOrd")]                                               //udUserDetailOrd AS UserDetailOrd,
        public uint UserDetailOrd { get; set; }

        [ReportData(Field = "udUserDetailCode")]                                              //udUserDetailCode AS UserDetailCode,
        public uint UserDetailCode { get; set; }

        [ReportData(Field = "udUserDetailName")]                                              //udUserDetailName AS UserDetailName,
        public string UserDetailName { get; set; }

        // Terminal
        [ReportData(Field = "trTerminal")]                                                    //trTerminal AS Terminal,
        public string Terminal { get; set; }

        [ReportData(Field = "trTerminalOrd")]                                                 //trTerminalOrd AS TerminalOrd,
        public uint TerminalOrd { get; set; }

        [ReportData(Field = "trTerminalCode")]                                                //trTerminalCode AS TerminalCode,
        public uint TerminalCode { get; set; }

        [ReportData(Field = "trTerminalDesignation")]                                         //trTerminalDesignation AS TerminalDesignation,
        public string TerminalDesignation { get; set; }

        // Place
        [ReportData(Field = "cpPlace")]                                                       //cpPlace AS Place,
        public string Place { get; set; }

        [ReportData(Field = "cpPlaceOrd")]                                                    //cpPlaceOrd AS PlaceOrd,
        public uint PlaceOrd { get; set; }

        [ReportData(Field = "cpPlaceCode")]                                                   //cpPlaceCode AS PlaceCode,
        public uint PlaceCode { get; set; }

        [ReportData(Field = "cpPlaceDesignation")]                                            //cpPlaceDesignation AS PlaceDesignation,
        public string PlaceDesignation { get; set; }

        // PlaceTable
        [ReportData(Field = "dmPlaceTable")]                                                  //ctPlaceTable AS PlaceTable,
        public string PlaceTable { get; set; }

        [ReportData(Field = "ctPlaceTableOrd")]                                               //ctPlaceTableOrd AS PlaceTableOrd,
        public uint PlaceTableOrd { get; set; }

        [ReportData(Field = "ctPlaceTableCode")]                                              //ctPlaceTableCode AS PlaceTableCode,
        public uint PlaceTableCode { get; set; }

        [ReportData(Field = "ctPlaceTableDesignation")]                                       //ctPlaceTableDesignation AS PlaceTableDesignation,
        public string PlaceTableDesignation { get; set; }

        //Article
        [ReportData(Field = "fdArticle")]                                                     //fd.Article AS fdArticle,
        public string Article { get; set; }

        [ReportData(Field = "fdOrd")]                                                         //fd.Ord AS fdOrd,
        public uint ArticleOrd { get; set; }

        [ReportData(Field = "fdCode")]                                                        //fd.Code AS fdCode,
        public string ArticleCode { get; set; }

        [ReportData(Field = "fdDesignation")]                                                 //fd.Designation AS fdDesignation,
        public string ArticleDesignation { get; set; }

        [ReportData(Field = "fdQuantity")]                                                    //fd.Quantity AS fdQuantity,
        public decimal ArticleQuantity { get; set; }

        [ReportData(Field = "fdUnitMeasure")]                                                 //fd.UnitMeasure AS fdUnitMeasure,
        public string ArticleUnitMeasure { get; set; }

        [ReportData(Field = "fdPrice")]                                                       //fd.Price AS fdPrice,
        public decimal ArticlePrice { get; set; }

        [ReportData(Field = "fdPriceWithDiscount")]                                           //(fd.Price - ((fd.Price * fd.Discount) / 100)) AS fdPriceWithDiscount,
        public decimal ArticlePriceWithDiscount { get; set; }

        [ReportData(Field = "fdVat")]                                                         //fd.Vat AS fdVat,
        public decimal ArticleVat { get; set; }

        [ReportData(Field = "fdDiscount")]                                                    //fd.Discount AS fdDiscount,
        public decimal ArticleDiscount { get; set; }

        [ReportData(Field = "arPriceWithVat")]                                                //ar.PriceWithVat AS arPriceWithVat,
        public bool ArticlePriceWithVat { get; set; }

        [ReportData(Field = "fdTotalNet")]                                                    //fd.TotalNet AS fdTotalNet,
        public decimal ArticleTotalNet { get; set; }

        [ReportData(Field = "fdTotalGross")]                                                  //fd.TotalGross AS fdTotalGross,
        public decimal ArticleTotalGross { get; set; }

        [ReportData(Field = "fdTotalDiscount")]                                               //fd.TotalDiscount AS fdTotalDiscount,
        public decimal ArticleTotalDiscount { get; set; }

        [ReportData(Field = "fdTotalTax")]                                                    //fd.TotalTax AS fdTotalTax,
        public decimal ArticleTotalTax { get; set; }

        [ReportData(Field = "fdTotalFinal")]                                                  //fd.TotalFinal AS fdTotalFinal,
        public decimal ArticleTotalFinal { get; set; }

        [ReportData(Field = "fdVatExemptionReason")]                                          //fd.VatExemptionReason AS fdVatExemptionReason,
        public string ArticleVatExemptionReason { get; set; }

        [ReportData(Field = "fdVatExemptionReasonDesignation")]                               //fd.VatExemptionReasonDesignation AS fdVatExemptionReasonDesignation,
        public string ArticleVatExemptionReasonDesignation { get; set; }

        [ReportData(Field = "cfVatCode")]
        public string ArticleVatDesignation { get; set; }

        [ReportData(Field = "cvTaxType")]
        public string ArticleVatCode { get; set; }
    }
}
