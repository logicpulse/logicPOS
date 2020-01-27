using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Gtk;
using logicpos.App;
using logicpos.datalayer.Enums;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace logicpos
{
    public class DataLayer
    {
        /// <summary>
        /// Create initial database Scheme and Initial scripts
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <param name="DatabaseType"></param>
        /// <param name="DatabaseName"></param>
        /// <returns></returns>
        public static bool CreateDatabaseSchema(string pXpoConnectionString, DatabaseType pDatabaseType, string pDatabaseName)
        {
            //Log4Net
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            bool result = false;
            string xpoConnectionString = pXpoConnectionString;
            DatabaseType databaseType = pDatabaseType;
            string databaseTypeString = Enum.GetName(typeof(DatabaseType), GlobalFramework.DatabaseType);
            string databaseName = pDatabaseName;
            IDataLayer xpoDataLayer = null;
            bool onErrorsDropDatabase = true;
            string sql = string.Empty;
            object resultCmd;
            Hashtable commands = new Hashtable();
            string commandSeparator = ";";
            bool databaseExists = false;
            Session xpoSession;
            Dictionary<string, string> replace = GetReplaceables(pDatabaseType);

            string sqlDatabaseSchema = FrameworkUtils.OSSlash(string.Format(SettingsApp.FileDatabaseSchema, databaseTypeString));
            //string sqlDatabaseOtherDatabaseType = FrameworkUtils.OSSlash(string.Format(SettingsApp.FileDatabaseOtherDatabaseType, databaseTypeString)); /* IN009045: Not in use */
            string sqlDatabaseOtherCommon = FrameworkUtils.OSSlash(SettingsApp.FileDatabaseOtherCommon);
            /* IN008024 and after IN009035: data being included by databasedata.sql accordingly to its specific theme/language */
            // string sqlDatabaseOtherCommonAppMode = string.Format("{0}/{1}", FrameworkUtils.OSSlash(SettingsApp.FileDatabaseOtherCommonAppMode), SettingsApp.CustomAppOperationMode.AppOperationTheme.ToLower());
            string sqlDatabaseOtherCommonPluginsSoftwareVendor = FrameworkUtils.OSSlash(SettingsApp.FileDatabaseOtherCommonPluginsSoftwareVendor);
            string FileDatabaseOtherCommonPluginsSoftwareVendorOtherCommonCountry = FrameworkUtils.OSSlash(SettingsApp.FileDatabaseOtherCommonPluginsSoftwareVendor);
            string sqlDatabaseData = FrameworkUtils.OSSlash(SettingsApp.FileDatabaseData);
            string sqlDatabaseDataDemo = FrameworkUtils.OSSlash(SettingsApp.FileDatabaseDataDemo);
            string sqlDatabaseViews = FrameworkUtils.OSSlash(SettingsApp.FileDatabaseViews);
            bool useDatabaseDataDemo = Convert.ToBoolean(GlobalFramework.Settings["useDatabaseDataDemo"]);

            switch (databaseType)
            {
                case DatabaseType.SQLite:
                case DatabaseType.MonoLite:
                    //connectionstring = string.Format(GlobalFramework.Settings["xpoConnectionString"], databaseName);
                    break;
                case DatabaseType.MSSqlServer:
                    //Required to Remove DataBase Name From Connection String
                    xpoConnectionString = xpoConnectionString.Replace(string.Format("Initial Catalog={0};", pDatabaseName), string.Empty);
                    commands.Add("select_schema", string.Format(@"SELECT name FROM sys.databases WHERE name = '{0}' AND name NOT IN ('master', 'tempdb', 'model', 'msdb');", databaseName));
                    commands.Add("create_database", string.Format(@"IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = '{0}') CREATE DATABASE {0};", databaseName));
                    commands.Add("use_database", string.Format(@"USE {0};", databaseName));
                    commands.Add("drop_database", string.Format(@"USE master; IF EXISTS(SELECT name FROM sys.databases WHERE name = '{0}') DROP DATABASE {0};", databaseName));
                    //ByPass Default commandSeparator ;
                    commandSeparator = "GO";
                    break;
                case DatabaseType.MySql:
                    //Required to Remove DataBase Name From Connection String
                    xpoConnectionString = xpoConnectionString.Replace(string.Format("database={0};", pDatabaseName), string.Empty);
                    commands.Add("select_schema", string.Format(@"SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '{0}';", databaseName));
                    commands.Add("create_database", string.Format(@"CREATE DATABASE IF NOT EXISTS {0} CHARACTER SET utf8 COLLATE utf8_bin /*!40100 DEFAULT CHARACTER SET utf8*/;", databaseName));
                    commands.Add("use_database", string.Format(@"USE {0};", databaseName));
                    commands.Add("drop_database", string.Format(@"DROP DATABASE IF EXISTS {0};", databaseName));
                    break;
            }

            //Get DataLayer
            try
            {
                xpoDataLayer = XpoDefault.GetDataLayer(xpoConnectionString, AutoCreateOption.None);
            }
            catch (Exception ex)
            {
                log.Error(string.Format("CreateDatabaseSchema(): {0}", ex.Message), ex);
                throw;
            }

            //Init Session
            xpoSession = new Session(xpoDataLayer);

            //Start CreateDatabaseSchema Process
            try
            {
                switch (databaseType)
                {
                    case DatabaseType.SQLite:
                    case DatabaseType.MonoLite:
                        string filename = string.Format("{0}.db", databaseName);
                        databaseExists = (File.Exists(filename) && new FileInfo(filename).Length > 0);
                        log.Debug(string.Format("DatabaseExists: [{0}], databaseName: [{1}]", databaseExists, string.Format("{0}.db", databaseName)));
                        break;
                    case DatabaseType.MSSqlServer:
                    case DatabaseType.MySql:
                    default:
                        sql = commands["select_schema"].ToString();
                        //log.Debug(string.Format("ExecuteScalar: [{0}]", sql));
                        resultCmd = xpoSession.ExecuteScalar(sql);
                        log.Debug(string.Format("Use Database resultCmd: [{0}]", resultCmd));
                        databaseExists = ((string)resultCmd == databaseName);
                        log.Debug(string.Format("DatabaseExists:[{0}] [{1}]", databaseName, databaseExists));
                        break;
                }

                //Create Database and Data
                if (!databaseExists)
                {
                    log.Debug(string.Format("Creating {0} Database: [{1}]", databaseType, databaseName));

                    //Always Delete Old appsession.json file when Create new Database
                    if (File.Exists(Utils.GetSessionFileName())) File.Delete(Utils.GetSessionFileName());

                    if (pDatabaseType != DatabaseType.SQLite && pDatabaseType != DatabaseType.MonoLite)
                    {
                        sql = commands["create_database"].ToString();
                        log.Debug(string.Format("ExecuteScalar: [{0}]", sql));
                        resultCmd = xpoSession.ExecuteScalar(sql);
                        log.Debug(string.Format("Create Database resultCmd: [{0}]", resultCmd));

                        sql = commands["use_database"].ToString();
                        log.Debug(string.Format("ExecuteScalar: [{0}]", sql));
                        resultCmd = xpoSession.ExecuteScalar(sql);
                        log.Debug(string.Format("Use Database resultCmd: [{0}]", resultCmd));
                    }

                    //Restore Script Files

                    //Schema
                    result = ProcessDump(xpoSession, sqlDatabaseSchema, commandSeparator, replace);
                    //Data
                    if (result)
                    {
                        result = ProcessDump(xpoSession, sqlDatabaseData, ";", replace);
                    }
                    //DataDemo
                    if (useDatabaseDataDemo && result)
                    {
                        result = ProcessDump(xpoSession, sqlDatabaseDataDemo, ";", replace);
                    }
                    //Process Other Files: DatabaseOtherCommonPluginsSoftwareVendor
                    if (result)
                    {
                        result = ProcessDumpDirectory(xpoSession, sqlDatabaseOtherCommonPluginsSoftwareVendor, ";", replace);
                    }
                    //Views
                    if (result)
                    {
                        result = ProcessDump(xpoSession, sqlDatabaseViews, ";", replace);
                    }
                    //Directory Scripts
                    //Process Other Files: DatabaseOtherDatabaseType
                    /* IN009045: not in use */
                    /*if (result)
                    {
                        result = ProcessDumpDirectory(xpoSession, sqlDatabaseOtherDatabaseType, ";", replace);//commandSeparator
                    }*/
                    //Process Other Files: DatabaseOtherCommon
                    if (result)
                    {
                        result = ProcessDumpDirectory(xpoSession, sqlDatabaseOtherCommon, ";", replace); /* IN009045 */
                    }
                    ////Process Other Files: DatabaseOtherCommonPluginsSoftwareVendor
                    //if (result)
                    //{
                    //    result = ProcessDumpDirectory(xpoSession, sqlDatabaseOtherCommonPluginsSoftwareVendor, commandSeparator, replace);
                    //}
                    //Process Other Files: DatabaseOtherCommonAppMode
                    /* IN009045 and IN009035: data being included by databasedata.sql accordingly to its specific theme/language */
                    /*if (result)
                    {
                        result = ProcessDumpDirectory(xpoSession, sqlDatabaseOtherCommonAppMode, ";", replace);
                    }*/

                    //Clean ConfigurationPreferenceParameter
                    string sqlConfigurationPreferenceParameter = @"UPDATE cfg_configurationpreferenceparameter SET Value = NULL WHERE (Token = 'COMPANY_COUNTRY' OR Token = 'COMPANY_COUNTRY_CODE2' OR Token = 'SYSTEM_CURRENCY' OR Token = 'COMPANY_COUNTRY_OID' OR Token = 'SYSTEM_CURRENCY_OID')";
                    if (result && Debugger.IsAttached == true)
                    {
                        xpoSession.ExecuteScalar(sqlConfigurationPreferenceParameter);
                    }
                    else
                    {
                        sqlConfigurationPreferenceParameter = string.Format("{0} {1}", sqlConfigurationPreferenceParameter, "OR (FormPageNo = 1 AND FormType = 1 AND Token <> 'COMPANY_TAX_ENTITY')");
                        xpoSession.ExecuteScalar(sqlConfigurationPreferenceParameter);
                    }
                }
                else
                {
                    log.Debug(string.Format("{0} Database: [{1}] Already Exist! Skip Creating Database", databaseType, databaseName));
                    result = false;
                    return false;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                result = false;
            }

            //If detect errors Drop Incompleted Database
            if (onErrorsDropDatabase && !result)
            {
                //Drop Database 
                sql = commands["drop_database"].ToString();
                log.Debug(string.Format("ExecuteScalar: [{0}]", sql));
                resultCmd = xpoSession.ExecuteScalar(sql);
                log.Debug(string.Format("Create Database resultCmd: [{0}]", resultCmd));
            }

            return result;
        }

        /// <summary>
        /// Check if current database Schema is Valid
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <returns></returns>
        public static bool IsSchemaValid(string pConnectionString)
        {
            //Log4Net
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            IDataLayer dl = XpoDefault.GetDataLayer(pConnectionString, AutoCreateOption.None);
            try
            {
                new Session(dl).UpdateSchema();
                return true;
            }
            catch (DevExpress.Xpo.DB.Exceptions.SchemaCorrectionNeededException ex)
            {
                log.Error(string.Format("IsSchemaValid(): [{0}]", ex.Message), ex);
                return false;
            }
        }

        /// <summary>
        /// GetReplaceables for current Database Type
        /// </summary>
        /// <param name="DataBaseType"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetReplaceables(DatabaseType pDataBaseType)
        {
            //Log4Net
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            string commandSeparator = string.Empty;
            Dictionary<string, string> result = new Dictionary<string, string>();

            try
            {
                switch (pDataBaseType)
                {
                    case DatabaseType.MySql:
                        /* IN009024 */
                        result.Add(@"\w", @"\\w");
                        result.Add(@"\d", @"\\d");
                        //result.Add(@"GETDATE()", @"'2018-10-01 23:16:18'");
                        //result.Add(@"\s", @"\\s");
                        break;
                    case DatabaseType.MonoLite:
                    case DatabaseType.SQLite:
                        //connectionstring = string.Format(GlobalFramework.Settings["xpoConnectionString"], databaseName);
                        //Replace content - Currently not used, Here only for Example
                        //result.Add("dm.Table", "dm.[Table]");
                        //result.Add("dt.Table", "dt.[Table]");
                        result.Add(@"\\", @"\");
                        result.Add("\\n", "' || CHAR(13) || '");
                        //view_articlestockmovement
                        result.Add("DATE_FORMAT(stk.Date, '%Y-%m-%d') AS stkDateDay,", "strftime('%Y-%m-%d', stk.Date) AS stkDateDay,");
                        //view_systemaudit
                        result.Add("DATE_FORMAT(sau.Date, '%Y-%m-%d') AS sauDateDay,", "strftime('%Y-%m-%d', sau.Date) AS sauDateDay,");
                        //view_systemaudit
                        result.Add("DATE_FORMAT(dmDateStart, '%Y-%m-%d') AS DateDay,", "strftime('%Y-%m-%d', dmDateStart) AS DateDay,");
                        //view_usercommission
                        result.Add("DATE_FORMAT(fmDate, '%Y-%m-%d') AS DateDay,", "strftime('%Y-%m-%d', fmDate) AS DateDay,");
                        break;
                    case DatabaseType.MSSqlServer:
                        //Replace content
                        result.Add(@"\\", @"\");
                        //Required to Replace with CHAR(13) else nothing seems to work
                        result.Add("\\n", "' + CHAR(13) + '");
                        // view_articlestockmovement
                        // Above SQLServer2008
                        //result.Add("DATE_FORMAT(stk.Date, '%Y-%m-%d') AS stkDateDay,", "FORMAT(stk.Date, 'yyyy-MM-dd', 'en-us') AS stkDateDay,");
                        // Lower SQLServer2008
                        result.Add("DATE_FORMAT(stk.Date, '%Y-%m-%d') AS stkDateDay,", "CONVERT(VARCHAR(19), stk.Date, 23) AS stkDateDay,");
                        // view_systemaudit
                        //result.Add("DATE_FORMAT(sau.Date, '%Y-%m-%d') AS sauDateDay,", "FORMAT(sau.Date, 'yyyy-MM-dd', 'en-us') AS sauDateDay,");
                        // Lower SQLServer2008
                        result.Add("DATE_FORMAT(sau.Date, '%Y-%m-%d') AS sauDateDay,", "CONVERT(VARCHAR(19), sau.Date, 23) AS sauDateDay,");
                        //view_documentfinance
                        result.Add("DATE_FORMAT(dmDateStart, '%Y-%m-%d') AS DateDay,", "CONVERT(VARCHAR(19), dmDateStart, 23) AS DateDay,");
                        //view_usercommission
                        result.Add("DATE_FORMAT(fmDate, '%Y-%m-%d') AS DateDay,", "CONVERT(VARCHAR(19), fmDate, 23) AS DateDay,");
                        //ByPass Default commandSeparator ;
                        commandSeparator = "GO";
                        break;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }

            return result;
        }

        /// <summary>
        /// Process filepath/filename script
        /// </summary>
        /// <param name="Session"></param>
        /// <param name="Filename"></param>
        /// <param name="CommandSeparator"></param>
        /// <param name="Replace"></param>
        /// <returns></returns>
        public static bool ProcessDump(Session pXpoSession, string pFilename, string pCommandSeparator, Dictionary<string, string> pReplaceables)
        {
            //Log4Net
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            log.Debug(string.Format("bool ProcessDump(Session pXpoSession, string pFilename, string pCommandSeparator, Dictionary<string, string> pReplaceables) :: ProcessDump Filename: [{0}]", pFilename));

            if (File.Exists(pFilename))
            {
                //Get Script Content
                FileInfo file = new FileInfo(pFilename);
                string script = file.OpenText().ReadToEnd() + "\r\n";

                //Replace Content before Process
                if (pReplaceables.Count > 0)
                {
                    foreach (KeyValuePair<string, string> item in pReplaceables)
                    {
                        script = script.Replace(item.Key, item.Value);
                    }
                }

                //if (pFilename.Equals("Resources/Database/databasedata.sql"))
                //{
                //    log.Debug("DEBUG");
                //}

                object result;
                string executeCommand;
                string[] commandSeparators = new string[] { pCommandSeparator };
                string[] commands;
                commands = script.Split(commandSeparators, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < commands.Length - 1; i++)
                {
                    //CarriageReturn \r\n = 
                    executeCommand = string.Format("{0};", FrameworkUtils.RemoveCarriageReturnAndExtraWhiteSpaces(commands[i]));
                    //Replace \n (Multiline Text like SEND_MAIL_FINANCE_DOCUMENTS_BODY)
                    executeCommand = executeCommand.Replace("\\n", Environment.NewLine);

                    //TODO: Muga melhorar isto : Move it to Replacable in DataBase Type in a Dynamic Value Action
                    executeCommand = executeCommand.Replace("</NEWGUI>", Guid.NewGuid().ToString());

                    // Helper to debug pReplaceables
                    //if (executeCommand.Contains("DATE_FORMAT"))
                    //if (executeCommand.Contains("3f3c562c-850d-452c-af1a-41f9c9e9c89e"))
                    //{
                    //    executeCommand = executeCommand.Replace("\\n", Environment.NewLine);
                    //    log.Debug("DEBUG");
                    //}

                    if (executeCommand != string.Empty && executeCommand != "\r\n" && !executeCommand.StartsWith("--"))
                    {
                        log.Debug(string.Format("{0}/{1}> [{2}]", i + 1, commands.Length - 1, executeCommand));
                        try
                        {
                            result = pXpoSession.ExecuteNonQuery(executeCommand);
                        }
                        catch (Exception ex)
                        {
							/* IN009021 */
                            //pXpoSession.RollbackTransaction();

                            string errorMessage = string.Format("bool ProcessDump(Session pXpoSession, string pFilename, string pCommandSeparator, Dictionary<string, string> pReplaceables) :: Error executing Sql Command: [{0}]{1}Exception: [{2}]", executeCommand, Environment.NewLine, ex.Message);
                            log.Error(string.Format("{0} : {1}", errorMessage, ex.Message), ex);
                            Utils.ShowMessageTouch(null, DialogFlags.Modal, new Size(800, 400), MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), errorMessage);

                            return false;
                        };
                    };
                }
            }

            return true;
        }

        /// <summary>
        /// Process all scripts in target directory
        /// </summary>
        /// <param name="Session"></param>
        /// <param name="DatabaseType"></param>
        /// <param name="CommandSeparator"></param>
        /// <param name="TargetDirectory"></param>
        /// <returns></returns>
        /// 
        public static bool ProcessDumpDirectory(Session pXpoSession, string pTargetDirectory, string pCommandSeparator, Dictionary<string, string> pReplaceables)
        {
            //Log4Net
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            //Start True
            bool result = true;
            string[] filesArray = null;

            //Ignores files if 
            List<string> ignoreFilesForDeveloper = new List<string>();
            if (Debugger.IsAttached == true)
            {
                //IgnoreClean Preference Parameter
                ignoreFilesForDeveloper.Add(@"Resources/Database/Other/configurationpreferenceparameter.sql");
            }

            try
            {
                if (Directory.Exists(pTargetDirectory))
                {
                    filesArray = Directory.GetFiles(pTargetDirectory, "*.sql");
                }

                //Process Files
                if (filesArray != null && filesArray.Length > 0)
                {
                    for (int i = 0; i < filesArray.Length; i++)
                    {
                        if (result)
                        {
                            //Ignore File if is in ignoreFilesForDeveloper List
                            if (!ignoreFilesForDeveloper.Contains(filesArray[i]))
                            {
                                result = ProcessDump(pXpoSession, FrameworkUtils.OSSlash(filesArray[i]), pCommandSeparator, pReplaceables);
                            }
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }

            return result;
        }

        /// <summary>
        /// Creates the SQL query for when retrieving linked documents.
        /// The parameter defines financial documents or payment documents.
        /// </summary>
        /// <param name="isPaymentDoc"></param>
        /// <returns></returns>
        public static string GenerateRelatedDocumentsQuery(bool isPaymentDoc = false)
        {
            string relatedDocumentsQuery = string.Empty;

            switch (GlobalFramework.DatabaseType)
            {
                case datalayer.Enums.DatabaseType.MySql:
                case datalayer.Enums.DatabaseType.SQLite:
                case datalayer.Enums.DatabaseType.MonoLite:
                    if (isPaymentDoc)
                    {
                        relatedDocumentsQuery = @"
SELECT 
    GROUP_CONCAT(DISTINCT DocFinMaster.DocumentNumber) AS ResultConcat
FROM 
	fin_documentfinancemasterpayment AS DocFinMasterPay
LEFT JOIN 
	fin_documentfinancemaster AS DocFinMaster 
        ON (DocFinMasterPay.DocumentFinanceMaster = DocFinMaster.Oid)
WHERE
    DocFinMasterPay.DocumentFinancePayment = '{0}'
GROUP BY
    DocFinMasterPay.DocumentFinancePayment;
";
                    }
                    else
                    {
                        /*  
                         SELECT 
    GROUP_CONCAT(DISTINCT DocumentNumber) AS ResultConcat
FROM
    view_documentfinancerelateddocumentlist
WHERE 
    DocumentParent = '{0}'
    OR
        DocumentChild = '{0}'
GROUP BY
    DocumentParent, DocumentChild
ORDER BY
    Date ASC;
                         
                         */
                        /* IN009157 */
                        relatedDocumentsQuery = @"
SELECT 
    GROUP_CONCAT(DISTINCT RelatedDocument.DocumentNumber) AS ResultConcat
FROM(
	SELECT
		DocFinMaster.DocumentNumber AS DocumentNumber,
		DocFinMaster.Date AS Date,
		DocFinMaster.DocumentParent AS DocumentParent,
		DocFinMaster.DocumentChild AS DocumentChild
	FROM
		fin_documentfinancemaster AS DocFinMaster
	WHERE
		DocFinMaster.DocumentStatusStatus <> 'A'
		AND (
			DocFinMaster.DocumentParent = '{0}'
			OR
				DocFinMaster.DocumentChild = '{0}'
				OR 
					DocFinMaster.Oid IN (
						SELECT 
							(SELECT B.Oid FROM fin_documentfinancemaster B WHERE B.Oid =  A.DocumentParent) AS DocumentParent
						FROM 
							fin_documentfinancemaster AS A
						WHERE
							A.Oid = '{0}'
					)
		)
	UNION
	SELECT
		DocFinPay.PaymentRefNo AS DocumentNumber,
		DocFinPay.DocumentDate AS Date,
		DocFinMasterPay.DocumentFinanceMaster AS DocumentParent,
		NULL AS DocumentChild
	FROM
		fin_documentfinancepayment AS DocFinPay
	LEFT JOIN fin_documentfinancemasterpayment DocFinMasterPay ON (DocFinPay.Oid = DocFinMasterPay.DocumentFinancePayment)
	WHERE
		DocFinPay.PaymentStatus <> 'A'
		AND
			DocFinMasterPay.DocumentFinanceMaster = '{0}'
) AS RelatedDocument;
";
                    }
                    break;
                case datalayer.Enums.DatabaseType.MSSqlServer:
                    if (isPaymentDoc)
                    {
                        relatedDocumentsQuery = @"
DECLARE @RelatedToPayDocuments VARCHAR(MAX);
SELECT
	@RelatedToPayDocuments = COALESCE(@RelatedToPayDocuments + ', ', '') + DocFinMaster.DocumentNumber
FROM 
	fin_documentfinancemasterpayment AS DocFinMasterPay
LEFT JOIN 
	fin_documentfinancemaster AS DocFinMaster 
        ON (DocFinMasterPay.DocumentFinanceMaster = DocFinMaster.Oid)
WHERE
    DocFinMasterPay.DocumentFinancePayment = '{0}'
ORDER BY
	DocFinMaster.Date ASC;
SELECT 
    @RelatedToPayDocuments;
";
                    }
                    else
                    {
                        /*
                                                relatedDocumentsQuery = @"
                        DECLARE @RelatedDocuments VARCHAR(MAX);
                        SELECT
                            @RelatedDocuments = COALESCE(@RelatedDocuments + ', ', '') + DocumentNumber
                        FROM
                            view_documentfinancerelateddocumentlist
                        WHERE
                            DocumentParent = '{0}'
                            OR
                                DocumentChild = '{0}'
                        ORDER BY 
                            Date ASC;
                        SELECT 
                            @RelatedDocuments;";
                        */
                        /* IN009157 - removing "view_documentfinancerelateddocumentlist" call and 
                         * implementing a new flow to retrieve all the children of a parent that has more than 1 child. 
                         * fin_documentfinancemaster.DocumentChild stores the last child only...
                         */
                        relatedDocumentsQuery = @"
DECLARE @RelatedDocuments VARCHAR(MAX);
SELECT
    @RelatedDocuments = COALESCE(@RelatedDocuments + ', ', '') + RelatedDocument.DocumentNumber
FROM(
	SELECT
		DocFinMaster.DocumentNumber AS DocumentNumber,
		DocFinMaster.Date AS Date,
		DocFinMaster.DocumentParent AS DocumentParent,
		DocFinMaster.DocumentChild AS DocumentChild
	FROM
		fin_documentfinancemaster AS DocFinMaster
	WHERE
		DocFinMaster.DocumentStatusStatus <> 'A'
		AND (
			DocFinMaster.DocumentParent = '{0}'
			OR
				DocFinMaster.DocumentChild = '{0}'
				OR 
					DocFinMaster.Oid IN (
						SELECT 
							(SELECT B.Oid FROM fin_documentfinancemaster B WHERE B.Oid =  A.DocumentParent) AS DocumentParent
						FROM 
							fin_documentfinancemaster AS A
						WHERE
							A.Oid = '{0}'
					)
		)
	UNION
	SELECT
		DocFinPay.PaymentRefNo AS DocumentNumber,
		DocFinPay.DocumentDate AS Date,
		DocFinMasterPay.DocumentFinanceMaster AS DocumentParent,
		NULL AS DocumentChild
	FROM
		fin_documentfinancepayment AS DocFinPay
	LEFT JOIN fin_documentfinancemasterpayment DocFinMasterPay ON (DocFinPay.Oid = DocFinMasterPay.DocumentFinancePayment)
	WHERE
		DocFinPay.PaymentStatus <> 'A'
		AND
			DocFinMasterPay.DocumentFinanceMaster = '{0}'
) AS RelatedDocument

ORDER BY 
    RelatedDocument.Date ASC;
SELECT 
    @RelatedDocuments;
";
                    }
                    break;
                default:
                    break;
            }
            return relatedDocumentsQuery;
        }
    }
}
