using System;
using System.Collections.Generic;

namespace logicpos.financial.library.Classes.Reports.BOs.Articles
{
    [FRBO(Entity = "fin_articlefamily")]
    public class FRBOArticleFamily : FRBOBaseObject
    {
        public UInt32 Ord{ get; set; }
        public UInt32 Code { get; set; }
        public string Designation { get; set; }
        public string ButtonLabel{ get; set; }
        public bool ButtonLabelHide{ get; set; }
        public string ButtonImage{ get; set; }
        public string ButtonIcon{ get; set; }
        // Related Objects
        public List<FRBOArticleSubFamily> ArticleSubFamily { get; set; }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Static

        //OLD TEST Report Code Before FRBOGenericCollection
        //public static List<FRBOArticleFamily> GetList(string pFilter = "", string pOrder = "")
        //{
        //  string filter = (pFilter != String.Empty) ? string.Format("WHERE ({0})", pFilter) : String.Empty;
        //  string order = (pOrder != String.Empty) ? string.Format("ORDER BY {0}", pOrder) : "ORDER BY Ord";

        //  //Disabled
        //  string sql = string.Format(@"
        //    SELECT 
        //      Oid,Code,Designation
        //    FROM 
        //      fin_articlefamily 
        //    {0}
        //    {1}
        //    ;"
        //    , filter
        //    , order
        //  );

        //  XPSelectData xPSelectData = Utils.GetSelectedDataFromQuery(sql);
        //  List<FRBOArticleFamily> businessObjectList = new List<FRBOArticleFamily>();
        //  FRBOArticleFamily businessObject = new FRBOArticleFamily();
        //  PropertyInfo propertyInfo;
        //  string fieldName = String.Empty;
        //  string fieldType = String.Empty;
        //  string fieldTypeDB = String.Empty;
        //  System.Object fieldValue;
        //  int fieldIndex;

        //  foreach (SelectStatementResultRow rowData in xPSelectData.Data)
        //  {
        //    businessObject = new FRBOArticleFamily();
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
