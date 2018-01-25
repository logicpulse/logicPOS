using System;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.Xpo.DB;

namespace logicpos.financial.library.Classes.Reports.BOs
{
    public class FRBOArticleFamily : FRBOBaseObject
    {
        public int Code { get; set; }
        public string Designation { get; set; }
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
