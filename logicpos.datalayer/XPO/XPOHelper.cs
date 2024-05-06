using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using logicpos.datalayer.App;
using logicpos.datalayer.DataLayer.Xpo;
using System;
using System.Data;
using static LogicPOS.Utility.DataConversionUtils;

namespace logicpos.datalayer.Xpo
{
    public static class XPOHelper
    {
        public static uint GetNextTableFieldID(string table, string field)
        {
            uint result = 0;

            string sql = string.Format("SELECT MAX({0}) FROM {1};", field, table);

            var resultInt = XPOSettings.Session.ExecuteScalar(sql);
            if (resultInt != null)
            {
                result = Convert.ToUInt32(resultInt) + 1;

                while (!("" + result).EndsWith("0"))
                {
                    result++;
                }
            }

            if (result <= 0)
            {
                result = 10;
            }

            return result;
        }

        public static XPGuidObject GetXPGuidObjectFromCriteria(Type pXPGuidObjectType, string pCriteriaFilter)
        {
            return GetXPGuidObjectFromCriteria(XPOSettings.Session, pXPGuidObjectType, pCriteriaFilter);
        }

        public static XPGuidObject GetXPGuidObjectFromCriteria(Session pSession, Type pXPGuidObjectType, string pCriteriaFilter)
        {
            CriteriaOperator criteria = CriteriaOperator.Parse(pCriteriaFilter);
            XPGuidObject result = (XPOSettings.Session.FindObject(pXPGuidObjectType, criteria) as XPGuidObject);
            return result;
        }

        public static XPCollection GetXPCollectionFromCriteria(Session pSession, Type pXPGuidObjectType, CriteriaOperator pCriteriaOperator, SortingCollection pSortingCollection)
        {
            XPCollection result = new XPCollection(pSession, pXPGuidObjectType, pCriteriaOperator);
            if (pSortingCollection != null) result.Sorting = pSortingCollection;
            return result;
        }

        public static XPSelectData GetSelectedDataFromQuery(string pSql)
        {
            return GetSelectedDataFromQuery(XPOSettings.Session, pSql);
        }

        public static XPSelectData GetSelectedDataFromQuery(Session pSession, string pSql)
        {
            SelectedData xpoSelectedData = pSession.ExecuteQueryWithMetadata(pSql);
            XPSelectData xPSelectData = new XPSelectData(xpoSelectedData);
            return xPSelectData;
        }

        public static DataTable GetDataTableFromQuery(string pSql)
        {
            return GetDataTableFromQuery(XPOSettings.Session, pSql);
        }

        public static DataTable GetDataTableFromQuery(Session pSession, string pSql)
        {
            //Get SelectedData
            XPSelectData xPSelectData = GetSelectedDataFromQuery(pSession, pSql);
            //Init DataTable
            DataTable resultDataTable = new DataTable();

            //Add Columns
            string fieldName;
            string fieldType;
            string fieldValue;

            foreach (SelectStatementResultRow row in xPSelectData.Meta)
            {
                fieldName = row.Values[0].ToString();
                fieldType = row.Values[2].ToString();
                resultDataTable.Columns.Add(fieldName, typeof(string));
            }

            //Add Rows
            foreach (SelectStatementResultRow rowData in xPSelectData.Data)
            {
                //Init a new DataRow
                string[] dataRow = new string[xPSelectData.Meta.Length];

                foreach (SelectStatementResultRow rowMeta in xPSelectData.Meta)
                {
                    fieldName = rowMeta.Values[0].ToString();
                    fieldType = rowMeta.Values[2].ToString();
                    //Check if is Not Null
                    if (rowData.Values[xPSelectData.GetFieldIndex(fieldName)] != null)
                    {
                        fieldValue =  FormatDataTableFieldFromType(
                            rowData.Values[xPSelectData.GetFieldIndex(fieldName)].ToString(), 
                            fieldType);
                    }
                    else
                    {
                        fieldValue = string.Empty;
                    }
                    dataRow[xPSelectData.GetFieldIndex(fieldName)] = fieldValue;
                }
                //resultDataTable.Rows.Add(rowData.Values[xPSelectData.GetFieldIndex(fieldName)].ToString());
                resultDataTable.Rows.Add(dataRow);
            }

            return resultDataTable;
        }

        public static SortingCollection GetXPCollectionDefaultSortingCollection()
        {
            SortingCollection sortingCollection = new SortingCollection
            {
                new SortProperty("Ord", SortingDirection.Ascending)
            };
            //sortingCollection.Add(new SortProperty("Designation", DevExpress.Xpo.DB.SortingDirection.Ascending));
            return sortingCollection;
        }

        public static int GetNextTableFieldInt(string pTable, string pField, string pFilter)
        {
            int result = -1;
            string filter = (pFilter != string.Empty) ? string.Format(" WHERE {0}", pFilter) : string.Empty;

            string sql = string.Format("SELECT MAX({0}) FROM {1}{2};", pField, pTable, filter);
            var resultInt = XPOSettings.Session.ExecuteScalar(sql);
            if (resultInt != null)
            {

                if (resultInt.GetType() == typeof(short))
                {
                    result = (short)((short)resultInt + 1);
                }
                else if (resultInt.GetType() == typeof(int))
                {
                    result = (int)((int)resultInt + 1);
                }
                return result;
            }


            if (result == -1) result = 1;
            return result;
        }
        public static Guid GetGuidFromQuery(string pSql)
        {
            return GetGuidFromQuery(XPOSettings.Session, pSql);
        }

        public static Guid GetGuidFromQuery(Session pSession, string pSql)
        {
            try
            {
                var resultField = pSession.ExecuteScalar(pSql);

                if (resultField != null)
                {
                    if (resultField.GetType() == typeof(string))
                    {
                        return new Guid((string)resultField);
                    }
                    else if (resultField.GetType() == typeof(Guid))
                    {
                        return (Guid)resultField;
                    }
                }

                return Guid.Empty;
            }
            catch
            {
                return Guid.Empty;
            }
        }

        public static XPGuidObject GetXPGuidObjectFromField(Type pType, string pSearchField, string pSearchValue)
        {
            return GetXPGuidObjectFromField(XPOSettings.Session, pType, pSearchField, pSearchValue);
        }

        public static XPGuidObject GetXPGuidObjectFromField(Session pSession, Type pType, string pSearchField, string pSearchValue)
        {
            string executeSql = string.Format(@"SELECT Oid FROM {0} WHERE (Disabled IS NULL OR Disabled  <> 1) AND {1} = '{2}';", pType.Name, pSearchField, pSearchValue);
            Guid guid = GetGuidFromQuery(pSession, executeSql);
            if (guid != Guid.Empty)
            {
                return (XPGuidObject)DataLayerUtils.GetXPGuidObject(pSession, pType, guid);
            }
            else
            {
                return null;
            }
        }
    }
}
