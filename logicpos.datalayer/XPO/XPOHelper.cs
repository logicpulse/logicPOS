using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using logicpos.datalayer.DataLayer.Xpo;
using LogicPOS.Settings;
using LogicPOS.Settings.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using static LogicPOS.Utility.DataConversionUtils;

namespace logicpos.datalayer.Xpo
{
    public static class XPOHelper
    {
        public static uint GetNextTableFieldID(string pTable, string pField, bool pEndsWithZero = true)
        {
            uint result = 0;

            string sql = string.Format("SELECT MAX({0}) FROM {1};", pField, pTable);

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
                        fieldValue = FormatDataTableFieldFromType(
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
                return (XPGuidObject)GetXPGuidObject(pSession, pType, guid);
            }
            else
            {
                return null;
            }
        }

        public static XPGuidObject GetXPGuidObject(Type pXPGuidObjectType, Guid pOid)
        {
            return GetXPGuidObject(XPOSettings.Session, pXPGuidObjectType, pOid);
        }

        public static XPGuidObject GetXPGuidObject(Session pSession, Type pXPGuidObjectType, Guid pOid)
        {
            XPClassInfo classInfo = pSession.GetClassInfo(pXPGuidObjectType);
            dynamic resultObject = pSession.GetObjectByKey(classInfo, pOid);
            XPGuidObject result = (XPGuidObject)resultObject;
            return result;
        }

        public static DateTime CurrentDateTimeAtomic()
        {
            string sql = string.Empty;
            var result = new DateTime();

            switch (DatabaseSettings.DatabaseType)
            {
                case DatabaseType.SQLite:
                case DatabaseType.MonoLite:
                    result = DateTime.Now;
                    break;
                case DatabaseType.MSSqlServer:
                    sql = "SELECT getdate() AS Now;";
                    break;
                case DatabaseType.MySql:
                    sql = "SELECT now() AS Now;";
                    break;
                default:
                    break;
            }

            if (sql != string.Empty)
            {
                result = (DateTime)XPOSettings.Session.ExecuteScalar(sql);
            }

            return result;
        }
        public static string CurrentDateTime(string pDateTimeFormat)
        {
            return CurrentDateTimeAtomic().ToString(pDateTimeFormat, CultureInfo.GetCultureInfo(CultureSettings.CurrentCulture.Name));
        }

        public static DateTime CurrentDateTimeAtomicMidnight()
        {
            return DateTimeToMidnightDate(CurrentDateTimeAtomic());
        }

        public static DateTime DateTimeToMidnightDate(DateTime pDateTime)
        {
            DateTime result = new DateTime(pDateTime.Year, pDateTime.Month, pDateTime.Day);

            return result;
        }

        public static string DateTimeToCombinedDateTimeString(object pValue)
        {
            DateTime tmpData = Convert.ToDateTime(pValue);
            string result = "" + tmpData.ToString("" + CultureSettings.DateTimeFormatCombinedDateTime);
            return (result);
        }

        public static string DateToString(object pValue)
        {
            DateTime tmpData = Convert.ToDateTime(pValue);
            string result = "" + tmpData.ToString("" + CultureSettings.DateTimeFormatDocumentDate);
            return (result);
        }

        public static string DateTimeToString(DateTime pValue)
        {
            return pValue.ToString(CultureSettings.DateTimeFormat);
        }

        public static Dictionary<DateTime, bool> GetHolidays()
        {
            if (_holidays == null)
                _holidays = GetHolidays(DateTime.Now.Year);

            return _holidays;
        }

        private static Dictionary<DateTime, bool> _holidays;

        public static Dictionary<DateTime, bool> GetHolidays(int pYear)
        {
            DateTime currentDateTime;
            Dictionary<DateTime, bool> result = new Dictionary<DateTime, bool>();
            CriteriaOperator criteriaOperator = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled is NULL) AND (Year = 0 OR Year = {0})", pYear));
            SortingCollection sortingCollection = new SortingCollection
            {
                new SortProperty("Ord", SortingDirection.Ascending)
            };
            XPCollection xpcConfigurationHolidays = GetXPCollectionFromCriteria(XPOSettings.Session, typeof(cfg_configurationholidays), criteriaOperator, sortingCollection);

            if (xpcConfigurationHolidays.Count > 0)
            {
                foreach (cfg_configurationholidays item in xpcConfigurationHolidays)
                {
                    currentDateTime = new DateTime(pYear, item.Month, item.Day);
                    result.Add(currentDateTime, item.Fixed);
                }
            }
            return result;
        }

        public static bool IsHoliday(DateTime pDateTime)
        {
            return IsHoliday(GetHolidays(), DateTimeToMidnightDate(pDateTime));
        }

        public static bool IsHoliday(Dictionary<DateTime, bool> pHolidays, DateTime pDateTime)
        {
            bool result = false;

            foreach (var item in pHolidays)
            {
                //Fixed
                if (item.Value)
                {
                    if (item.Key.Month == pDateTime.Month && item.Key.Day == pDateTime.Day)
                        result = true;
                }
                else
                {
                    if (item.Key.Year == pDateTime.Year && item.Key.Month == pDateTime.Month && item.Key.Day == pDateTime.Day)
                        result = true;
                }
            }
            return result;
        }

        public static List<DateTime> GetUtilDays(DateTime pDateStart, bool pWithHoydays)
        {
            return GetUtilDays(DateTimeToMidnightDate(pDateStart), CurrentDateTimeAtomicMidnight(), pWithHoydays);
        }

        public static List<DateTime> GetUtilDays(DateTime pDateStart, DateTime pDateEnd, bool pWithHoydays)
        {
            List<DateTime> result = new List<DateTime>();
            //Range Interval
            DateTime startDateTime = pDateStart.Date.AddDays(1);

            while (startDateTime < pDateEnd.Date)
            {
                if (startDateTime.DayOfWeek != DayOfWeek.Saturday && startDateTime.DayOfWeek != DayOfWeek.Sunday)
                {
                    string isHoliday = (IsHoliday(startDateTime)) ? "Holiday" : string.Empty;

                    if ((pWithHoydays && !IsHoliday(startDateTime)) || !pWithHoydays)
                    {
                        result.Add(startDateTime);
                    }
                }
                //Advance onde Day
                startDateTime = startDateTime.AddDays(1);
            }
            return result;
        }

        public static DateTime GetDateTimeBackUtilDays(DateTime pDateTime, int pDays, bool pWithHoydays)
        {
            DateTime result = DateTimeToMidnightDate(pDateTime);
            string isHoliday = string.Empty;
            int i = 0;
            while (i < pDays)
            {
                //Start Back one Day
                result = result.AddDays(-1);

                if (result.DayOfWeek != DayOfWeek.Saturday && result.DayOfWeek != DayOfWeek.Sunday)
                {
                    isHoliday = (IsHoliday(result)) ? "<Holiday>" : string.Empty;

                    if ((pWithHoydays && !IsHoliday(result)) || !pWithHoydays)
                    {
                        i++;
                    }
                }
            }
            return result;
        }

        public static string GuidToStringId(string pGuidString)
        {
            return pGuidString.Substring(0, 30);
        }
    }
}
