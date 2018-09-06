using System;

/* Report Fields
SELECT 
	stkDate, stkDocumentNumber, artCode, artDesignation, stkQuantity, aumAcronym
FROM 
	view_articlestock 
ORDER BY 
	stkDate
;
*/

namespace logicpos.financial.library.Classes.Reports.BOs.Articles
{
    [FRBO(Entity = "view_articlestockmovement")]
    public class FRBOArticleStockMovementView : FRBOBaseObject
    {
        // ArticleFamily
        [FRBO(Field = "afaOid")]
        //Primary Oid (Required)
        override public string Oid { get; set; }                            //afaOid AS Oid,  

        [FRBO(Field = "afaOrd")]
        public uint ArticleFamilyOrd { get; set; }                          //afaOrd AS ArticleFamilyOrd,

        [FRBO(Field = "afaCode")]                                           
        public uint ArticleFamilyCode { get; set; }                         //afaCode AS ArticleFamilyCode,

        [FRBO(Field = "afaDesignation")]
        public string ArticleFamilyDesignation { get; set; }                //afaDesignation AS ArticleFamilyDesignation,

        // ArticleSubFamily
        [FRBO(Field = "asfOid")]
        public string ArticleSubFamily { get; set; }                        //asfOid AS ArticleSubFamily,

        [FRBO(Field = "asfOrd")]
        public uint ArticleSubFamilyOrd { get; set; }                       //asfOrd AS ArticleSubFamilyOrd,

        [FRBO(Field = "asfCode")]
        public uint ArticleSubFamilyCode { get; set; }                      //asfCode AS ArticleSubFamilyCode,

        [FRBO(Field = "asfDesignation")]
        public string ArticleSubFamilyDesignation { get; set; }             //asfDesignation AS ArticleSubFamilyDesignation,

        // Article
        [FRBO(Field = "artOid")]
        public string Article { get; set; }                                 //artOid AS Article,

        [FRBO(Field = "artOrd")]
        public uint ArticleOrd { get; set; }                                //artOrd AS ArticleOrd,

        [FRBO(Field = "artCode")]
        public string ArticleCode { get; set; }                             //artCode AS ArticleCode,

        [FRBO(Field = "artCodeDealer")]
        public string ArticleCodeDealer { get; set; }                       //artCodeDealer AS ArticleCodeDealer,

        [FRBO(Field = "artDesignation")]
        public string ArticleDesignation { get; set; }                      //artDesignation AS ArticleDesignation,

        // ArticleStock
        [FRBO(Field = "stkOid")]
        public string ArticleStock { get; set; }                            //stkOid AS ArticleStock,

        [FRBO(Field = "stkDate")]
        public DateTime ArticleStockDate { get; set; }                      //stkDate AS ArticleStockDate,

        [FRBO(Field = "stkDateDay")]
        public string ArticleStockDateDay { get; set; }                     //stkDate AS ArticleStockDateDay,

        [FRBO(Field = "stkDocumentNumber")]
        public string ArticleStockDocumentNumber { get; set; }              //stkDocumentNumber AS ArticleStockDocumentNumber,

        [FRBO(Field = "stkQuantity")]
        public decimal ArticleStockQuantity { get; set; }                   //stkQuantity AS ArticleStockQuantity,

        // ConfigurationUnitMeasure
        [FRBO(Field = "aumAcronym")]
        public string ConfigurationUnitMeasureaumAcronym { get; set; }      //aumAcronym AS ConfigurationUnitMeasureaumAcronym

        [FRBO(Field = "aumDesignation")]
        public string ConfigurationUnitMeasureDesignation { get; set; }     //aumDesignation AS ConfigurationUnitMeasureDesignation,
    }
}
