using System;

namespace logicpos.financial.library.Classes.Reports.BOs.Articles
{
    [FRBO(Entity = "fin_article")]
    public class FRBOArticle : FRBOBaseObject
    {
        public UInt32 Ord { get; set; }
        public string Code { get; set; }
        public string CodeDealer { get; set; }
        public string Designation { get; set; }
        public string ButtonLabel { get; set; }
        public bool ButtonLabelHide { get; set; }
        public string ButtonImage { get; set; }
        public string ButtonIcon { get; set; }
        public decimal Price1 { get; set; }
        public decimal Price2 { get; set; }
        public decimal Price3 { get; set; }
        public decimal Price4 { get; set; }
        public decimal Price5 { get; set; }
        public decimal Price1Promotion { get; set; }
        public decimal Price2Promotion { get; set; }
        public decimal Price3Promotion { get; set; }
        public decimal Price4Promotion { get; set; }
        public decimal Price5Promotion { get; set; }
        public bool Price1UsePromotionPrice { get; set; }
        public bool Price2UsePromotionPrice { get; set; }
        public bool Price3UsePromotionPrice { get; set; }
        public bool Price4UsePromotionPrice { get; set; }
        public bool Price5UsePromotionPrice { get; set; }
        public bool PriceWithVat { get; set; }
        public decimal Discount { get; set; }
        public decimal DefaultQuantity { get; set; }
        public decimal Accounting { get; set; }
        public decimal Tare { get; set; }
        public decimal Weight { get; set; }
        public string BarCode { get; set; }
        public bool PVPVariable { get; set; }
        public bool Favorite { get; set; }
        public string Token1 { get; set; }
        public string Token2 { get; set; }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Static

        //OLD TEST Report Code Before FRBOGenericCollection
        //public static List<FRBOArticle> GetList(string pFilter = "", string pOrder = "")
        //{
        //  string filter = (pFilter != String.Empty) ? string.Format("WHERE ({0})", pFilter) : String.Empty;
        //  string order = (pOrder != String.Empty) ? string.Format("ORDER BY {0}", pOrder) : "ORDER BY Ord";

        //  //Disabled
        //  string sql = string.Format(@"
        //    SELECT 
        //      Oid,Code,Designation,Price1,Discount,BarCode
        //    FROM 
        //      fin_article
        //    {0}
        //    {1}
        //    ;"
        //    , filter
        //    , order
        //  );

        //  XPSelectData xPSelectData = Utils.GetSelectedDataFromQuery(sql);
        //  List<FRBOArticle> businessObjectList = new List<FRBOArticle>();
        //  FRBOArticle businessObject = new FRBOArticle();
        //  PropertyInfo propertyInfo;
        //  string fieldName = String.Empty;
        //  string fieldType = String.Empty;
        //  string fieldTypeDB = String.Empty;
        //  System.Object fieldValue;
        //  int fieldIndex;

        //  foreach (SelectStatementResultRow rowData in xPSelectData.Data)
        //  {
        //    businessObject = new FRBOArticle();
        //    foreach (SelectStatementResultRow rowMeta in xPSelectData.Meta)
        //    {
        //      fieldName = rowMeta.Values[0].ToString();
        //      fieldTypeDB = rowMeta.Values[1].ToString();;
        //      fieldType = rowMeta.Values[2].ToString();;
        //      fieldIndex = xPSelectData.GetFieldIndex(fieldName);
        //      fieldValue = rowData.Values[fieldIndex];

        //      //If Property Exist in businessObject Assign it
        //      propertyInfo = businessObject.GetType().GetProperty(fieldName);
        //      if (propertyInfo != null) propertyInfo.SetValue(businessObject, fieldValue);
        //    }
        //    businessObjectList.Add(businessObject);
        //  }

        //  return businessObjectList;
        //}
    }
}
