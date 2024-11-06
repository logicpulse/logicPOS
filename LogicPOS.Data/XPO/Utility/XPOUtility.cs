using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using LogicPOS.Api.Entities;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.DTOs.Common;
using LogicPOS.DTOs.Printing;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.Settings.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Security;
using System.Text;
using static LogicPOS.Utility.DataConversionUtils;

namespace LogicPOS.Data.XPO.Utility
{
    public static partial class XPOUtility
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

        public static Entity GetXPGuidObjectFromCriteria(Type pXPGuidObjectType, string pCriteriaFilter)
        {
            return GetXPGuidObjectFromCriteria(XPOSettings.Session, pXPGuidObjectType, pCriteriaFilter);
        }

        public static Entity GetXPGuidObjectFromCriteria(Session pSession, Type pXPGuidObjectType, string pCriteriaFilter)
        {
            CriteriaOperator criteria = CriteriaOperator.Parse(pCriteriaFilter);
            Entity result = XPOSettings.Session.FindObject(pXPGuidObjectType, criteria) as Entity;
            return result;
        }

        public static XPCollection GetXPCollectionFromCriteria(Session pSession, Type pXPGuidObjectType, CriteriaOperator pCriteriaOperator, SortingCollection pSortingCollection)
        {
            XPCollection result = new XPCollection(pSession, pXPGuidObjectType, pCriteriaOperator);
            if (pSortingCollection != null) result.Sorting = pSortingCollection;
            return result;
        }

        public static SQLSelectResultData GetSelectedDataFromQuery(string pSql)
        {
            return GetSelectedDataFromQuery(XPOSettings.Session, pSql);
        }

        public static SQLSelectResultData GetSelectedDataFromQuery(Session pSession, string pSql)
        {
            SelectedData xpoSelectedData = pSession.ExecuteQueryWithMetadata(pSql);
            SQLSelectResultData xPSelectData = new SQLSelectResultData(xpoSelectedData);
            return xPSelectData;
        }

        public static DataTable GetDataTableFromQuery(string pSql)
        {
            return GetDataTableFromQuery(XPOSettings.Session, pSql);
        }

        public static DataTable GetDataTableFromQuery(Session pSession, string pSql)
        {
            //Get SelectedData
            SQLSelectResultData xPSelectData = GetSelectedDataFromQuery(pSession, pSql);
            //Init DataTable
            DataTable resultDataTable = new DataTable();

            //Add Columns
            string fieldName;
            string fieldType;
            string fieldValue;

            foreach (SelectStatementResultRow row in xPSelectData.MetaDataRows)
            {
                fieldName = row.Values[0].ToString();
                fieldType = row.Values[2].ToString();
                resultDataTable.Columns.Add(fieldName, typeof(string));
            }

            //Add Rows
            foreach (SelectStatementResultRow rowData in xPSelectData.DataRows)
            {
                //Init a new DataRow
                string[] dataRow = new string[xPSelectData.MetaDataRows.Length];

                foreach (SelectStatementResultRow rowMeta in xPSelectData.MetaDataRows)
                {
                    fieldName = rowMeta.Values[0].ToString();
                    fieldType = rowMeta.Values[2].ToString();
                    //Check if is Not Null
                    if (rowData.Values[xPSelectData.GetFieldIndexFromName(fieldName)] != null)
                    {
                        fieldValue = FormatDataTableFieldFromType(
                            rowData.Values[xPSelectData.GetFieldIndexFromName(fieldName)].ToString(),
                            fieldType);
                    }
                    else
                    {
                        fieldValue = string.Empty;
                    }
                    dataRow[xPSelectData.GetFieldIndexFromName(fieldName)] = fieldValue;
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
            string filter = pFilter != string.Empty ? string.Format(" WHERE {0}", pFilter) : string.Empty;

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
                    result = (int)resultInt + 1;
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

        public static Entity GetXPGuidObjectFromField(Type pType, string pSearchField, string pSearchValue)
        {
            return GetXPGuidObjectFromField(XPOSettings.Session, pType, pSearchField, pSearchValue);
        }

        public static Entity GetXPGuidObjectFromField(Session pSession, Type pType, string pSearchField, string pSearchValue)
        {
            string executeSql = string.Format(@"SELECT Oid FROM {0} WHERE (Disabled IS NULL OR Disabled  <> 1) AND {1} = '{2}';", pType.Name, pSearchField, pSearchValue);
            Guid guid = GetGuidFromQuery(pSession, executeSql);
            if (guid != Guid.Empty)
            {
                return GetXPGuidObject(pSession, pType, guid);
            }
            else
            {
                return null;
            }
        }

        public static Entity GetXPGuidObject(Type pXPGuidObjectType, Guid pOid)
        {
            return GetXPGuidObject(XPOSettings.Session, pXPGuidObjectType, pOid);
        }

        public static TEntity GetEntityById<TEntity>(
            Guid id,
            Session session = null) where TEntity : class
        {
            if (session == null)
            {
                session = XPOSettings.Session;
            }

            XPClassInfo entityClassInfo = session.GetClassInfo(typeof(TEntity));
            return session.GetObjectByKey(entityClassInfo, id) as TEntity;
        }

        public static Entity GetXPGuidObject(Session pSession, Type pXPGuidObjectType, Guid pOid)
        {
            XPClassInfo classInfo = pSession.GetClassInfo(pXPGuidObjectType);
            dynamic resultObject = pSession.GetObjectByKey(classInfo, pOid);
            Entity result = (Entity)resultObject;
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
            return result;
        }

        public static string DateToString(object pValue)
        {
            DateTime tmpData = Convert.ToDateTime(pValue);
            string result = "" + tmpData.ToString("" + CultureSettings.DateTimeFormatDocumentDate);
            return result;
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
                    string isHoliday = IsHoliday(startDateTime) ? "Holiday" : string.Empty;

                    if (pWithHoydays && !IsHoliday(startDateTime) || !pWithHoydays)
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
                    isHoliday = IsHoliday(result) ? "<Holiday>" : string.Empty;

                    if (pWithHoydays && !IsHoliday(result) || !pWithHoydays)
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

        public static Dictionary<string, string> GetPreferencesParameters()
        {
            return GetPreferencesParameters(XPOSettings.Session);
        }

        public static Dictionary<string, string> GetPreferencesParameters(Session pSession)
        {
            Dictionary<string, string> resultPreferences = new Dictionary<string, string>();

            SortingCollection sortingCollection = new SortingCollection
                {
                    new SortProperty("Ord", SortingDirection.Ascending)
                };
            CriteriaOperator criteriaOperator = CriteriaOperator.Parse("Disabled = 0 OR Disabled is NULL");
            XPCollection xpcPreferenceParameter = new XPCollection(pSession, typeof(cfg_configurationpreferenceparameter), criteriaOperator);

            foreach (cfg_configurationpreferenceparameter item in xpcPreferenceParameter)
            {
                resultPreferences.Add(item.Token, item.Value);
            }

            return resultPreferences;
        }

        public static Dictionary<string, bool> GetUserPermissions(sys_userdetail pUser)
        {

            Dictionary<string, bool> resultPermissions = new Dictionary<string, bool>
            {
                { "BACKOFFICE_MAN_SYSTEM_MENU_MENU", true },
                { "BACKOFFICE_MAN_SYSTEM_POS_MENU", true },
                { "BACKOFFICE_MAN_SYSTEM_QUIT_MENU", true },
                { "BACKOFFICE_MAN_SYSTEM_NOTIFICATION_MENU", true },
                { "BACKOFFICE_MAN_SYSTEM_CHANGELOG_MENU", true }
            };

            if (pUser != null)
            {
                if (pUser.Profile != null)
                {
                    foreach (sys_userpermissionprofile item in pUser.Profile.Permissions)
                    {

                        resultPermissions.Add(item.PermissionItem.Token, item.Granted);
                    }
                }
            }

            return resultPermissions;
        }

        public static Dictionary<string, bool> GetUserPermissions()
        {
            return GetUserPermissions(XPOSettings.LoggedUser);
        }

        public static void SystemNotification()
        {
            SystemNotification(XPOSettings.Session);
        }

        public static void SystemNotification(Session xpoSession)
        {
            string cultureFinancialRules = AppSettings.Instance.cultureFinancialRules;

            uint ord = 1;
            sys_systemnotificationtype systemNotificationType;
            sys_systemnotification systemNotification;
            CriteriaOperator criteriaOperator;
            XPCollection xpcSystemNotification;

            systemNotificationType = (sys_systemnotificationtype)xpoSession.GetObjectByKey(typeof(sys_systemnotificationtype), NotificationSettings.XpoOidSystemNotificationTypeNewTerminalRegistered);
            if (systemNotificationType != null)
            {
                criteriaOperator = CriteriaOperator.Parse(string.Format("NotificationType = '{0}' AND TerminalLastRead = '{1}'", NotificationSettings.XpoOidSystemNotificationTypeNewTerminalRegistered, TerminalSettings.LoggedTerminal.Oid));
                xpcSystemNotification = new XPCollection(xpoSession, typeof(sys_systemnotification), criteriaOperator);
                //Create Notification
                if (xpcSystemNotification.Count == 0)
                {
                    systemNotification = new sys_systemnotification(xpoSession);
                    systemNotification.Ord = ord;
                    systemNotification.NotificationType = systemNotificationType;
                    systemNotification.Message = string.Format(systemNotificationType.Message, TerminalSettings.LoggedTerminal.Designation);
                    systemNotification.Save();
                    ord++;
                }
            };

           
            switch (cultureFinancialRules)
            {
                case "pt-PT":
                    int defaultBackDaysForInvoice = NotificationSettings.XpoOidSystemNotificationDaysBackWhenFiltering;

                  
                    systemNotification = ProcessFinanceDocumentToInvoice(xpoSession, NotificationSettings.XpoOidSystemNotificationTypeCurrentAccountDocumentsToInvoice, DocumentSettings.CurrentAccountInputId, "(Payed = 0 OR Payed IS NULL)", defaultBackDaysForInvoice);
                    if (systemNotification != null)
                    {
                        systemNotification.Ord = ord; systemNotification.Save();
                        ord++;

                    };

             
                    systemNotification = ProcessFinanceDocumentToInvoice(xpoSession, NotificationSettings.XpoOidSystemNotificationTypeConsignationInvoiceDocumentsToInvoice, DocumentSettings.ConsignationInvoiceId, "(DocumentChild IS NULL)", defaultBackDaysForInvoice);
                    if (systemNotification != null)
                    {
                        systemNotification.Ord = ord; systemNotification.Save();
                        ord++;

                    };

                    systemNotification = ProcessFinanceDocumentToInvoice(xpoSession, NotificationSettings.XpoOidSystemNotificationTypeSaftDocumentTypeMovementOfGoods, SaftDocumentType.MovementOfGoods, "(DocumentChild IS NULL AND DocumentStatusStatus = 'N')", defaultBackDaysForInvoice);
                    if (systemNotification != null)
                    {
                        systemNotification.Ord = ord; systemNotification.Save(); ord++;
                    };

                    break;

                default:
                    break;
            }

        }

        public static sys_systemnotification ProcessFinanceDocumentToInvoice(Session pSession, Guid pSystemNotificationTypeGuid, Guid pDocumentType, string pExtraFilter, int pDaysBack)
        {
            string filter = string.Format("(DocumentType = '{0}' AND DocumentStatusStatus = 'N')", pDocumentType.ToString());
            return ProcessFinanceDocumentToInvoice(pSession, pSystemNotificationTypeGuid, filter, pExtraFilter, pDaysBack);
        }

        public static sys_systemnotification ProcessFinanceDocumentToInvoice(Session pSession, Guid pSystemNotificationTypeGuid, SaftDocumentType pSaftDocumentType, string pExtraFilter, int pDaysBack)
        {
            string filter = string.Empty;

            CriteriaOperator criteriaOperator = CriteriaOperator.Parse(string.Format("(SaftDocumentType = {0})", Convert.ToInt16(pSaftDocumentType)));
            XPCollection xpcDocumentFinanceType = new XPCollection(pSession, typeof(fin_documentfinancetype), criteriaOperator);
            if (xpcDocumentFinanceType.Count > 0)
            {
                int i = 0;
                foreach (fin_documentfinancetype item in xpcDocumentFinanceType)
                {
                    i++;
                    filter += string.Format("DocumentType = '{0}'{1}", item.Oid, i < xpcDocumentFinanceType.Count ? " OR " : string.Empty);
                }
                filter = string.Format("({0})", filter);
            }

            return ProcessFinanceDocumentToInvoice(pSession, pSystemNotificationTypeGuid, filter, pExtraFilter, pDaysBack);
        }

        public static sys_systemnotification ProcessFinanceDocumentToInvoice(Session pSession, Guid pSystemNotificationTypeGuid, string pFilter, string pExtraFilter, int pDaysBackToFilter)
        {
            //Init Local Vars
            sys_systemnotificationtype systemNotificationType = (sys_systemnotificationtype)pSession.GetObjectByKey(typeof(sys_systemnotificationtype), pSystemNotificationTypeGuid);
            sys_systemnotification result = null;
            //Used to Persist sys_systemnotification if greater than 0
            int totalNotificatedDocuments = 0;
            //Ignore Notificate Documents after Documents Have Been Notified a determined Number Of Times
            int ignoreNotificationsAfterHaveBeenNotificatedNumberOfTimes = 0;

            ignoreNotificationsAfterHaveBeenNotificatedNumberOfTimes = Convert.ToInt16(GeneralSettings.PreferenceParameters["NOTIFICATION_DOCUMENTS_TO_INVOICE_IGNORE_AFTER_SHOW_NUMBER_OF_TIMES"]);


            //Get Date Back DaysBackToFilter (Without WeekEnds and Holidays)
            int warnDaysBefore = systemNotificationType.WarnDaysBefore > 0 ? systemNotificationType.WarnDaysBefore : 0;
            int daysBackToFilter = pDaysBackToFilter - warnDaysBefore;
            DateTime dateFilterFrom = GetDateTimeBackUtilDays(CurrentDateTimeAtomicMidnight(), daysBackToFilter, true);

            //Extra Filter 
            string filter = pFilter;
            if (pExtraFilter != string.Empty) filter = string.Format("{0} AND {1}", filter, pExtraFilter);
            filter = string.Format("{0} AND (Date <= '{1} 23:59:59')", filter, dateFilterFrom.ToString(CultureSettings.DateFormat));

            CriteriaOperator criteriaOperator = CriteriaOperator.Parse(filter);
            SortProperty sortProperty = new SortProperty("CreatedAt", SortingDirection.Ascending);
            XPCollection xpcDocumentFinanceMaster = new XPCollection(pSession, typeof(fin_documentfinancemaster), criteriaOperator, sortProperty);

            if (xpcDocumentFinanceMaster.Count > 0)
            {
                int i = 0;
                string documentsMessage = string.Empty;
                int documentBackUtilDays = 0;

                //Generate DocumentNumber List documentsMessage
                foreach (fin_documentfinancemaster item in xpcDocumentFinanceMaster)
                {
                    i++;
                    //Get BackDays
                    documentBackUtilDays = GetUtilDays(item.Date, true).Count;
                    //Show total Showed Notification in Document
                    // Get total notification for Current Document
                    int totalNotificationsInDocument = item.Notifications.Count;

                    // If document has less notifications show it again, or if is ignored with ignoreNotificationsAfterBeenNotificatedTimes == 0
                    if (ignoreNotificationsAfterHaveBeenNotificatedNumberOfTimes == 0 || totalNotificationsInDocument < ignoreNotificationsAfterHaveBeenNotificatedNumberOfTimes)
                    {
                        // Increment notifications counter
                        totalNotificatedDocuments++;
                        // Add To Message
                        documentsMessage += string.Format(
                            "- {0} : {1} : {2} {3} : (#{4})",
                            item.DocumentNumber, item.Date,
                            documentBackUtilDays,
                            CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName,
                            "global_day_days"),
                            item.Notifications.Count + 1);

                        //Add New Line if not Last Document
                        if (i < xpcDocumentFinanceMaster.Count) documentsMessage += Environment.NewLine;
                    }
                }

                // Create Notification Object if has notifications
                if (totalNotificatedDocuments > 0)
                {
                    result = new sys_systemnotification(pSession)
                    {
                        NotificationType = systemNotificationType,
                        Message = string.Format(systemNotificationType.Message, totalNotificatedDocuments, pDaysBackToFilter, documentsMessage)
                    };
                    result.Save();

                    //Persist sys_systemnotificationdocumentmaster manyToMany Relantionship
                    foreach (fin_documentfinancemaster item in xpcDocumentFinanceMaster)
                    {
                        sys_systemnotificationdocumentmaster notificationDocumentMasterReference = new sys_systemnotificationdocumentmaster(pSession)
                        {
                            DocumentMaster = item,
                            Notification = result
                        };
                        notificationDocumentMasterReference.Save();
                    }
                }
            }

            return result;
        }

        public static bool Audit(string pAuditTypeToken, string pDescription = "")
        {
            return Audit(
                XPOSettings.Session,
                XPOSettings.LoggedUser ?? null,
                TerminalSettings.LoggedTerminal ?? null,
                pAuditTypeToken,
                pDescription
            );
        }

        public static bool Audit(Session pSession, sys_userdetail pLoggedUser, pos_configurationplaceterminal pLoggedTerminal, string pAuditTypeToken, string pDescription = "")
        {
            bool result = false;
            DateTime dateTime = CurrentDateTimeAtomic();
            string executeSql = string.Format(@"SELECT Oid FROM sys_systemaudittype WHERE (Disabled IS NULL or Disabled  <> 1) AND Token = '{0}';", pAuditTypeToken);

            
            //Get auditType Guid from Query
            Guid guidAuditType = GetGuidFromQuery(executeSql);

            if (!guidAuditType.Equals(Guid.Empty))
            {
                //Fresh User and Terminal, to prevent Object Delection Problem
                sys_userdetail xpoUserDetail = pLoggedUser != null ? (sys_userdetail)GetXPGuidObject(typeof(sys_userdetail), pLoggedUser.Oid) : null;
                pos_configurationplaceterminal xpoTerminal = (pos_configurationplaceterminal)GetXPGuidObject(typeof(pos_configurationplaceterminal), pLoggedTerminal.Oid);
                //get AuditType Object
                sys_systemaudittype xpoAuditType = (sys_systemaudittype)GetXPGuidObject(typeof(sys_systemaudittype), guidAuditType);
                string description = pDescription != string.Empty ? pDescription
                  : xpoAuditType.ResourceString != null && CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, xpoAuditType.ResourceString) != null
                  ? CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, xpoAuditType.ResourceString) : xpoAuditType.Designation;

                sys_systemaudit systemAudit = new sys_systemaudit(pSession)
                {
                    Date = dateTime,
                    Description = description,
                    UserDetail = xpoUserDetail,
                    Terminal = xpoTerminal,
                    AuditType = xpoAuditType
                };
                systemAudit.Save();



                result = true;
            }
            else
            {
                string exceptionMessage = string.Format("Invalid AuditType: [{0}]", pAuditTypeToken);
                throw new Exception(exceptionMessage);
            }

            return result;
        }

        public static decimal GetPartialPaymentPayedItems(Session pSession, Guid pDocumentOrderMain, Guid pArticle)
        {
            decimal result = 0.0m;

            string sql = string.Format(@"
                SELECT 
                  SUM(fdQuantity) AS Quantity 
                FROM 
                  view_documentfinance 
                WHERE 
                  fmSourceOrderMain = '{0}' AND 
                  fdArticle = '{1}'
                ;"
              , pDocumentOrderMain
              , pArticle
            );
            //_logger.Debug(string.Format("sql: [{0}]", sql));
            var partialPayedItems = pSession.ExecuteScalar(sql);

            return partialPayedItems != null && Convert.ToDecimal(partialPayedItems) > 0
              ? Convert.ToDecimal(partialPayedItems)
              : result;
        }

        public static erp_customer GetFinalConsumerEntity()
        {
            erp_customer result = null;

            string filterCriteria = string.Format("Oid = '{0}'", InvoiceSettings.FinalConsumerId.ToString());
            result = GetXPGuidObjectFromCriteria(typeof(erp_customer), filterCriteria) as erp_customer;

            return result;
        }

        public static bool IsFinalConsumerEntity(string pFiscalNumber)
        {
            bool result = false;

            var entity = GetFinalConsumerEntity();
            result = entity != null && pFiscalNumber == entity.FiscalNumber;

            return result;
        }

        public static string GetDocumentsQuery(bool pWayBillMode)
        {
            return GetDocumentsQuery(pWayBillMode, Guid.Empty);
        }

        public static string GetDocumentsQuery(bool pWayBillMode, Guid pDocumentMaster)
        {
            //Common : Require to Check if has Records with ReturnCode -3 (Já foi registado um documento idêntico.)
            string where = @"(
                ft.WsAtDocument = 1 AND (fm.ATValidAuditResult IS NULL OR fm.ATResendDocument = 1)  
                AND (SELECT COUNT(*) FROM sys_systemauditat WHERE DocumentMaster = fm.Oid AND ReturnCode = '-3') = 0
            ) AND";

            //Invoices
            //
            //1.4 – Tipo (InvoiceType)
            //Tipo de documento. Pode assumir os seguintes valores:
            //    FT – Fatura;
            //    FS – Fatura Simplificada;
            //    NC –.Nota de Crédito;
            //    ND – Nota de Débito;
            //
            //1.4 – Estado (InvoiceStatus)	
            //Estado de documento. Pode assumir os seguintes valores:
            //    N – Normal;
            //    A – Anulada;
            if (!pWayBillMode)
            {
                //Includes FR SAF-T v1.03
                where += @" (
                    ((ft.Acronym = 'FT' AND ft.WayBill <> 1) OR ft.Acronym = 'FS' OR ft.Acronym = 'NC' OR ft.Acronym = 'ND' OR ft.Acronym = 'FR') 
                    AND (fm.DocumentStatusStatus = 'N' OR fm.DocumentStatusStatus = 'A') 
                    AND ((SELECT COUNT(*) FROM sys_systemauditat WHERE DocumentMaster = fm.Oid AND ReturnCode = '0') = 0 ) 
                )";
                /* IN009083 - Premisses:
                 * - we do not cancel financial documents already received by AT, therefore, do not resend them (COUNT statement)
                 * - "fm.ATResendDocument = 1" is not settled to ft.WayBill <> 1 (non-WayBill documents)
                 */
            }
            //TransportDocuments/WayBill
            //
            //1.8 – Tipo do documento (MovementType)
            //Deve ser preenchido com:
            //    GR – Guia de remessa;
            //    GT – Guia de transporte;
            //    GA – Guia de movimentação de ativos próprios;
            //    GC – Guia de consignação;
            //    GD – Guia ou nota de devolução efetuada pelo cliente.
            //
            //1.6 - Estado atual do documento
            //Estado de documento. Pode assumir os seguintes
            //(MovementStatus)
            //valores:
            //    N – Normal;
            //    T – Por conta de terceiros;
            //    A – Anulada.
            //
            //1.3.4 – Pais (Country)
            //Preencher com <<PT>>.

            //1.12 – Endereço da Empresa Cliente (CustomerAddress)
            //1.12.4 – Pais (Country)
            //Preencher com <<PT>>.

            //1.14 – Local de carga (AddressFrom)
            //1.14.4 – Pais (Country)
            //Preencher com PT.

            //É obrigatório comunicar um documento de transporte à AT cujo destinatário seja um consumidor final?
            //Não. Estão excluídos das obrigações de comunicação os documentos de transporte em que o destinatário ou adquirente seja consumidor final.
            else
            {
                where += string.Format(@" (
                    (ft.Acronym = 'GR' OR ft.Acronym = 'GT' OR ft.Acronym = 'GA' OR ft.Acronym = 'GC' OR ft.Acronym = 'GD')
                    AND (fm.DocumentStatusStatus = 'N' OR fm.DocumentStatusStatus = 'T' OR fm.DocumentStatusStatus = 'A')
                    AND (fm.ShipToCountry = 'PT' AND fm.ShipFromCountry = 'PT')
                    AND (fm.EntityOid <> '{0}')
                )"
                // Skip FinalConsumer
                , InvoiceSettings.FinalConsumerId
                );
            }

            //Shared: If Has Target Document
            if (pDocumentMaster != Guid.Empty)
            {
                where = string.Format("{0} AND fm.Oid='{1}'", where, pDocumentMaster.ToString());
            }

            //Build Query
            string result = string.Format(@"
                SELECT 
                    fm.Oid AS Oid
                FROM
                    (
                        fin_documentfinancemaster fm
                        LEFT JOIN fin_documentfinancetype ft ON (fm.DocumentType = ft.Oid)
                    )
                WHERE
                    {0}
                ORDER BY 
                    fm.DocumentDate DESC;
                ",
                where
            );

            return result;
        }

        /// <summary>
        /// Get Document <Line>s Splitted by Tax and <DocumentTotals> Content
        /// 	Linhas do Documento por Taxa (Line)
        /// 	Resumo das linhas da fatura por taxa de imposto, e motivo de isenção ou não liquidação.
        /// 	Deve existir uma, e uma só linha, por cada taxa (TaxType, TaxCountryRegion, TaxCode) e motivo de isenção ou não liquidação (TaxExemptionReason)
        /// </summary>
        /// <param name="DocumentMaster"></param>
        /// <returns></returns>
        public static string GetDocumentContentLinesAndDocumentTotals(fin_documentfinancemaster pDocumentMaster)
        {
            decimal taxPayable = 0.0m;
            decimal netTotal = 0.0m;
            decimal grossTotal = 0.0m;
            //Prepare Node Name
            string nodeName = pDocumentMaster.DocumentType.Credit ? "CreditAmount" : "DebitAmount";


            //Init Locals Vars
            string result;
            try
            {
                string sql = string.Format(@"
                    SELECT 
	                    fdVat AS Vat,
	                    cvTaxType AS TaxType,
	                    cvTaxCode AS TaxCode,
	                    cvTaxCountryRegion AS TaxCountryRegion,
	                    SUM(fdTotalNet) AS TotalNet,
	                    SUM(fdTotalGross) AS TotalGross,
                        SUM(fdTotalDiscount) AS TotalDiscount,
	                    SUM(fdTotalTax) AS TotalTax,
	                    SUM(fdTotalFinal) AS TotalFinal,
	                    cxAcronym AS VatExemptionReasonAcronym
                    FROM
	                    view_documentfinance
                    WHERE 
	                    fmOid = '{0}'
                    GROUP BY
	                    fdVat,cvTaxType,cvTaxCode,cvTaxCountryRegion,cxAcronym
                    ORDER BY
	                    fdVat, VatExemptionReasonAcronym
                    ;"
                    , pDocumentMaster.Oid
                );

                DataTable dtResult = GetDataTableFromQuery(sql);

                //Init StringBuilder
                StringBuilder sb = new StringBuilder();

                foreach (DataRow item in dtResult.Rows)
                {
                    string taxExemptionReason = !string.IsNullOrEmpty(item["VatExemptionReasonAcronym"].ToString())
                        ? string.Format("{0}      <ns2:TaxExemptionReason>{1}</ns2:TaxExemptionReason>", Environment.NewLine, item["VatExemptionReasonAcronym"])
                        : string.Empty
                    ;

                    sb.Append(string.Format(@"    <Line>
      <ns2:{0}>{1}</ns2:{0}>
      <ns2:Tax>
        <ns2:TaxType>{2}</ns2:TaxType>
        <ns2:TaxCountryRegion>{3}</ns2:TaxCountryRegion>
        <ns2:TaxPercentage>{4}</ns2:TaxPercentage>
      </ns2:Tax>{5}
    </Line>
"
                        , nodeName
                        , DecimalToString(Convert.ToDecimal(item["TotalNet"]))
                        , item["TaxType"]
                        , item["TaxCountryRegion"]
                        , DecimalToString(Convert.ToDecimal(item["Vat"]))
                        , taxExemptionReason
                    ));

                    //Sum DocumentTotals
                    taxPayable += Convert.ToDecimal(item["TotalTax"]);
                    netTotal += Convert.ToDecimal(item["TotalNet"]);
                    //Is TotalFinal not db TotalGross
                    grossTotal += Convert.ToDecimal(item["TotalFinal"]);
                }

                //Add DocumentTotals
                sb.Append(string.Format(@"    <DocumentTotals>
      <ns2:TaxPayable>{0}</ns2:TaxPayable>
      <ns2:NetTotal>{1}</ns2:NetTotal>
      <ns2:GrossTotal>{2}</ns2:GrossTotal>
    </DocumentTotals>"
                    , DecimalToString(taxPayable)
                    , DecimalToString(netTotal)
                    , DecimalToString(grossTotal)
                ));

                result = sb.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //WayBill

        public static string GetDocumentWayBillContentLines(fin_documentfinancemaster pDocumentMaster)
        {

            string result;
            try
            {
                string sql = string.Format(@"
                    SELECT 
	                    Designation AS ProductDescription, Quantity, UnitMeasure AS UnitOfMeasure, Price AS UnitPrice,
	                    (SELECT rf.OriginatingON FROM fin_documentfinancedetailorderreference AS rf WHERE rf.DocumentDetail = fd.Oid) AS OrderReferences
                    FROM 
	                    fin_documentfinancedetail AS fd 
                    WHERE 
	                    DocumentMaster = '{0}' 
                    ORDER BY 
	                    Ord
                    ;"
                    , pDocumentMaster.Oid
                );

                DataTable dtResult = GetDataTableFromQuery(sql);

                //Init StringBuilder
                StringBuilder sb = new StringBuilder();

                foreach (DataRow item in dtResult.Rows)
                {
                    string orderReferences = !string.IsNullOrEmpty(item["OrderReferences"].ToString())
                        ? string.Format(@"{0}      <OrderReferences>
        <OriginatingON>{1}</OriginatingON>
      </OrderReferences>", Environment.NewLine, item["OrderReferences"])
                        : string.Empty
                    ;

                    sb.Append(string.Format(@"    <Line>{0}
      <ProductDescription>{1}</ProductDescription>
      <Quantity>{2}</Quantity>
      <UnitOfMeasure>{3}</UnitOfMeasure>
      <UnitPrice>{4}</UnitPrice>
    </Line>
"
                        , orderReferences
                        , SecurityElement.Escape(item["ProductDescription"].ToString())
                        , DecimalToString(Convert.ToDecimal(item["Quantity"]))
                        , item["UnitOfMeasure"]
                        , DecimalToString(Convert.ToDecimal(item["UnitPrice"]))
                    ));
                }

                result = sb.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public static CurrenyDto GetUsdCurrencyDto()
        {
            var curreny = GetEntityById<cfg_configurationcurrency>(CultureSettings.USDCurrencyId);
            return MappingUtils.GetCurrencyDto(curreny);
        }

        public static bool InsertSystemPrint(
            Document financeMasterDto,
            PrintingFinancePaymentDto financePaymentDto,
            string printerDesignation,
            int printCopies,
            List<int> copyNames,
            bool isSecondPrint,
            string printMotive,
            Guid userDetailsId,
            Guid terminalId)
        {
            bool result = false;

            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                string designation = string.Empty;

                //Get Objects into Current UOW Session
                var userDetails = GetEntityById<sys_userdetail>(userDetailsId, unitOfWork);
                var terminal = GetEntityById<pos_configurationplaceterminal>(terminalId, unitOfWork);

                string copyNamesCommaDelimited = string.Join(",", copyNames);

                sys_systemprint systemPrint = new sys_systemprint(unitOfWork)
                {
                    Date = CurrentDateTimeAtomic(),
                    Designation = designation,
                    PrintCopies = printCopies,
                    CopyNames = copyNamesCommaDelimited,
                    SecondPrint = isSecondPrint,
                    UserDetail = userDetails,
                    Terminal = terminal
                };
                if (printMotive != string.Empty) systemPrint.PrintMotive = printMotive;

                //Mode: DocumentFinanceMaster
                if (financeMasterDto != null)
                {
                    var financeMaster = GetEntityById<fin_documentfinancemaster>(financeMasterDto.Id, unitOfWork);
                    systemPrint.DocumentMaster = financeMaster;
                    designation = $"{CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, ResourceNames.GLOBAL_PRINTED)} {financeMaster.DocumentType.Designation} : {financeMaster.DocumentNumber}";

                    financeMaster.Printed = true;
                }

                //Mode: DocumentFinancePayment
                if (financePaymentDto != null)
                {
                    fin_documentfinancepayment documentFinancePayment = GetEntityById<fin_documentfinancepayment>(financePaymentDto.Id, unitOfWork);
                    systemPrint.DocumentPayment = documentFinancePayment;
                    designation = string.Format("{0} {1} : {2}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, ResourceNames.GLOBAL_PRINTED), documentFinancePayment.DocumentType.Designation, documentFinancePayment.PaymentRefNo);
                }
                systemPrint.Designation = designation;

                try
                {
                    //Commit UOW Changes : Before get current OrderMain
                    unitOfWork.CommitChanges();
                    //Audit
                    Audit("SYSTEM_PRINT_FINANCE_DOCUMENT", designation);
                    result = true;
                }
                catch (Exception ex)
                {
                    unitOfWork.RollbackTransaction();
                }
            }

            return result;
        }
    }
}
