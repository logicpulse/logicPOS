using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.datalayer.Xpo;
using logicpos.shared.Classes.Finance;
using logicpos.shared.Classes.Orders;
using logicpos.shared.Enums;
using LogicPOS.DTOs.Common;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.Settings.Enums;
using LogicPOS.Settings.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using static logicpos.datalayer.Xpo.XPOHelper;

namespace logicpos.shared.App
{
    public static class SharedUtils
    {
        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public static decimal GetGlobalDiscount()
        {
            decimal result = 0.0m;

            if (POSSession.CurrentSession != null
                && POSSession.CurrentSession.OrderMains.ContainsKey(POSSession.CurrentSession.CurrentOrderMainId)
            )
            {
                //get CurrentOrderMain
                OrderMain orderMain = POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId];
                //Get Table to Get Discount
                pos_configurationplacetable xConfigurationPlaceTable = (pos_configurationplacetable)GetXPGuidObject(XPOSettings.Session, typeof(pos_configurationplacetable), orderMain.Table.Oid);
                //Get Fresh Discount From Table/Future 
                if (xConfigurationPlaceTable != null)
                {
                    result = xConfigurationPlaceTable.Discount;
                }
            }

            return result;
        }

        public static PriceProperties GetArticlePrice(fin_article pArticle, TaxSellType pTaxSellType)
        {
            //get PriceType from CurrentOrderMain Table
            OrderMain orderMain = POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId];
            return GetArticlePrice(pArticle, orderMain.Table.PriceType, pTaxSellType);
        }

        public static PriceProperties GetArticlePrice(fin_article pArticle, PriceType pPriceType, TaxSellType pTaxSellType)
        {
            decimal priceSource = 0.0m;
            decimal priceDefault = 0.0m;
            decimal priceTax = 0.0m;

            // Get priceTax Based on AppOperationMode : in retail mode VatOnTable is always null
            if (AppOperationModeSettings.AppMode == AppOperationMode.Default)
            {
                //Protecções de integridade das BD's e funcionamento da aplicação [IN:013327]
                // Default : Restaurants with dual Tax ex Normal, TakeAway
                if (pTaxSellType == TaxSellType.Normal && pArticle.VatOnTable != null) priceTax = pArticle.VatOnTable.Value;
                else if (pArticle.VatDirectSelling != null) priceTax = pArticle.VatDirectSelling.Value;
            }
            else if (AppOperationModeSettings.AppMode == AppOperationMode.Retail)
            {
                // Mono priceTax 
                if (pArticle.VatDirectSelling != null)
                    priceTax = pArticle.VatDirectSelling.Value;
            }

            //Default Price, used when others are less or equal to zero
            if (pArticle.Price1UsePromotionPrice && pArticle.Price1Promotion > 0)
            {
                priceDefault = pArticle.Price1Promotion;
            }
            else if (pArticle.Price1 > 0)
            {
                priceDefault = pArticle.Price1;
            }

            if (pArticle != null)
            {
                switch (pPriceType)
                {
                    case PriceType.Price1:
                        priceSource = priceDefault;
                        break;
                    case PriceType.Price2:
                        if (pArticle.Price2UsePromotionPrice && pArticle.Price2Promotion > 0)
                        {
                            priceSource = pArticle.Price2Promotion;
                        }
                        else if (pArticle.Price2 > 0)
                        {
                            priceSource = pArticle.Price2;
                        }
                        if (priceSource <= 0.0m) priceSource = priceDefault;
                        break;
                    case PriceType.Price3:
                        if (pArticle.Price3UsePromotionPrice && pArticle.Price3Promotion > 0)
                        {
                            priceSource = pArticle.Price3Promotion;
                        }
                        else if (pArticle.Price3 > 0)
                        {
                            priceSource = pArticle.Price3;
                        }
                        if (priceSource <= 0.0m) priceSource = priceDefault;
                        break;
                    case PriceType.Price4:
                        if (pArticle.Price4UsePromotionPrice && pArticle.Price4Promotion > 0)
                        {
                            priceSource = pArticle.Price4Promotion;
                        }
                        else if (pArticle.Price4 > 0)
                        {
                            priceSource = pArticle.Price4;
                        }
                        if (priceSource <= 0.0m) priceSource = priceDefault;
                        break;
                    case PriceType.Price5:
                        if (pArticle.Price5UsePromotionPrice && pArticle.Price5Promotion > 0)
                        {
                            priceSource = pArticle.Price5Promotion;
                        }
                        else if (pArticle.Price5 > 0)
                        {
                            priceSource = pArticle.Price5;
                        }
                        if (priceSource <= 0.0m) priceSource = priceDefault;
                        break;
                }
            }

            PricePropertiesSourceMode sourceMode = (!pArticle.PriceWithVat) ? PricePropertiesSourceMode.FromPriceNet : PricePropertiesSourceMode.FromPriceUser;

            //Get Price
            PriceProperties priceProperties = PriceProperties.GetPriceProperties(
              sourceMode,
              pArticle.PriceWithVat,
              priceSource,
              1.0m,
              pArticle.Discount,
              GetGlobalDiscount(),
              priceTax
            );

            //Return PriceProperties Object
            return priceProperties;
        }

        public static bool Audit(string pAuditTypeToken, string pDescription = "")
        {
            return Audit(
                XPOSettings.Session,
                XPOSettings.LoggedUser ?? null,
                XPOSettings.LoggedTerminal ?? null,
                pAuditTypeToken,
                pDescription
            );
        }

        public static bool Audit(Session pSession, sys_userdetail pLoggedUser, pos_configurationplaceterminal pLoggedTerminal, string pAuditTypeToken, string pDescription = "")
        {
            bool result = false;
            DateTime dateTime = CurrentDateTimeAtomic();
            string executeSql = string.Format(@"SELECT Oid FROM sys_systemaudittype WHERE (Disabled IS NULL or Disabled  <> 1) AND Token = '{0}';", pAuditTypeToken);

            //Check if has a Valid LoggedUser else Assign NULL to INSERT, usefull to log stuff when User is not Yet Logged
            //string loggedUserOid = (pLoggedUser != null) ? string.Format("'{0}'", pLoggedUser.Oid.ToString()) : "NULL";

            try
            {
                //Get auditType Guid from Query
                Guid guidAuditType = GetGuidFromQuery(executeSql);

                if (!guidAuditType.Equals(Guid.Empty))
                {
                    //Fresh User and Terminal, to prevent Object Delection Problem
                    sys_userdetail xpoUserDetail = (pLoggedUser != null) ? (sys_userdetail)GetXPGuidObject(typeof(sys_userdetail), pLoggedUser.Oid) : null;
                    pos_configurationplaceterminal xpoTerminal = (pos_configurationplaceterminal)GetXPGuidObject(typeof(pos_configurationplaceterminal), pLoggedTerminal.Oid);
                    //get AuditType Object
                    sys_systemaudittype xpoAuditType = (sys_systemaudittype)GetXPGuidObject(typeof(sys_systemaudittype), guidAuditType);
                    string description = (pDescription != string.Empty) ? pDescription
                      : (xpoAuditType.ResourceString != null && CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), xpoAuditType.ResourceString) != null)
                      ? CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), xpoAuditType.ResourceString) : xpoAuditType.Designation;

                    sys_systemaudit systemAudit = new sys_systemaudit(pSession)
                    {
                        Date = dateTime,
                        Description = description,
                        UserDetail = xpoUserDetail,
                        Terminal = xpoTerminal,
                        AuditType = xpoAuditType
                    };
                    systemAudit.Save();

                    _logger.Debug(string.Format("Audit(): {0} > {1}", pAuditTypeToken, description));

                    result = true;
                }
                else
                {
                    string exceptionMessage = string.Format("Invalid AuditType: [{0}]", pAuditTypeToken);
                    _logger.Error(exceptionMessage);
                    //TriggerError
                    throw (new Exception(exceptionMessage));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Audit(): {0} > {1}", pAuditTypeToken, ex.Message), ex);
            }

            return result;
        }

        public static void ExecuteExternalProcess(string pExe)
        {
            ExecuteExternalProcess(pExe, string.Empty);
        }
        public static void ExecuteExternalProcess(string pExe, string pArgs)
        {
            try
            {
                var proc = new Process();

                proc.StartInfo.FileName = pExe;
                if (pArgs != null || pArgs != string.Empty) proc.StartInfo.Arguments = pArgs;
                proc.Start();

                // GlobalApp.WindowReportsWinForm = new logicpos.Classes.Gui.WinForms.BackOffice.Windows.BackOfficeReportWindow();
                _logger.Debug("Iniciar " + pExe);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        public static bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }

        public static bool UsePosPDFViewer()
        {
            bool result = false;

            try
            {
                result = Convert.ToBoolean(GeneralSettings.PreferenceParameters["USE_POS_PDF_VIEWER"]);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return (result);
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

            return (partialPayedItems != null && Convert.ToDecimal(partialPayedItems) > 0)
              ? Convert.ToDecimal(partialPayedItems)
              : result;
        }

        public static Dictionary<string, string> GetPreferencesParameters()
        {
            return GetPreferencesParameters(XPOSettings.Session);
        }

        public static Dictionary<string, string> GetPreferencesParameters(Session pSession)
        {
            try
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
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return null;
            }
        }

        public static List<int> CommaDelimitedStringToIntList(string pInput)
        {
            return CommaDelimitedStringToIntList(pInput, ',');
        }

        public static List<int> CommaDelimitedStringToIntList(string pInput, char pSeparator)
        {
            string[] arrayString = pInput.Split(pSeparator);
            List<int> listResult = new List<int>();

            for (int i = 0; i < arrayString.Length; i++)
            {
                int integer;
                bool res = int.TryParse(arrayString[i], out integer);

                if (res)
                {
                    listResult.Add(integer);
                }

            }
            return listResult;
        }

        public static string StringListToCommaDelimitedString<T>(List<T> pList, char pSeparator)
        {
            string result = string.Empty;

            try
            {
                int i = 0;
                foreach (var item in pList)
                {
                    i++;
                    result += item.ToString();
                    if (i < pList.Count) result += pSeparator.ToString();
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return result;
        }

        public static Dictionary<string, string> CSVFileToDictionary(string pFilePath)
        {
            return CSVFileToDictionary(pFilePath, ',');
        }

        public static Dictionary<string, string> CSVFileToDictionary(string pFilePath, char pSplitter)
        {
            bool debug = false;
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (File.Exists(pFilePath))
            {
                StreamReader streamReader = new StreamReader(File.OpenRead(pFilePath));

                try
                {
                    while (!streamReader.EndOfStream)
                    {
                        var line = streamReader.ReadLine();
                        var values = line.Split(pSplitter);

                        if (values.Length == 2 && !string.IsNullOrEmpty(values[0]) && !string.IsNullOrEmpty(values[1]))
                        {
                            string val1 = values[0];
                            string val2 = values[1];

                            if (!result.ContainsKey(val1))
                            {
                                if (debug) _logger.Debug(string.Format("CSVFileToDictionary Add: [{0}],[{1}]", val1, val2));
                                result.Add(val1, val2);
                            }
                        }
                    }
                    streamReader.Close();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                }
                finally
                {
                    streamReader.Close();
                }
            }

            return result;
        }

        public static List<T> MergeGenericLists<T>(List<List<T>> pLists)
        {
            List<T> result = new List<T>();

            for (int i = 0; i < pLists.Count; i++)
            {
                result = result.Concat(pLists[i]).ToList();
            }

            return result.Distinct().ToList();
        }

        public static Dictionary<string, bool> GetUserPermissions()
        {
            return GetUserPermissions(XPOSettings.LoggedUser);
        }

        public static Dictionary<string, bool> GetUserPermissions(sys_userdetail pUser)
        {
            bool debug = false;
            Dictionary<string, bool> resultPermissions = new Dictionary<string, bool>();

            try
            {
                //Shared Permissions For All Users
                resultPermissions.Add("BACKOFFICE_MAN_SYSTEM_MENU_MENU", true);
                resultPermissions.Add("BACKOFFICE_MAN_SYSTEM_POS_MENU", true);
                resultPermissions.Add("BACKOFFICE_MAN_SYSTEM_QUIT_MENU", true);
                resultPermissions.Add("BACKOFFICE_MAN_SYSTEM_NOTIFICATION_MENU", true);
                resultPermissions.Add("BACKOFFICE_MAN_SYSTEM_CHANGELOG_MENU", true);

                if (pUser != null)
                {
                    if (pUser.Profile != null)
                    {
                        foreach (sys_userpermissionprofile item in pUser.Profile.Permissions)
                        {
                            if (debug) _logger.Debug(string.Format("Permission: Token [{0}], Granted [{1}]", item.PermissionItem.Token, item.Granted));
                            resultPermissions.Add(item.PermissionItem.Token, item.Granted);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //If error Detect Check Duplicated PermissionItems in Profile
                //SELECT upiToken,COUNT(*) as Count FROM view_userprofile WHERE uprOid = '1626e21f-75e6-429e-b0ac-edb755e733c2' GROUP BY upiToken ORDER BY COUNT(*) DESC,upiCode;
                _logger.Error(ex.Message, ex);
            }
            return resultPermissions;
        }

        public static bool HasPermissionTo(string pToken)
        {
            bool result;
            if (SharedFramework.LoggedUserPermissions != null && SharedFramework.LoggedUserPermissions.ContainsKey(pToken))
            {
                result = SharedFramework.LoggedUserPermissions[pToken];
            }
            else
            {
                result = false;
            }

            //_logger.Debug(string.Format("HasPermissionTo({0}): {1}", pToken, result));
            return result;
        }

        public static void SystemNotification()
        {
            SystemNotification(XPOSettings.Session);
        }

        public static void SystemNotification(Session pSession)
        {
            _logger.Debug("void FrameworkUtils.SystemNotification(Session pSession) :: pSession: " + pSession.ToString());
            bool debug = true;

            //Filter 
            //To All: "NotificationType = '{0}'"
            //To User: "NotificationType = '{0}' AND UserLastRead = '{1}'"
            //To Terminal: "NotificationType = '{0}' AND TerminalLastRead = '{1}'"
            //To User Terminal: "NotificationType = '{0}' AND UserLastRead = '{1}' AND TerminalLastRead = '{1}'"
            try
            {
                //Settings
                string cultureFinancialRules = GeneralSettings.Settings["cultureFinancialRules"];

                //Local Vars
                uint ord = 1;
                sys_systemnotificationtype systemNotificationType;
                sys_systemnotification systemNotification;
                CriteriaOperator criteriaOperator;
                XPCollection xpcSystemNotification;

                //:::: Notification : Edit Configuration Parameters ::::
                //Check existing Notification before Create
                //systemNotificationType = (sys_systemnotificationtype)pSession.GetObjectByKey(typeof(sys_systemnotificationtype), SettingsApp.XpoOidSystemNotificationTypeEditConfigurationParameters);
                //if (systemNotificationType != null)
                //{
                //    criteriaOperator = CriteriaOperator.Parse(string.Format("NotificationType = '{0}'", SettingsApp.XpoOidSystemNotificationTypeEditConfigurationParameters));
                //    xpcSystemNotification = new XPCollection(pSession, typeof(sys_systemnotification), criteriaOperator);
                //    //Create Notification
                //    if (xpcSystemNotification.Count == 0)
                //    {
                //        systemNotification = new sys_systemnotification(pSession);
                //        systemNotification.Ord = ord;
                //        systemNotification.NotificationType = systemNotificationType;
                //        systemNotification.Message = systemNotificationType.Message;
                //        systemNotification.Save();
                //        ord++;
                //        if (debug) _logger.Debug(string.Format("Notification created: [{0}]", systemNotificationType.Designation));
                //    }
                //};

                //:::: Notification : WelcomeMessage ::::
                //Check existing Notification before Create
                systemNotificationType = (sys_systemnotificationtype)pSession.GetObjectByKey(typeof(sys_systemnotificationtype), NotificationSettings.XpoOidSystemNotificationTypeNewTerminalRegistered);
                if (systemNotificationType != null)
                {
                    criteriaOperator = CriteriaOperator.Parse(string.Format("NotificationType = '{0}' AND TerminalLastRead = '{1}'", NotificationSettings.XpoOidSystemNotificationTypeNewTerminalRegistered, XPOSettings.LoggedTerminal.Oid));
                    xpcSystemNotification = new XPCollection(pSession, typeof(sys_systemnotification), criteriaOperator);
                    //Create Notification
                    if (xpcSystemNotification.Count == 0)
                    {
                        systemNotification = new sys_systemnotification(pSession);
                        systemNotification.Ord = ord;
                        systemNotification.NotificationType = systemNotificationType;
                        systemNotification.Message = string.Format(systemNotificationType.Message, XPOSettings.LoggedTerminal.Designation);
                        systemNotification.Save();
                        ord++;
                        if (debug) _logger.Debug(string.Format("Notification created: [{0}]", systemNotificationType.Designation));
                    }
                };

                //:::: Notification : RequestPasswordChange ::::
                //Check existing Notification before Create
                /* DISABLE Currently in Progress
                systemNotificationType = (SystemNotificationType)pSession.GetObjectByKey(typeof(SystemNotificationType), SettingsApp.XpoOidSystemNotificationTypeFirstLoginRequestPasswordChange);
                if (systemNotificationType != null)
                {
                    criteriaOperator = CriteriaOperator.Parse(string.Format("NotificationType = '{0}' AND UserLastRead = '{1}'", SettingsApp.XpoOidSystemNotificationTypeFirstLoginRequestPasswordChange, XPOSettings.LoggedUser));
                    xpcSystemNotification = new XPCollection(pSession, typeof(SystemNotification), criteriaOperator);
                    //Create Notification
                    if (xpcSystemNotification.Count == 0)
                    {
                        systemNotification = new SystemNotification(pSession);
                        systemNotification.Ord = ord;
                        systemNotification.NotificationType = systemNotificationType;
                        systemNotification.Message = string.Format(systemNotificationType.Message, XPOSettings.LoggedUser.Login, XPOSettings.LoggedUser.Name);
                        systemNotification.Save();
                        ord++;
                        if (debug) _logger.Debug(string.Format("Notification created: [{0}]", systemNotificationType.Designation));
                    }
                };
                */

                //:::: Notification : ProprietaryTestMessage ::::
                //Check existing Notification before Create
                /*TEMP REMOVED FOR MediaNova
                systemNotificationType = (SystemNotificationType)pSession.GetObjectByKey(typeof(SystemNotificationType), SettingsApp.XpoOidSystemNotificationTypeProprietaryTestMessage);
                criteriaOperator = CriteriaOperator.Parse(string.Format("NotificationType = '{0}' AND UserTarget = '{1}'", SettingsApp.XpoOidSystemNotificationTypeProprietaryTestMessage, userProprietary));
                xpcSystemNotification = new XPCollection(pSession, typeof(SystemNotification), criteriaOperator);
                //Create Notification
                if (xpcSystemNotification.Count == 0)
                {
                    UserDetail user = (UserDetail)pSession.GetObjectByKey(typeof(UserDetail), new Guid(userProprietary));
                    systemNotification = new SystemNotification(pSession)
                    {
                        Ord = ord,
                        NotificationType = systemNotificationType,
                        Message = string.Format(systemNotificationType.Message, user.Name),
                        UserTarget = user
                    };
                    systemNotification.Save();
                    ord++;
                    if (debug) _logger.Debug(string.Format("Notification created: [{0}]", systemNotificationType.Designation));
                }
                */

                //Financial Culture Notifications
                //switch (LogicPOS.Settings.CultureSettings.CurrentCulture.ToString())
                switch (cultureFinancialRules)
                {
                    case "pt-PT":
                        int defaultBackDaysForInvoice = NotificationSettings.XpoOidSystemNotificationDaysBackWhenFiltering;

                        //:::: Notification : CurrentAccountDocumentsToInvoice ::::
                        //ProcessFinanceDocumentToInvoice to Create Notification in Spool for CurrentAccount Documents
                        //systemNotificationType = (SystemNotificationType)pSession.GetObjectByKey(typeof(SystemNotificationType), SettingsApp.XpoOidSystemNotificationTypeCurrentAccountDocumentsToInvoice);
                        systemNotification = ProcessFinanceDocumentToInvoice(pSession, NotificationSettings.XpoOidSystemNotificationTypeCurrentAccountDocumentsToInvoice, DocumentSettings.XpoOidDocumentFinanceTypeCurrentAccountInput, "(Payed = 0 OR Payed IS NULL)", defaultBackDaysForInvoice);
                        if (systemNotification != null)
                        {
                            systemNotification.Ord = ord; systemNotification.Save(); ord++;
                            if (debug) _logger.Debug(string.Format("Notification created: [{0}]", systemNotificationType.Designation));
                        };

                        //:::: Notification : ConsignationInvoiceDocumentsToInvoice ::::
                        //ProcessFinanceDocumentToInvoice to Create Notification in Spool for CurrentAccount Documents
                        //systemNotificationType = (SystemNotificationType)pSession.GetObjectByKey(typeof(SystemNotificationType), SettingsApp.XpoOidSystemNotificationTypeConsignationInvoiceDocumentsToInvoice);
                        systemNotification = ProcessFinanceDocumentToInvoice(pSession, NotificationSettings.XpoOidSystemNotificationTypeConsignationInvoiceDocumentsToInvoice, DocumentSettings.XpoOidDocumentFinanceTypeConsignationInvoice, "(DocumentChild IS NULL)", defaultBackDaysForInvoice);
                        if (systemNotification != null)
                        {
                            systemNotification.Ord = ord; systemNotification.Save(); ord++;
                            if (debug) _logger.Debug(string.Format("Notification created: [{0}]", systemNotificationType.Designation));
                        };

                        //:::: Notification : SaftDocumentType.MovementOfGoodsToInvoice ::::
                        //ProcessFinanceDocumentToInvoice to Create Notification in Spool for CurrentAccount Documents
                        systemNotification = ProcessFinanceDocumentToInvoice(pSession, NotificationSettings.XpoOidSystemNotificationTypeSaftDocumentTypeMovementOfGoods, SaftDocumentType.MovementOfGoods, "(DocumentChild IS NULL AND DocumentStatusStatus = 'N')", defaultBackDaysForInvoice);
                        if (systemNotification != null)
                        {
                            systemNotification.Ord = ord; systemNotification.Save(); ord++;
                            if (debug) _logger.Debug(string.Format("Notification created: [{0}]", systemNotificationType.Designation));
                        };

                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
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
                    filter += (string.Format("DocumentType = '{0}'{1}", item.Oid, (i < xpcDocumentFinanceType.Count) ? " OR " : string.Empty));
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
            try
            {
                ignoreNotificationsAfterHaveBeenNotificatedNumberOfTimes = Convert.ToInt16(GeneralSettings.PreferenceParameters["NOTIFICATION_DOCUMENTS_TO_INVOICE_IGNORE_AFTER_SHOW_NUMBER_OF_TIMES"]);
            }
            catch (Exception)
            {
                _logger.Error(string.Format("Error using PreferenceParameters: NOTIFICATION_DOCUMENTS_TO_INVOICE_IGNORE_AFTER_SHOW_NUMBER_OF_TIMES, using default Value of : [{0}]", ignoreNotificationsAfterHaveBeenNotificatedNumberOfTimes));
            }

            try
            {
                //Get Date Back DaysBackToFilter (Without WeekEnds and Holidays)
                int warnDaysBefore = (systemNotificationType.WarnDaysBefore > 0) ? systemNotificationType.WarnDaysBefore : 0;
                int daysBackToFilter = pDaysBackToFilter - warnDaysBefore;
                DateTime dateFilterFrom = GetDateTimeBackUtilDays(CurrentDateTimeAtomicMidnight(), daysBackToFilter, true);

                //Extra Filter 
                string filter = pFilter;
                if (pExtraFilter != string.Empty) filter = string.Format("{0} AND {1}", filter, pExtraFilter);
                filter = string.Format("{0} AND (Date <= '{1} 23:59:59')", filter, dateFilterFrom.ToString(CultureSettings.DateFormat));

                CriteriaOperator criteriaOperator = CriteriaOperator.Parse(filter);
                SortProperty sortProperty = new SortProperty("CreatedAt", SortingDirection.Ascending);
                XPCollection xpcDocumentFinanceMaster = new XPCollection(pSession, typeof(fin_documentfinancemaster), criteriaOperator, sortProperty);

                // Debug Helper
                //if (pSystemNotificationTypeGuid.Equals(SettingsApp.XpoOidSystemNotificationTypeSaftDocumentTypeMovementOfGoods))
                //{
                //    _logger.Debug("BREAK");
                //}

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
                                CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(),
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
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return result;
        }

        public static bool Validate(string pValidateValue, string pRule, bool pRequired)
        {
            bool result = false;

            try
            {
                //Without Rule
                if (pRule == null || pRule == string.Empty)
                {
                    if (pRequired)
                    {
                        if (pValidateValue == string.Empty)
                        {
                            result = false;
                        }
                        else if (pValidateValue != string.Empty)
                        {
                            result = true;
                        }
                    }
                    else
                    {
                        result = true;
                    }
                }
                //With Rule
                else
                {
                    if (pRequired)
                    {
                        if (pValidateValue != string.Empty && ValidateString(pValidateValue, pRule))
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }
                    }
                    else
                    {
                        //If not Required and as a Empty Value or Empty Guid
                        if (pValidateValue == string.Empty || pValidateValue == Convert.ToString(Guid.Empty))
                        {
                            result = true;
                        }
                        else
                            if (pValidateValue != string.Empty && ValidateString(pValidateValue, pRule))
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return result;
        }

        public static ValidateMaxLenghtMaxWordsResult ValidateMaxLenghtMaxWords(string pValue, string pIinitialLabelText, int pMaxLength, int pMaxWords)
        {
            ValidateMaxLenghtMaxWordsResult result = new ValidateMaxLenghtMaxWordsResult();

            try
            {
                result.Text = pValue;
                result.Length = pValue.Length;

                string lengthLabelText = string.Empty;
                string maxWordsLabelText = string.Empty;

                if (pMaxLength > 0)
                {
                    if (result.Length > pMaxLength)
                    {
                        result.Length = pMaxLength;
                        //Cut Text
                        result.Text = pValue.Substring(0, pMaxLength);
                    }
                    lengthLabelText = string.Format("{0}: {1}/{2}", CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_characters"), result.Length, pMaxLength);
                }
                var minWordLengthConsidered = Convert.ToInt16(GeneralSettings.Settings["MinWordLengthConsidered"]);

                result.Words = LogicPOS.Utility.StringUtils.GetNumWords(
                    result.Text,
                    minWordLengthConsidered);

                if (pMaxWords > 0)
                {
                    if (result.Words > pMaxWords)
                    {
                        result.Words = pMaxWords;
                        result.Text = LogicPOS.Utility.StringUtils.GetWords(result.Text, pMaxWords);
                    }
                    maxWordsLabelText = string.Format("{0}: {1}/{2}", CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_words"), result.Words, pMaxWords);
                }

                if (result.Length > 0)
                {
                    string addToLabelText = string.Empty;
                    if (lengthLabelText != string.Empty) addToLabelText += lengthLabelText;
                    if (lengthLabelText != string.Empty && maxWordsLabelText != string.Empty) addToLabelText += " ";
                    if (maxWordsLabelText != string.Empty) addToLabelText += maxWordsLabelText;

                    if (addToLabelText != string.Empty)
                    {
                        result.LabelText = string.Format("{0} : ({1})", pIinitialLabelText, addToLabelText);
                    }
                }
                else
                {
                    result.LabelText = pIinitialLabelText;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return result;
        }

        public static bool ValidateString(string pValidate, string pRegExRule, Type pType = null)
        {
            //If Type Decimal and User uses "," replace it by "."
            if (pType == typeof(decimal)) pValidate = pValidate.Replace(',', '.');

            //Check if can Convert to type
            if (pType != null && !CanConvert(pValidate, pType))
            {
                return false;
            };

            //Check if is a Blank GUID 
            if (pValidate == new Guid().ToString() && pRegExRule == LogicPOS.Utility.RegexUtils.RegexGuid)
            {
                return false;
            }

            if (pValidate != string.Empty && pRegExRule != string.Empty)
            {
                try
                {
                    return Regex.IsMatch(pValidate, pRegExRule);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool CanConvert(string pValue, Type pType)
        {
            if (string.IsNullOrEmpty(pValue) || pType == null) return false;
            System.ComponentModel.TypeConverter typeConverter = System.ComponentModel.TypeDescriptor.GetConverter(pType);
            if (typeConverter.CanConvertFrom(typeof(string)))
            {
                try
                {
                    //typeConverter.ConvertFrom(pValue);
                    typeConverter.ConvertFrom(null, CultureSettings.CurrentCultureNumberFormat, pValue);
                    return true;
                }
                catch (Exception)
                {
                    //Hide Error from log
                    //_logger.Error(ex.Message, ex);
                    return false;
                }
            }
            return false;
        }

        public static object GetFieldValueFromType(Type pType, string pFieldName)
        {
            object result = null;

            try
            {
                //pFieldName = "DatabaseName";//Works with Current Settings
                //pFieldName = "AppCompanyName";//Works with Base Class
                //object o = pType.BaseType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.FlattenHierarchy);

                //Trick to get current and base class Fields
                FieldInfo fieldInfo = pType.GetField(pFieldName, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                //Note: use first parameter has null, static classes cannot be instanced, else use object
                if (fieldInfo != null) result = fieldInfo.GetValue(null);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return result;
        }
    }
}
