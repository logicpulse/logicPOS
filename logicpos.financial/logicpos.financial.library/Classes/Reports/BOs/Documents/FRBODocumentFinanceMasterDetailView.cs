namespace logicpos.financial.library.Classes.Reports.BOs.Documents
{
    // Extended FRBODocumentFinanceMasterView, Must redeclare [FRBO(Entity = "view_documentfinance")] we disable Inherited in FRBOAttribute

    [FRBO(Entity = "view_documentfinance")]
    public class FRBODocumentFinanceMasterDetailView : FRBODocumentFinanceMasterView
    {
        //ArticleFamily
        [FRBO(Field = "afFamily")]                                                      //af.Oid AS afFamily,
        public string ArticleFamily { get; set; }

        [FRBO(Field = "afFamilyOrd")]                                                   //af.Ord AS afOrd,
        public uint ArticleFamilyOrd { get; set; }

        [FRBO(Field = "afFamilyCode")]                                                  //af.Code AS afFamilyCode,
        public uint ArticleFamilyCode { get; set; }

        [FRBO(Field = "afFamilyDesignation")]                                           //af.Designation AS afFamilyDesignation,
        public string ArticleFamilyDesignation { get; set; }

        //ArticleSubFamily
        [FRBO(Field = "sfSubFamily")]                                                   //sf.Oid AS sfSubFamily,
        public string ArticleSubFamily { get; set; }

        [FRBO(Field = "sfSubFamilyOrd")]                                                //sf.SubFamilyOrd AS sfSubFamilyOrd,
        public uint ArticleSubFamilyOrd { get; set; }

        [FRBO(Field = "sfSubFamilyCode")]                                               //sf.Code AS sfSubFamilyCode,
        public uint ArticleSubFamilyCode { get; set; }

        [FRBO(Field = "sfSubFamilyDesignation")]                                        //sf.Designation AS sfSubFamilyDesignation,
        public string ArticleSubFamilyDesignation { get; set; }

        // UserDetail
        [FRBO(Field = "udUserDetail")]                                                  //udUserDetail AS UserDetail,
        public string UserDetail { get; set; }

        [FRBO(Field = "udUserDetailOrd")]                                               //udUserDetailOrd AS UserDetailOrd,
        public uint UserDetailOrd { get; set; }

        [FRBO(Field = "udUserDetailCode")]                                              //udUserDetailCode AS UserDetailCode,
        public uint UserDetailCode { get; set; }

        [FRBO(Field = "udUserDetailName")]                                              //udUserDetailName AS UserDetailName,
        public string UserDetailName { get; set; }

        // Terminal
        [FRBO(Field = "trTerminal")]                                                    //trTerminal AS Terminal,
        public string Terminal { get; set; }

        [FRBO(Field = "trTerminalOrd")]                                                 //trTerminalOrd AS TerminalOrd,
        public uint TerminalOrd { get; set; }

        [FRBO(Field = "trTerminalCode")]                                                //trTerminalCode AS TerminalCode,
        public uint TerminalCode { get; set; }

        [FRBO(Field = "trTerminalDesignation")]                                         //trTerminalDesignation AS TerminalDesignation,
        public string TerminalDesignation { get; set; }

        // Place
        [FRBO(Field = "cpPlace")]                                                       //cpPlace AS Place,
        public string Place { get; set; }

        [FRBO(Field = "cpPlaceOrd")]                                                    //cpPlaceOrd AS PlaceOrd,
        public uint PlaceOrd { get; set; }

        [FRBO(Field = "cpPlaceCode")]                                                   //cpPlaceCode AS PlaceCode,
        public uint PlaceCode { get; set; }

        [FRBO(Field = "cpPlaceDesignation")]                                            //cpPlaceDesignation AS PlaceDesignation,
        public string PlaceDesignation { get; set; }

        // PlaceTable
        [FRBO(Field = "dmPlaceTable")]                                                  //ctPlaceTable AS PlaceTable,
        public string PlaceTable { get; set; }

        [FRBO(Field = "ctPlaceTableOrd")]                                               //ctPlaceTableOrd AS PlaceTableOrd,
        public uint PlaceTableOrd { get; set; }

        [FRBO(Field = "ctPlaceTableCode")]                                              //ctPlaceTableCode AS PlaceTableCode,
        public uint PlaceTableCode { get; set; }

        [FRBO(Field = "ctPlaceTableDesignation")]                                       //ctPlaceTableDesignation AS PlaceTableDesignation,
        public string PlaceTableDesignation { get; set; }

        //Article
        [FRBO(Field = "fdArticle")]                                                     //fd.Article AS fdArticle,
        public string Article { get; set; }

        [FRBO(Field = "fdOrd")]                                                         //fd.Ord AS fdOrd,
        public uint ArticleOrd { get; set; }

        [FRBO(Field = "fdCode")]                                                        //fd.Code AS fdCode,
        public string ArticleCode { get; set; }

        [FRBO(Field = "fdDesignation")]                                                 //fd.Designation AS fdDesignation,
        public string ArticleDesignation { get; set; }

        [FRBO(Field = "fdQuantity")]                                                    //fd.Quantity AS fdQuantity,
        public decimal ArticleQuantity { get; set; }

        [FRBO(Field = "fdUnitMeasure")]                                                 //fd.UnitMeasure AS fdUnitMeasure,
        public string ArticleUnitMeasure { get; set; }

        [FRBO(Field = "fdPrice")]                                                       //fd.Price AS fdPrice,
        public decimal ArticlePrice { get; set; }

        [FRBO(Field = "fdPriceWithDiscount")]                                           //(fd.Price - ((fd.Price * fd.Discount) / 100)) AS fdPriceWithDiscount,
        public decimal ArticlePriceWithDiscount { get; set; }

        [FRBO(Field = "fdVat")]                                                         //fd.Vat AS fdVat,
        public decimal ArticleVat { get; set; }

        [FRBO(Field = "fdDiscount")]                                                    //fd.Discount AS fdDiscount,
        public decimal ArticleDiscount { get; set; }

        [FRBO(Field = "arPriceWithVat")]                                                //ar.PriceWithVat AS arPriceWithVat,
        public bool ArticlePriceWithVat { get; set; }

        [FRBO(Field = "fdTotalNet")]                                                    //fd.TotalNet AS fdTotalNet,
        public decimal ArticleTotalNet { get; set; }

        [FRBO(Field = "fdTotalGross")]                                                  //fd.TotalGross AS fdTotalGross,
        public decimal ArticleTotalGross { get; set; }

        [FRBO(Field = "fdTotalDiscount")]                                               //fd.TotalDiscount AS fdTotalDiscount,
        public decimal ArticleTotalDiscount { get; set; }

        [FRBO(Field = "fdTotalTax")]                                                    //fd.TotalTax AS fdTotalTax,
        public decimal ArticleTotalTax { get; set; }

        [FRBO(Field = "fdTotalFinal")]                                                  //fd.TotalFinal AS fdTotalFinal,
        public decimal ArticleTotalFinal { get; set; }

        [FRBO(Field = "fdVatExemptionReason")]                                          //fd.VatExemptionReason AS fdVatExemptionReason,
        public string ArticleVatExemptionReason { get; set; }

        [FRBO(Field = "fdVatExemptionReasonDesignation")]                               //fd.VatExemptionReasonDesignation AS fdVatExemptionReasonDesignation,
        public string ArticleVatExemptionReasonDesignation { get; set; }
    }
}
