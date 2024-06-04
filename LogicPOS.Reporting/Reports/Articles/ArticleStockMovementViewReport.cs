using LogicPOS.Reporting.Common;
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

namespace LogicPOS.Reporting.Reports.Articles
{
    [Report(Entity = "view_articlestockmovement")]
    public class ArticleStockMovementViewReport : ReportBase
    {
        // ArticleFamily
        [Report(Field = "afaOid")]
        //Primary Oid (Required)
        override public string Oid { get; set; }                            //afaOid AS Oid,  

        [Report(Field = "afaOrd")]
        public uint ArticleFamilyOrd { get; set; }                          //afaOrd AS ArticleFamilyOrd,

        [Report(Field = "afaCode")]
        public uint ArticleFamilyCode { get; set; }                         //afaCode AS ArticleFamilyCode,

        [Report(Field = "afaDesignation")]
        public string ArticleFamilyDesignation { get; set; }                //afaDesignation AS ArticleFamilyDesignation,

        // ArticleSubFamily
        [Report(Field = "asfOid")]
        public string ArticleSubFamily { get; set; }                        //asfOid AS ArticleSubFamily,

        [Report(Field = "asfOrd")]
        public uint ArticleSubFamilyOrd { get; set; }                       //asfOrd AS ArticleSubFamilyOrd,

        [Report(Field = "asfCode")]
        public uint ArticleSubFamilyCode { get; set; }                      //asfCode AS ArticleSubFamilyCode,

        [Report(Field = "asfDesignation")]
        public string ArticleSubFamilyDesignation { get; set; }             //asfDesignation AS ArticleSubFamilyDesignation,

        // Article
        [Report(Field = "artOid")]
        public string Article { get; set; }                                 //artOid AS Article,

        [Report(Field = "artOrd")]
        public uint ArticleOrd { get; set; }                                //artOrd AS ArticleOrd,

        [Report(Field = "artCode")]
        public string ArticleCode { get; set; }                             //artCode AS ArticleCode,

        [Report(Field = "artCodeDealer")]
        public string ArticleCodeDealer { get; set; }                       //artCodeDealer AS ArticleCodeDealer,

        [Report(Field = "artDesignation")]
        public string ArticleDesignation { get; set; }                      //artDesignation AS ArticleDesignation,

        // ArticleStock
        [Report(Field = "stkOid")]
        public string ArticleStock { get; set; }                            //stkOid AS ArticleStock,

        [Report(Field = "stkDate")]
        public DateTime ArticleStockDate { get; set; }                      //stkDate AS ArticleStockDate,

        [Report(Field = "stkDateDay")]
        public string ArticleStockDateDay { get; set; }                     //stkDate AS ArticleStockDateDay,

        [Report(Field = "stkDocumentNumber")]
        public string ArticleStockDocumentNumber { get; set; }              //stkDocumentNumber AS ArticleStockDocumentNumber,

        [Report(Field = "stkQuantity")]
        public decimal ArticleStockQuantity { get; set; }                   //stkQuantity AS ArticleStockQuantity,

        // ConfigurationUnitMeasure
        [Report(Field = "aumAcronym")]
        public string ConfigurationUnitMeasureaumAcronym { get; set; }      //aumAcronym AS ConfigurationUnitMeasureaumAcronym

        [Report(Field = "aumDesignation")]
        public string ConfigurationUnitMeasureDesignation { get; set; }     //aumDesignation AS ConfigurationUnitMeasureDesignation,
    }
}
