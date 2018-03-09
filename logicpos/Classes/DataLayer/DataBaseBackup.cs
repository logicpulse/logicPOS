using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Gtk;
using logicpos.App;
using logicpos.Classes.Enums;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.financial.library.Classes.Finance;
using logicpos.resources.Resources.Localization;
using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.IO;
using System.Threading;

namespace logicpos.Classes.DataLayer
{
    class DataBaseBackupFileInfo
    {
        private string _fileName = string.Empty;
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }
        private string _fileNamePacked = string.Empty;
        public string FileNamePacked
        {
            get { return _fileNamePacked; }
            set { _fileNamePacked = value; }
        }
        private string _fileHashDB = string.Empty;
        public string FileHashDB
        {
            get { return _fileHashDB; }
            set { _fileHashDB = value; _FileHashValid = (_fileHashDB == _fileHashFile); }
        }
        private string _fileHashFile = string.Empty;
        public string FileHashFile
        {
            get { return _fileHashFile; }
            set { _fileHashFile = value; _FileHashValid = (_fileHashDB == _fileHashFile); }
        }
        private bool _FileHashValid = false;
        public bool FileHashValid
        {
            get { return _FileHashValid; }
            set { _FileHashValid = value; }
        }
        private ResponseType _response = ResponseType.Cancel;
        public ResponseType Response
        {
            get { return _response; }
            set { _response = value; }
        }

        public DataBaseBackupFileInfo() { }
        public DataBaseBackupFileInfo(string pFileName, string pFileNamePacked, string pFileHashDB, string pFileHashFile, bool pFileHashValid)
        {
            _fileName = pFileName;
            _fileNamePacked = pFileNamePacked;
            _fileHashDB = pFileHashDB;
            _fileHashFile = pFileHashFile;
            _FileHashValid = pFileHashValid;
        }
    }

    class DataBaseBackup
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //Enable Debug
        private static bool _debug = false;
        //Settings
        private static string _backupConnectionString;
        private static string _fileExtension;
        private static string _pathBackups = GlobalFramework.Path["backups"].ToString();
        //Private Vars
        private static readonly Size _sizeDialog = new Size(800, 300);

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Shared for All DataBaseTypes
        public static void Init()
        {
            switch (GlobalFramework.DatabaseType)
            {
                case DatabaseType.SQLite:
                case DatabaseType.MonoLite:
                    _fileExtension = "db";
                    break;
                case DatabaseType.MSSqlServer:
                    _fileExtension = "bak";
                    _backupConnectionString = (string.Format(
                      "Data Source={0};Initial Catalog={1};User ID={2};Password={3};Persist Security Info=true;Integrated Security=SSPI;Pooling=false"
                      , GlobalFramework.DatabaseServer
                      , GlobalFramework.DatabaseName
                      , GlobalFramework.DatabaseUser
                      , GlobalFramework.DatabasePassword
                    ));
                    break;
                case DatabaseType.MySql:
                    _fileExtension = "sql";
                    _backupConnectionString = string.Format(
                        "server={0};database={1};user={2};pwd={3};"
                        , GlobalFramework.DatabaseServer
                        , GlobalFramework.DatabaseName
                        , GlobalFramework.DatabaseUser
                        , GlobalFramework.DatabasePassword
                      );
                    break;
                default:
                    break;
            }
        }

        public static bool Backup()
        {
            return Backup(null);
        }

        public static bool Backup(Window pSourceWindow)
        {
            bool backupResult = true;
            string fileName = string.Empty;
            string fullFileNamePacked = string.Empty;
            string fileHash = string.Empty;
            Thread thread;

            try
            {
                //FrameworkUtils.ShowWaitingCursor();

                Init();

                //Initialize object before start Actions, to allocate database (automatic backups) and assign CreatedAt, this way next Terminal Skip Backup when trigger backup event
                SYS_SystemBackup systemBackup = new SYS_SystemBackup(GlobalFramework.SessionXpo)
                {
                    FileName = Path.GetRandomFileName(),
                    FileNamePacked = Path.GetRandomFileName(),
                    DataBaseType = GlobalFramework.DatabaseType,
                    Version = FrameworkUtils.GetNextTableFieldID("SYS_SystemBackup", "Version", false),
                    Terminal = (POS_ConfigurationPlaceTerminal)GlobalFramework.SessionXpo.GetObjectByKey(typeof(POS_ConfigurationPlaceTerminal), GlobalFramework.LoggedTerminal.Oid)
                };
                systemBackup.Save();

                switch (GlobalFramework.DatabaseType)
                {
                    case DatabaseType.MonoLite:
                    case DatabaseType.SQLite:
                        fileName = GetBackupFileName(_fileExtension, systemBackup.Version);
                        //Non Thread
                        //resultBackup = BackupSQLite(fileName);
                        //Thread
                        thread = new Thread(() => backupResult = BackupSQLite(fileName));
                        Utils.ThreadStart(pSourceWindow, thread);
                        break;
                    case DatabaseType.MSSqlServer:
                        fileName = GetBackupFileName(_fileExtension, systemBackup.Version);
                        //Non Thread
                        //resultBackup = BackupMSSqlServer(fileName);
                        //Thread
                        thread = new Thread(() => backupResult = BackupMSSqlServer(fileName));
                        Utils.ThreadStart(pSourceWindow, thread);
                        break;
                    case DatabaseType.MySql:
                        fileName = GetBackupFileName(_fileExtension, systemBackup.Version);
                        //Non Thread
                        //resultBackup = BackupMySql(_backupConnectionString, fileName);
                        //Thread
                        thread = new Thread(() => backupResult = BackupMySql(_backupConnectionString, fileName));
                        Utils.ThreadStart(pSourceWindow, thread);
                        break;
                    default:
                        break;
                }

                if (_debug) _log.Debug(string.Format("Backup DatabaseType: [{0}] to FileName: [{1}], resultBackup:[{2}]", GlobalFramework.DatabaseType, fileName, backupResult));

                if (backupResult)
                {
                    //Update SystemBackup after Backup
                    systemBackup.FileName = Path.GetFileName(fileName);
                    systemBackup.FilePath = Path.GetDirectoryName(fileName);

                    //Extra Protection for System Automatic Backups, with unlogged users
                    SYS_UserDetail userDetail = (GlobalFramework.LoggedUser != null) ? (SYS_UserDetail)GlobalFramework.SessionXpo.GetObjectByKey(typeof(SYS_UserDetail), GlobalFramework.LoggedUser.Oid) : null;
                    if (userDetail != null) systemBackup.User = userDetail;

                    //Non MSSqlServer Work: Cant Get Remote File Sizes, Hash etc from LPDev Backups
                    if (GlobalFramework.DatabaseType != DatabaseType.MSSqlServer)
                    {
                        //systemBackup.FileSize = new FileInfo(fileName).Length;
                        systemBackup.FileNamePacked = Path.ChangeExtension(systemBackup.FileName, SettingsApp.BackupExtension);
                        //Compress File : Required OSSlash
                        fullFileNamePacked = FrameworkUtils.OSSlash(string.Format(@"{0}\{1}", systemBackup.FilePath, systemBackup.FileNamePacked));
                        // Old Method before PluginSoftwareVendor Implementation
                        //backupResult = Utils.ZipPack(new string[] { fileName }, fullFileNamePacked);
                        backupResult = GlobalFramework.PluginSoftwareVendor.BackupDatabase(SettingsApp.SecretKey, new string[] { fileName }, fullFileNamePacked);
                        // Add FileHash
                        if (backupResult) systemBackup.FileHash = FrameworkUtils.MD5HashFile(fullFileNamePacked);
                    }
                    //MSSqlServer
                    else
                    {
                        //Must Assign FileName to FileNamePacked else next backup violate iFileNamePacked_SystemBackup Index
                        systemBackup.FileNamePacked = systemBackup.FileName;
                    }

                    if (backupResult)
                    {
                        //Commit Object
                        systemBackup.Save();
                        //Remove Temporary Backup
                        try
                        {
                            if (File.Exists(fileName)) {
                                File.Delete(fileName);
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.Error(ex.Message, ex);
                        }

                        //Post Backup
                        FrameworkUtils.Audit("DATABASE_BACKUP", string.Format(Resx.audit_message_database_backup,
                            (fullFileNamePacked != string.Empty) ? fullFileNamePacked : systemBackup.FileNamePacked
                        ));

                        //Moved to Thread Outside > Only Show if not in Silence Mode
                        if (pSourceWindow != null) Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, _sizeDialog, MessageType.Info, ButtonsType.Close, Resx.global_information, string.Format(Resx.dialog_message_database_backup_successfully, fullFileNamePacked));
                    }
                    else
                    {
                        //On Error Delete Object form Database, this way we dont have a invalid Backup
                        systemBackup.Delete();
                    }
                }
                else
                {
                    //Moved to Thread Outside > Only Show if not in Silence Mode
                    if (pSourceWindow != null) Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, _sizeDialog, MessageType.Error, ButtonsType.Close, Resx.global_error, string.Format(Resx.dialog_message_database_backup_error, fullFileNamePacked));
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
            finally
            {
                //FrameworkUtils.HideWaitingCursor();
            }

            return backupResult;
        }

        public static bool Restore(Window pSourceWindow, DataBaseRestoreFrom pRestoreFrom)
        {
            try
            {
                //FrameworkUtils.ShowWaitingCursor();

                Init();

                bool restoreResult = false;
                string sql = string.Empty;
                string fileName = string.Empty;
                string fileNamePacked = string.Empty;
                //default pathBackups from Settings, can be Overrided in ChooseFromFilePickerDialog Mode
                string pathBackups = _pathBackups;
                DataBaseBackupFileInfo fileInfo = null;
                Guid systemBackupGuid = Guid.Empty;
                //Required to assign current FileName and FileNamePacked after restore, else name will be the TempName ex acegvpls.soj & n2sjiamk.32o
                SYS_SystemBackup systemBackup = null;
                string currentFileName = string.Empty, currentFileNamePacked = string.Empty, currentFilePath = string.Empty, currentFileHash = string.Empty;
                SYS_UserDetail currentUserDetail = null;

                switch (pRestoreFrom)
                {
                    case DataBaseRestoreFrom.SystemBackup:
                        fileInfo = GetSelectRecordFileName(pSourceWindow);
                        //Equal to Filename not FileNamePacked
                        fileNamePacked = fileInfo.FileNamePacked;
                        if (_debug) _log.Debug(string.Format("RestoreBackup: FileNamePacked:[{0}], FileHashDB:[{1}], FileHashFile:[{2}] FileHashValid:[{3}]", fileInfo.FileNamePacked, fileInfo.FileHashDB, fileInfo.FileHashFile, fileInfo.FileHashValid));
                        if (fileInfo.Response != ResponseType.Cancel && !fileInfo.FileHashValid)
                        {
                            //#EQUAL#1
                            string message = string.Format(Resx.dialog_message_database_restore_error_invalid_backup_file, fileNamePacked);
                            Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, new Size(600, 300), MessageType.Error, ButtonsType.Ok, Resx.global_error, message);
                            return false;
                        }
                        break;
                    case DataBaseRestoreFrom.ChooseFromFilePickerDialog:
                        FileFilter fileFilterBackups = Utils.GetFileFilterBackups();
                        PosFilePickerDialog dialog = new PosFilePickerDialog(pSourceWindow, DialogFlags.DestroyWithParent, fileFilterBackups, FileChooserAction.Open);
                        ResponseType response = (ResponseType)dialog.Run();
                        if (response == ResponseType.Ok)
                        {
                            fileNamePacked = dialog.FilePicker.Filename;
                            //Override Default pathBackups
                            pathBackups = string.Format("{0}/", Path.GetDirectoryName(fileNamePacked));
                        }
                        dialog.Destroy();
                        break;
                    default:
                        break;
                }

                if (GlobalFramework.DatabaseType != DatabaseType.MSSqlServer)
                {
                    fileName = Path.ChangeExtension(fileNamePacked, _fileExtension);
                }
                else
                {
                    //Require to assign filename and packed filename from fileInfo
                    fileName = fileInfo.FileName;
                    fileNamePacked = fileInfo.FileName;
                }

                if (fileName != string.Empty)
                {
                    if (_debug) _log.Debug(string.Format("Restore Filename:[{0}] to pathBackups[{1}]", fileNamePacked, pathBackups));
                    if (GlobalFramework.DatabaseType != DatabaseType.MSSqlServer)
                    {
                        // Old Method before PluginSoftwareVendor Implementation
                        //restoreResult = Utils.ZipUnPack(fileNamePacked, pathBackups, true);
                        restoreResult = GlobalFramework.PluginSoftwareVendor.RestoreBackup(SettingsApp.SecretKey, fileNamePacked, pathBackups, true);
                        if (_debug) _log.Debug(string.Format("RestoreBackup: unpackResult:[{0}]", restoreResult));
                    }

                    if (restoreResult || GlobalFramework.DatabaseType == DatabaseType.MSSqlServer)
                    {
                        //Get properties from SystemBackup Object before Restore, to Assign Properties after Restore (FilePath,FileHash,User,Terminal)
                        sql = string.Format("SELECT Oid FROM sys_systembackup WHERE fileNamePacked = '{0}';", Path.GetFileName(fileNamePacked));
                        systemBackupGuid = FrameworkUtils.GetGuidFromQuery(sql);
                        if (systemBackupGuid != Guid.Empty)
                        {
                            systemBackup = (SYS_SystemBackup)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(SYS_SystemBackup), systemBackupGuid);
                            currentFileName = systemBackup.FileName;
                            currentFileNamePacked = systemBackup.FileNamePacked;
                            currentFilePath = systemBackup.FilePath;
                            currentFileHash = systemBackup.FileHash;
                            currentUserDetail = systemBackup.User;
                        }

                        //Send fileNamePacked only to show its name in success dialog
                        if (Restore(pSourceWindow, fileName, fileNamePacked, systemBackup))
                        {
                            //Audit DATABASE_RESTORE
                            FrameworkUtils.Audit("DATABASE_RESTORE", string.Format(Resx.audit_message_database_restore, fileNamePacked));
                            //Required to DropIdentity before get currentDocumentFinanceYear Object, else it exists in old non restored Session
                            GlobalFramework.SessionXpo.DropIdentityMap();
                            //Get Current Active FinanceYear
                            FIN_DocumentFinanceYears currentDocumentFinanceYear = ProcessFinanceDocumentSeries.GetCurrentDocumentFinanceYear();

                            //Disable all Active FinanceYear Series and SeriesTerminal if Exists
                            if (currentDocumentFinanceYear != null) ProcessFinanceDocumentSeries.DisableActiveYearSeriesAndTerminalSeries(currentDocumentFinanceYear);

                            //Restore SystemBackup properties else it keeps temp names after Restore acegvpls.soj & n2sjiamk.32o, empty hash etc
                            if (systemBackupGuid != Guid.Empty)
                            {
                                //ReFresh UserDetail from Repository
                                currentUserDetail = (currentUserDetail != null) ? (SYS_UserDetail)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(SYS_UserDetail), currentUserDetail.Oid) : null;
                                //Get Current Restored systemBackup Object
                                systemBackup = (SYS_SystemBackup)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(SYS_SystemBackup), systemBackupGuid);
                                systemBackup.FileName = currentFileName;
                                systemBackup.FileNamePacked = currentFileNamePacked;
                                systemBackup.FilePath = currentFilePath;
                                systemBackup.FileHash = currentFileHash;
                                systemBackup.User = currentUserDetail;
                                systemBackup.Save();
                            }
                            //If Cant get Record, because works on ChooseFromFilePickerDialog, we must recreate Record from file, only in the case of record with miss fileNamePacked 
                            else
                            {
                                sql = "DELETE FROM sys_systembackup WHERE FilePath IS NULL AND FileHash IS NULL AND User IS NULL;";
                                GlobalFramework.SessionXpo.ExecuteNonQuery(sql);
                            }

                            //Audit
                            FrameworkUtils.Audit("APP_CLOSE");
                            //Call QuitWithoutConfirmation without Audit
                            LogicPos.QuitWithoutConfirmation(false);

                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        //#EQUAL#1
                        string message = string.Format(Resx.dialog_message_database_restore_error_invalid_backup_file, fileNamePacked);
                        Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, new Size(600, 300), MessageType.Error, ButtonsType.Ok, Resx.global_error, message);
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return false;
            }
            finally
            {
                //FrameworkUtils.HideWaitingCursor();
            }
        }

        public static bool Restore(Window pSourceWindow, string pFileName, string pFileNamePacked, SYS_SystemBackup pSystemBackup)
        {
            Thread thread;
            bool resultRestore = false;

            switch (GlobalFramework.DatabaseType)
            {
                case DatabaseType.MonoLite:
                case DatabaseType.SQLite:
                    //Non Thread
                    //resultRestore = RestoreSQLite(pFileName);
                    //Thread
                    thread = new Thread(() => resultRestore = RestoreSQLite(pFileName));
                    Utils.ThreadStart(pSourceWindow, thread);
                    break;
                case DatabaseType.MSSqlServer:
                    //Non Thread
                    //resultRestore = RestoreMSSqlServer(_backupConnectionString, pFileName);
                    thread = new Thread(() => resultRestore = RestoreMSSqlServer(_backupConnectionString, pFileName));
                    Utils.ThreadStart(pSourceWindow, thread);
                    break;
                case DatabaseType.MySql:
                    //Non Thread
                    //resultRestore = RestoreMySql(_backupConnectionString, pFileName);
                    thread = new Thread(() => resultRestore = RestoreMySql(_backupConnectionString, pFileName));
                    Utils.ThreadStart(pSourceWindow, thread);
                    break;
                default:
                    break;
            }

            //Always Remove Extracted File before Show Dialog. Prevent user to get it
            if (File.Exists(pFileName)) File.Delete(pFileName);

            if (resultRestore)
            {
                Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, _sizeDialog, MessageType.Info, ButtonsType.Close, Resx.global_information, string.Format(Resx.dialog_message_database_restore_successfully, pFileNamePacked));
            }
            else
            {
                Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, _sizeDialog, MessageType.Error, ButtonsType.Close, Resx.global_error, string.Format(Resx.dialog_message_database_restore_error, pFileNamePacked));
            }

            return resultRestore;
        }

        private static string GetBackupFileName(string pFileExtension, uint pFileVersion)
        {
            //Settings
            string pathBackups = GlobalFramework.Path["backups"].ToString();
            string fileDataBaseBackup = SettingsApp.FileFormatDataBaseBackup;
            string dateTimeFileFormat = SettingsApp.FileFormatDateTime;
            //Local Vars
            string dateTime = FrameworkUtils.CurrentDateTimeAtomic().ToString(dateTimeFileFormat);

            //Override Default pathBackups
            if (GlobalFramework.DatabaseType == DatabaseType.MSSqlServer) pathBackups = string.Empty;

            /*
            OLD CODE, NOT USED ANYMORE
            #if MONOLINUX
            #else
            //Override default LocalPath with MSSqlServer.BackupDirectory
            if (GlobalFramework.DatabaseType == "MSSqlServer")
            {
              _backupConnectionString = string.Format(GlobalFramework.Settings["backupConnectionString"], GlobalFramework.DatabaseServer);
              ServerConnection connection = new ServerConnection(_backupConnectionString);
              Server server = new Server(connection);

              //SqlServer Edition
              if (GlobalFramework.DatabaseServer.ToUpper() != @".\SQLEXPRESS".ToUpper())
              {
                try
                {
                  //Using SQL Server authentication
                  server.ConnectionContext.LoginSecure = false;
                  server.ConnectionContext.Login = GlobalFramework.DatabaseUser;
                  server.ConnectionContext.Password = GlobalFramework.DatabasePassword;
                }
                catch (Exception ex)
                {
                  _log.Error(ex.Message, ex);
                }
              }

              pathBackups = string.Format(@"{0}\", server.BackupDirectory);
              if (server.ConnectionContext.IsOpen) server.ConnectionContext.Disconnect();
            }
            #endif
            */

            return pathBackups + string.Format(fileDataBaseBackup, GlobalFramework.DatabaseType, GlobalFramework.DatabaseName, pFileVersion, dateTime, pFileExtension).ToLower();
        }

        private static string GetLatestValidBackupFileName()
        {
            string fileName = string.Empty;
            string sql = @"SELECT FileName FROM sys_systembackup ORDER BY Version DESC;";
            SelectedData xpoSelectedDataSystemBackup = GlobalFramework.SessionXpo.ExecuteQuery(sql);

            foreach (SelectStatementResultRow item in xpoSelectedDataSystemBackup.ResultSet[0].Rows)
            {
                fileName = item.Values[0].ToString();

                if (File.Exists(fileName))
                {
                    return fileName;
                }
            }
            return string.Empty;
        }

        private static DataBaseBackupFileInfo GetSelectRecordFileName(Window pSourceWindow)
        {
            DataBaseBackupFileInfo resultFileInfo = new DataBaseBackupFileInfo();

            try
            {
                CriteriaOperator criteriaOperator = CriteriaOperator.Parse(string.Format("DataBaseType = '{0}' && FileName IS NOT NULL", GlobalFramework.DatabaseType));

                PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewSystemBackup>
                  dialogSystemBackup = new PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewSystemBackup>(
                    pSourceWindow,
                    Gtk.DialogFlags.DestroyWithParent,
                    Resx.window_title_select_backup_filename,
                    new Size(780, 580),
                    null, //XpoDefaultValue
                    criteriaOperator,
                    GenericTreeViewMode.Default,
                    null  //ActionAreaButtons
                  );

                ResponseType response = (ResponseType)dialogSystemBackup.Run();
                if (response == ResponseType.Ok)
                {
                    //Assign Result
                    resultFileInfo.Response = response;

                    SYS_SystemBackup systemBackup = (SYS_SystemBackup)dialogSystemBackup.GenericTreeView.DataSourceRow;
                    if (systemBackup != null)
                    {
                        if (GlobalFramework.DatabaseType == DatabaseType.MSSqlServer)
                        {
                            resultFileInfo.FileName = systemBackup.FileName;
                            resultFileInfo.FileHashValid = true;
                        }
                        else
                        {
                            resultFileInfo.FileName = FrameworkUtils.OSSlash(string.Format(@"{0}{1}", _pathBackups, systemBackup.FileName));
                            resultFileInfo.FileNamePacked = FrameworkUtils.OSSlash(string.Format(@"{0}{1}", _pathBackups, systemBackup.FileNamePacked));
                            resultFileInfo.FileHashDB = systemBackup.FileHash;
                            resultFileInfo.FileHashFile = FrameworkUtils.MD5HashFile(resultFileInfo.FileNamePacked);
                        }
                    }
                }
                dialogSystemBackup.Destroy();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return resultFileInfo;
        }

        public static void ShowRequestBackupDialog(Window pSourceWindow)
        {
            ResponseType responseType = Utils.ShowMessageTouch(
              pSourceWindow,
              DialogFlags.Modal,
              MessageType.Question,
              ButtonsType.YesNo,
              Resx.global_information,
              Resx.dialog_message_request_backup
            );

            if (responseType == ResponseType.Yes)
            {
                Backup(pSourceWindow);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //SQLite
        private static bool BackupSQLite(string pFileName)
        {
            string filenameSource = string.Format("{0}.db", GlobalFramework.DatabaseName);
            string filenameTarget = pFileName;
            if (_debug) _log.Debug(string.Format("BackupSQLite filenameSource: [{0}] to filenameSource: [{1}]", filenameSource, filenameTarget));

            try
            {
                File.Copy(filenameSource, filenameTarget);
                return true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return false;
            }
            finally
            {
                //Notify WakeupMain and Call ReadyEvent
                GlobalApp.DialogThreadNotify.WakeupMain();
            }
        }

        private static bool RestoreSQLite(string pFileName)
        {
            string filenameSource = pFileName;
            string filenameTarget = string.Format("{0}.db", GlobalFramework.DatabaseName);
            if (_debug) _log.Debug(string.Format("BackupSQLite filenameSource: [{0}] to filenameSource: [{1}]", filenameSource, filenameTarget));

            try
            {
                //Disconnect From Database
                GlobalFramework.SessionXpo.Disconnect();
                File.Copy(filenameSource, filenameTarget, true);
                return true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return false;
            }
            finally
            {
                //Notify WakeupMain and Call ReadyEvent
                GlobalApp.DialogThreadNotify.WakeupMain();
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //MSSqlServer

        private static bool BackupMSSqlServer(string pFileName)
        {
            try
            {
                string sql = string.Format(@"
                  BACKUP DATABASE {0} TO DISK='{1}';"
                  , GlobalFramework.DatabaseName
                  , pFileName
                );
                GlobalFramework.SessionXpo.ExecuteScalar(sql);
                return true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                throw;
            }
            finally
            {
                //Notify WakeupMain and Call ReadyEvent
                GlobalApp.DialogThreadNotify.WakeupMain();
            }
        }

        private static bool RestoreMSSqlServer(string pConnectionString, string pFileName)
        {
            //Show Connections
            //SELECT * FROM sys.sysprocesses WHERE dbid = DB_ID('logicpos_backup')
            try
            {
                string sql = string.Format(@"
                  USE MASTER; 
                  ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                  RESTORE DATABASE {0} FROM DISK = '{1}';
                  ALTER DATABASE {0} SET MULTI_USER;
                  USE {0};
                  "
                  , GlobalFramework.DatabaseName
                  , pFileName
                );
                //_log.Debug(string.Format("RestoreMSSqlServer.sql: [{0}]", sql));

                GlobalFramework.SessionXpo.ExecuteScalar(sql);

                //Direct Connection
                //SqlConnection connection = new SqlConnection(pConnectionString);
                //connection.Open();
                //SqlCommand cmd = new SqlCommand();
                //SqlDataReader reader;
                //cmd.CommandText = sql;
                //cmd.CommandType = CommandType.Text;
                //cmd.Connection = connection;
                //reader = cmd.ExecuteReader();
                //connection.Close();

                return true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                throw;
            }
            finally
            {
                //Notify WakeupMain and Call ReadyEvent
                GlobalApp.DialogThreadNotify.WakeupMain();
            }
        }

        /*
        //Notes
        //The database must not be in use when backing up
        //The folder holding the file must have appropriate permissions given
        private static bool BackupMSSqlServer(string pConnectionString, string pFileName)
        {
          if (_debug) _log.Debug(string.Format("BackupMSSqlServer fileName: [{0}]", pFileName));

          try
          {
            ServerConnection connection = new ServerConnection(_backupConnectionString);
            Server server = new Server(connection);
            //Using SQL Server authentication
            if (GlobalFramework.DatabaseServer.ToUpper() != @".\SQLEXPRESS".ToUpper())
            {
              server.ConnectionContext.LoginSecure = false;
              server.ConnectionContext.Login = GlobalFramework.DatabaseUser;
              server.ConnectionContext.Password = GlobalFramework.DatabasePassword;
            }
            Backup source = new Backup();
            source.Action = BackupActionType.Database;
            source.Database = GlobalFramework.DatabaseName;
            BackupDeviceItem destination = new BackupDeviceItem(pFileName, DeviceType.File);
            source.Devices.Add(destination);
            source.SqlBackup(server);
            connection.Disconnect();
            if (server.ConnectionContext.IsOpen) server.ConnectionContext.Disconnect();

            return true;
          }
          catch (Exception ex)
          {
            _log.Error(ex.Message, ex);
            return false;
          }
        }

        private static bool RestoreMSSqlServer(string pConnectionString, string pFileName)
        {
          if (_debug) _log.Debug(string.Format("RestoreMSSqlServer fileName: [{0}]", pFileName));

          try
          {
            ServerConnection connection = new ServerConnection(_backupConnectionString);
            Server server = new Server(connection);
            //Using SQL Server authentication
            if (GlobalFramework.DatabaseServer.ToUpper() != @".\SQLEXPRESS".ToUpper())
            {
              server.ConnectionContext.LoginSecure = false;
              server.ConnectionContext.Login = GlobalFramework.DatabaseUser;
              server.ConnectionContext.Password = GlobalFramework.DatabasePassword;
            }
            //Deletes the specified database and drops any active connection
            server.KillDatabase(GlobalFramework.DatabaseName);
            Restore destination = new Restore();
            destination.Action = RestoreActionType.Database;
            destination.Database = GlobalFramework.DatabaseName;
            BackupDeviceItem source = new BackupDeviceItem(pFileName, DeviceType.File);
            destination.Devices.Add(source);
            destination.ReplaceDatabase = true;
            destination.SqlRestore(server);
            if (server.ConnectionContext.IsOpen) server.ConnectionContext.Disconnect();

            return true;
          }
          catch (Exception ex)
          {
            _log.Error(ex.Message, ex);
            return false;
          }
        }
        */

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //MySQL

        private static bool BackupMySql(string pConnectionString, string pFileName)
        {
            if (_debug) _log.Debug(string.Format("BackupMySql fileName: [{0}]", pFileName));

            try
            {
                using (MySqlConnection conn = new MySqlConnection(pConnectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        using (MySqlBackup mb = new MySqlBackup(cmd))
                        {
                            cmd.Connection = conn;
                            conn.Open();
                            mb.ExportToFile(pFileName);
                            conn.Close();
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return false;
            }
            finally
            {
                //Notify WakeupMain and Call ReadyEvent
                GlobalApp.DialogThreadNotify.WakeupMain();
            }
        }

        private static bool RestoreMySql(string pConnectionString, string pFileName)
        {
            if (_debug) _log.Debug(string.Format("RestoreMySql fileName: [{0}]", pFileName));

            try
            {
                using (MySqlConnection conn = new MySqlConnection(pConnectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        using (MySqlBackup mb = new MySqlBackup(cmd))
                        {
                            cmd.Connection = conn;
                            conn.Open();
                            mb.ImportFromFile(pFileName);
                            conn.Close();
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return false;
            }
            finally
            {
                //Notify WakeupMain and Call ReadyEvent
                GlobalApp.DialogThreadNotify.WakeupMain();
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Helpers

        public static DateTime GetLastBackupDate()
        {
            string sql = "SELECT CreatedAt FROM sys_systembackup ORDER BY CreatedAt DESC";
            var createdAt = GlobalFramework.SessionXpo.ExecuteScalar(sql);
            DateTime result = Convert.ToDateTime(createdAt);
            return result;
        }
    }
}
