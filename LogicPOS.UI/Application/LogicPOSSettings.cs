using LogicPOS.Settings;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Application
{
    public static class LogicPOSSettings
    {

#if (DEBUG)
        //Used to Force/Override Intellilock assigned GlobalFramework.LicenceRegistered in BootStrap
        public static bool LicenceRegistered = true;
        //Valid databaseType Values: SQLite, MySql, MSSqlServer (DBName Must be lowercase)
        public static string DatabaseName = "logicposdbdevelopment";
        //Used to Force create DatabaseSchema and Fixtures with XPO (Non Script Mode): Requirements for Work: Empty or Non Exist Database
        //Notes: OnError "An exception of type 'DevExpress.Xpo.DB.Exceptions.SchemaCorrectionNeededException'", UnCheck [X] Break when this exception is user-unhandled and continue, watch log and wait until sucefull message appear
        public static bool XPOCreateDatabaseAndSchema = false;
        //Use file based Protected Files
        public static bool ProtectedFilesUse = false;
        //Used to Recreate Protection File, based on ProtectedFilesList | the files must be in bin folder, else dont be generated (File.Exist)
        public static bool ProtectedFilesRecreateCSV = false;
        //Used to develop reports or tickets, to permit changes in debug mode, ex design reports without check permissions
        public static bool ProtectedFilesIgnoreProtection = false;
        //Used to Work in DEBUG Mode and Fake Hardware ID, Must be UNIQUE for Terminals
        //TIP: Must be changed if Work in Multi User Database Environments, else all Terminals act like there is only one Terminal
        //Disable > public static string AppHardwareId = null;
        public static string AppHardwareId = "92A4-EADD-8AF0-B693-DBD0-2A22";
#else
        public static bool LicenceRegistered = false;
        public static string DatabaseName = "logicposdb";
        public static bool ProtectedFilesIgnoreProtection = true;
        public static bool XPOCreateDatabaseAndSchema = false;
        public static bool UseProtectedFiles = false;
        public static bool ProtectedFilesRecreateCSV = false;
        public static string AppHardwareId = string.Empty;
#endif

        public static string AppName = "LogicPos";
        public static string AppIcon = "application.ico";
        public static string AppUrl = "www.logicpulse.com";

        public static string LicenceFileName = "licence.lic";

        //Executables
        public static string ExecutableComposer = "logicpos.composer.exe";
        public static string ExecutableReports = "logicpos.reports.exe";

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Override Settings

        public static bool EnablePosSessionApp = true;
        public static bool EnablePosWorkSessionPeriod = true;
        public static bool EnablePosTables = true;

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //ProtectedFiles

        //Protected Files Settings
        public static string ProtectedFilesFileName = "logicpos.exe.files";
        public static List<string> ProtectedFilesList { get { return GetProtectedFilesList(); } }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //DataBase Backup System

        /* ERR201810#15 - Database backup issues */
        public static uint BackupTimerInterval = 1000 * 60;
        public static string BackupExtension = "bak";

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Formats

        //Dont change This, Required to use . in many things like SAF-T etc
        public static string CultureNumberFormat = "en-US";

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Files/File Formats

        //DataBase Backup File Format : {databasetype}_{databasename}_{version}_{date}.{extension}
        public static string FileFormatDataBaseBackup = "{0}_{1}_{2}_{3}.{4}";

        //Theme File Format : ex.: {theme}_{default}_{default}_{1024}x{768}.xml (LOWERCASE) - {0}: appTheme, {1}: appOperationModeToken, {2}: Width, {3}: Height
        //private static string FileFormatThemeFile = "theme_{0}_{1}_{2}x{3}.xml";//Deprecated
        private static readonly string FileFormatThemeFile = "theme_{0}_{1}.xml";
        public static string FileTheme { get { return GetThemeFileLocation(); } }

        //Database Script Files
        public static string FileDatabaseSchema = @"Resources\Database\{0}\databaseschema.sql";

        public static string FileDatabaseUpdate = @"Resources\Database\{0}\databaseschemaupdate.sql";

        /* IN009035 */
        public static string FileDatabaseDataPath = @"Resources\Database\Data\{0}\{1}\databasedata.sql"; // "Resources\Database\Data\Default\en\databasedata.sql"
        public static string FileDatabaseData = GetDatabaseFileName(false, FileDatabaseDataPath);// Default Script: "databasedata.sql";
        /* IN008024 */
        public static string FileDatabaseDataDemoPath = @"Resources\Database\Demos\{0}\{1}\{2}"; // "Resources\Database\Demos\Backery\en\databasedatademo_backery.sql"
        public static string FileDatabaseDataDemo = GetDatabaseFileName(true, FileDatabaseDataDemoPath);// Default Script: "databasedatademo.sql";

        public static string FileDatabaseViews = @"Resources\Database\databaseviews.sql";
        // public static string FileDatabaseOtherDatabaseType = @"Resources\Database\{0}\Other\";/* IN009045: Not in use */
        public static string FileDatabaseOtherCommon = @"Resources\Database\Other\";
        /* IN009035: data being included by databasedata.sql accordingly to its specific theme/language */
        // public static string FileDatabaseOtherCommonAppMode = @"Resources\Database\Other\AppMode";
        //Encrypted Scripts
        /* IN009035 */
        public static string FileDatabaseOtherCommonPluginsSoftwareVendorPath = @"Resources\Database\Other\Plugins\SoftwareVendor\Data\{0}\{1}"; // "Resources\Database\Other\Plugins\SoftwareVendor\Data\Default\en"
        public static string FileDatabaseOtherCommonPluginsSoftwareVendor = GetDatabaseFileName(false, FileDatabaseOtherCommonPluginsSoftwareVendorPath);// Default Path: "Resources\Database\Other\Plugins\SoftwareVendor"

        //Country Scripts
        public static string FileDatabaseOtherCommonCountry = @"Resources\Database\Other\Country";
        //Country Encrypted Scripts
        public static string FileDatabaseOtherCommonPluginsSoftwareVendorOtherCommonCountry = @"Resources\Database\Other\Plugins\SoftwareVendor\Other\Country";

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //User Config Oids - Values in Config, Proxy here to always use SettingsApp.VAR in Code

        //Database Oids/Guids

        //Payment Defaults
        public static Guid XpoOidConfigurationPaymentConditionDefaultInvoicePaymentCondition { get; set; } = new Guid(AppSettings.Instance.xpoOidConfigurationPaymentConditionDefaultInvoicePaymentCondition);
        public static Guid XpoOidConfigurationPaymentMethodDefaultInvoicePaymentMethod = new Guid(AppSettings.Instance.xpoOidConfigurationPaymentMethodDefaultInvoicePaymentMethod);

        //ConfigurationPlaceTable
        public static Guid XpoOidConfigurationPlaceTableDefaultOpenTable = new Guid(AppSettings.Instance.xpoOidConfigurationPlaceTableDefaultOpenTable);

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // Other

        //Privileges {table} Prefix
        public static string PrivilegesBackOfficeCRUDOperationPrefix = "BACKOFFICE_MAN_{0}";
        public static string PrivilegesBackOfficeMenuOperationFormat = string.Format("{0}_{1}", PrivilegesBackOfficeCRUDOperationPrefix, "MENU");
        public static string PrivilegesReportDialogFormat = "{0}";

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //AT Web Services

        public static bool ServiceATSendDocuments { get { return GetServiceATSendDocuments(); } }
        public static bool ServiceATSendDocumentsWayBill { get { return GetServiceATSendDocumentsWayBill(); } }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //BackOffice

        //BackOffice Insert Objects start Disabled
        public static bool BOXPOObjectsStartDisabled = false;

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Payments Window

        //Use CurrentAccount or CustomerCard in PaymentsDialog
        public static bool PosPaymentsDialogUseCurrentAccount = AppSettings.Instance.posPaymentsDialogUseCurrentAccount;

        //First time boot POS flag
        public static bool FirstBoot;
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Pagination Related
        /// <summary>
        /// Define Rows per Page for pagination load
        /// </summary>
        public static int PaginationRowsPerPage = 30;

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Helper Methods

        private static List<string> GetProtectedFilesList()
        {
            List<string> result = new List<string>
            {
                @"Resources\Reports\UserReports\ReportArticle.frx",
                @"Resources\Reports\UserReports\ReportDocumentFinance.frx",
                @"Resources\Reports\UserReports\ReportDocumentFinancePayment.frx",
                @"Resources\Reports\UserReports\ReportDocumentFinanceWayBill.frx",
                @"Resources\Reports\UserReports\ReportTest.frx"
            };

            return result;
        }

        private static string GetThemeFileLocation()
        {

            CustomAppOperationMode customAppOperationMode = AppOperationModeSettings.CustomAppOperationMode;

            var themeLocation = $"{PathsSettings.Paths["themes"]}{string.Format(FileFormatThemeFile, GeneralSettings.AppTheme.ToLower(), customAppOperationMode.AppOperationTheme.ToLower())}";
            return themeLocation;
        }

        private static bool GetServiceATSendDocuments()
        {
            return Convert.ToBoolean(GeneralSettings.PreferenceParameters["SERVICE_AT_SEND_DOCUMENTS"]);
        }

        private static bool GetServiceATSendDocumentsWayBill()
        {
            return Convert.ToBoolean(GeneralSettings.PreferenceParameters["SERVICE_AT_SEND_DOCUMENTS_WAYBILL"]);
        }

        /// <summary>
        /// This method is responsible for loading the proper Demo database script for selected user's option during POS installation.
        /// For further details, please see #IN008024# and #IN009035#.
        /// </summary>
        /// <returns></returns>
        private static string GetDatabaseFileName(bool demo, string basePath)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            string result;

            /* Custom scripts */
            try
            {
                CustomAppOperationMode customAppOperationMode = CustomAppOperationMode.GetAppOperationMode(AppSettings.Instance.appOperationModeToken);

                string cultureName = CultureSettings.CurrentCultureName;
                string cultureCountryPrefix = cultureName.Substring(0, cultureName.IndexOf('-'));

                if (demo)
                {
                    string appOperationModeToken = customAppOperationMode.AppOperationModeToken;
                    string fileName = customAppOperationMode.DatabaseDemoFileName;

                    result = string.Format(basePath,
                        appOperationModeToken,
                        cultureCountryPrefix,
                        fileName
                    );
                }
                else
                {
                    //Angola - Certificação [TK:016268]
                    if (cultureName == "pt-AO" && basePath == "Resources\\Database\\Data\\{0}\\{1}\\databasedata.sql")
                    {
                        cultureCountryPrefix = "ao";
                    }
                    /* Default or Retail */
                    string appOperationTheme = customAppOperationMode.AppOperationTheme;
                    //Utiliza SQL para BackOfficeMode
                    if (AppOperationModeSettings.CustomAppOperationMode.AppOperationModeToken == "BackOfficeMode")
                    {
                        appOperationTheme = "BackOfficeMode";
                    }
                    // "Resources\Database\Data\{0}\{1}\databasedata.sql"
                    // "..\Resources\Database\Other\Plugins\SoftwareVendor\Data\{0}\{1}"
                    result = string.Format(basePath,
                        appOperationTheme,
                        cultureCountryPrefix
                    );
                }
            }
            catch (Exception ex)
            {
                log.Error("string GetDatabaseFileName(bool demo) :: " + ex.Message, ex);
                /* Default script for demo or data */
                result = demo ? @"Resources\Database\databasedatademo.sql" : basePath.EndsWith(".sql") ? @"Resources\Database\databasedata.sql" : @"Resources\Database\Other\Plugins\SoftwareVendor";
            }

            return result;
        }
    }
}
