using System;

/* Report Fields
SELECT 
	Date, artCode, artDesignation, stkQuantity, aumAcronym
FROM 
	view_articlestock 
ORDER BY 
	stkDate
;
*/

namespace logicpos.financial.library.Classes.Reports.BOs.Articles
{
    [FRBO(Entity = "view_articlestock")]
    public class FRBOArticleStockView : FRBOBaseObject
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
        [FRBO(Field = "Article")]
        public string Article { get; set; }                                 //artOid AS Article,

        [FRBO(Field = "artOrd")]
        public uint ArticleOrd { get; set; }                                //artOrd AS ArticleOrd,

        [FRBO(Field = "artCode")]
        public string ArticleCode { get; set; }                             //artCode AS ArticleCode,

        [FRBO(Field = "artDesignation")]
        public string ArticleDesignation { get; set; }                      //artDesignation AS ArticleDesignation,

        // ArticleStock
		[FRBO(Field = "artStk")]
        public decimal ArticleStockQuantity { get; set; }                   //artStk AS ArticleStockQuantity,

        [FRBO(Field = "artMinStock")]
        public decimal ArticleStockMinimum { get; set; }                   //artMinStock AS ArticleStockMinimum,

        // ConfigurationUnitMeasure
        [FRBO(Field = "aumAcronym")]
        public string ConfigurationUnitMeasureaumAcronym { get; set; }      //aumAcronym AS ConfigurationUnitMeasureaumAcronym

        [FRBO(Field = "aumDesignation")]
        public string ConfigurationUnitMeasureDesignation { get; set; }     //aumDesignation AS ConfigurationUnitMeasureDesignation,
		
		// Date
		[FRBO(Field = "stmDateDay")]
        public string ArticleStockDateDay { get; set; }                   

        [FRBO(Field = "Date")]
        public DateTime ArticleStockDate { get; set; }                   

    }
}
