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

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //User Config Oids - Values in Config, Proxy here to always use SettingsApp.VAR in Code

       
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // Other

        //Privileges {table} Prefix
        public static string PrivilegesBackOfficeCRUDOperationPrefix = "BACKOFFICE_MAN_{0}";
        public static string PrivilegesBackOfficeMenuOperationFormat = string.Format("{0}_{1}", PrivilegesBackOfficeCRUDOperationPrefix, "MENU");
        public static string PrivilegesReportDialogFormat = "{0}";

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

        private static string GetThemeFileLocation()
        {

            var themeLocation = $"{PathsSettings.Paths["themes"]}{string.Format(FileFormatThemeFile, "default", "default")}";
            return themeLocation;
        }

    
    }
}
