using ClosedXML.Excel;
using ExcelDataReader;
using Gtk;
using logicpos.App;
using logicpos.Classes.DataLayer;
using logicpos.Classes.Enums;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Xpo;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.Settings.Enums;
using LogicPOS.Settings.Extensions;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

//TK016231 - Backoffice - Importação/Exportação clientes/artigos 
namespace logicpos
{
    public class ExcelProcessing
    {
        private static string _fileExtension;
        private static readonly System.Drawing.Size _sizeDialog = new System.Drawing.Size(800, 300);
        private static Thread _threadImport;
        private static Thread _threadExport;

        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // Import/Export XLS 
        public static ImportExportFileOpen ImportExportFileOpen;
        private static string pathBackups;

        public static void OpenFilePicker(Gtk.Window pSourceWindow, ImportExportFileOpen pImportFrom)
        {
            try
            {
                //FrameworkUtils.ShowWaitingCursor();

                Init();

                string sql = string.Empty;
                string fileName = string.Empty;
                string fileNamePacked = string.Empty;
                //default pathBackups from Settings, can be Overrided in ChooseFromFilePickerDialog Mode

                DataBaseBackupFileInfo fileInfo = null;
                Guid systemBackupGuid = Guid.Empty;
                //Required to assign current FileName and FileNamePacked after restore, else name will be the TempName ex acegvpls.soj & n2sjiamk.32o
                //sys_systembackup systemBackup = null;
                string currentFileName = string.Empty, currentFileNamePacked = string.Empty, currentFilePath = string.Empty, currentFileHash = string.Empty;

                switch (pImportFrom)
                {
                    case ImportExportFileOpen.OpenExcelArticles:

                        string windowName = (CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_articles"));
                        FileFilter fileFilterBackups = Utils.GetFileFilterImportExport();
                        PosFilePickerDialog dialog = new PosFilePickerDialog(pSourceWindow, DialogFlags.DestroyWithParent, fileFilterBackups, FileChooserAction.Open, windowName);
                        ResponseType response = (ResponseType)dialog.Run();
                        if (response == ResponseType.Ok)
                        {
                            fileNamePacked = dialog.FilePicker.Filename;
                            //Override Default pathBackups
                            pathBackups = string.Format("{0}/", Path.GetDirectoryName(fileNamePacked));
                            dialog.Destroy();
                        }
                        else
                        {
                            dialog.Destroy();
                            break;
                        }

                        if (!string.IsNullOrEmpty(pathBackups))
                        {
                            fileName = Path.ChangeExtension(fileNamePacked, _fileExtension);
                            ReadExcel(fileName, pSourceWindow, pImportFrom);
                        }
                        else
                        {
                            //Require to assign filename and packed filename from fileInfo
                            fileName = fileInfo.FileName;
                            fileNamePacked = fileInfo.FileName;
                            ReadExcel(fileName, pSourceWindow, pImportFrom);
                        }
                        break;

                    case ImportExportFileOpen.OpenExcelCostumers:

                        windowName = (CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_customer"));
                        fileFilterBackups = Utils.GetFileFilterImportExport();
                        dialog = new PosFilePickerDialog(pSourceWindow, DialogFlags.DestroyWithParent, fileFilterBackups, FileChooserAction.Open, windowName);
                        response = (ResponseType)dialog.Run();
                        if (response == ResponseType.Ok)
                        {
                            fileNamePacked = dialog.FilePicker.Filename;
                            //Override Default pathBackups
                            pathBackups = string.Format("{0}/", Path.GetDirectoryName(fileNamePacked));
                            dialog.Destroy();
                        }
                        else
                        {
                            dialog.Destroy();
                            break;
                        }

                        if (!string.IsNullOrEmpty(pathBackups))
                        {
                            fileName = Path.ChangeExtension(fileNamePacked, _fileExtension);
                            ReadExcel(fileName, pSourceWindow, pImportFrom);
                        }
                        else
                        {
                            //Require to assign filename and packed filename from fileInfo
                            fileName = fileInfo.FileName;
                            fileNamePacked = fileInfo.FileName;
                            ReadExcel(fileName, pSourceWindow, pImportFrom);
                        }
                        break;

                    case ImportExportFileOpen.ExportArticles:

                        windowName = (CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_articles"));
                        fileFilterBackups = Utils.GetFileFilterImportExport();
                        dialog = new PosFilePickerDialog(pSourceWindow, DialogFlags.DestroyWithParent, fileFilterBackups, FileChooserAction.Save, windowName);
                        response = (ResponseType)dialog.Run();
                        if (response == ResponseType.Ok)
                        {
                            fileNamePacked = dialog.FilePicker.Filename;
                            //Override Default pathBackups
                            pathBackups = string.Format("{0}/", Path.GetDirectoryName(fileNamePacked));
                            dialog.Destroy();
                        }
                        else
                        {
                            dialog.Destroy();
                            break;
                        }

                        if (!string.IsNullOrEmpty(pathBackups))
                        {
                            fileName = Path.ChangeExtension(fileNamePacked, _fileExtension);

                            CreateDataTableFromDB(fileName, pSourceWindow, pImportFrom);
                        }
                        else
                        {
                            //Require to assign filename and packed filename from fileInfo
                            fileName = fileInfo.FileName;
                            fileNamePacked = fileInfo.FileName;
                            CreateDataTableFromDB(fileName, pSourceWindow, pImportFrom);
                        }
                        break;

                    case ImportExportFileOpen.ExportCustomers:

                        windowName = (CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_customer"));
                        fileFilterBackups = Utils.GetFileFilterImportExport();
                        dialog = new PosFilePickerDialog(pSourceWindow, DialogFlags.DestroyWithParent, fileFilterBackups, FileChooserAction.Save, windowName);
                        response = (ResponseType)dialog.Run();
                        if (response == ResponseType.Ok)
                        {
                            fileNamePacked = dialog.FilePicker.Filename;
                            //Override Default pathBackups
                            pathBackups = string.Format("{0}/", Path.GetDirectoryName(fileNamePacked));
                            dialog.Destroy();
                        }
                        else
                        {
                            dialog.Destroy();
                            break;
                        }

                        if (!string.IsNullOrEmpty(pathBackups))
                        {
                            fileName = Path.ChangeExtension(fileNamePacked, _fileExtension);

                            CreateDataTableFromDB(fileName, pSourceWindow, pImportFrom);
                        }
                        else
                        {
                            //Require to assign filename and packed filename from fileInfo
                            fileName = fileInfo.FileName;
                            fileNamePacked = fileInfo.FileName;
                            CreateDataTableFromDB(fileName, pSourceWindow, pImportFrom);
                        }
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

        public static void Init()
        {
            switch (ImportExportFileOpen)
            {
                case ImportExportFileOpen.OpenExcelArticles:
                case ImportExportFileOpen.OpenExcelCostumers:
                    _fileExtension = "xlsx";
                    break;
                default:
                    break;
            }
        }


        public static DataTable ReadExcel(string path, Gtk.Window pSourceWindow, ImportExportFileOpen pImportFrom)
        {
            DataTable dtResult = new DataTable();
            try
            {
                _logger.Debug("Proccess file: " + path);
                if (path.Contains(".xls"))
                {
                    using (var stream = File.Open(path, FileMode.Open, FileAccess.ReadWrite))
                    {
                        // Auto-detect format, supports:
                        //  - Binary Excel files (2.0-2003 format; *.xls)
                        //  - OpenXml Excel files (2007 format; *.xlsx)
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            var result = reader.AsDataSet();

                            if (result.Tables.Count > 0)
                            {
                                dtResult = result.Tables[0];
                            }
                        }
                    }
                }
                if (dtResult != null)
                {
                    int result = 0;
                    switch (pImportFrom)
                    {
                        case ImportExportFileOpen.OpenExcelArticles:
                            _threadImport = new Thread(() => result = SaveArticles(dtResult, pSourceWindow));
                            Utils.ThreadStart(pSourceWindow, _threadImport, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_import_articles"));
                            _threadImport.Abort();
                            //SaveArticles(dtResult, pSourceWindow);
                            break;
                        case ImportExportFileOpen.OpenExcelCostumers:
                            _threadImport = new Thread(() => result = SaveCostumers(dtResult, pSourceWindow));
                            Utils.ThreadStart(pSourceWindow, _threadImport, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_import_customers"));
                            _threadImport.Abort();
                            //SaveCostumers(dtResult, pSourceWindow);
                            break;

                        default:
                            break;
                    }
                    if (result == 1)
                    {
                        Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, MessageType.Info, ButtonsType.Close, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_message_operation_successfully"), CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_message_import_successfully"));
                    }
                    else if (result == -1)
                    {
                        Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, MessageType.Warning, ButtonsType.Close, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_warning"), CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_message_partial_import"));
                    }
                    else
                    {
                        Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, MessageType.Error, ButtonsType.Close, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_error"), CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_message_import_error"));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ReadExcel: Error proccess file " + ex.Message, ex);
                Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, MessageType.Error, ButtonsType.Close, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_error"), string.Format(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_message_import_error")));
            }

            return dtResult;
        }

        public static string GetLastCodeFromTable(string tableName)
        {
            string lastArticleCode = "0";
            try
            {
                if (DatabaseSettings.DatabaseType.IsSqlServer())
                {
                    string lastArticleSql = string.Format("SELECT MAX(CAST(Code AS INT))FROM {0}", tableName);
                    lastArticleCode = XPOSettings.Session.ExecuteScalar(lastArticleSql).ToString();
                    return lastArticleCode;
                }
                else if (DatabaseSettings.DatabaseType.IsSQLite())
                {
                    string lastArticleSql = string.Format("SELECT MAX(CAST(Code AS INT))FROM {0}", tableName);
                    lastArticleCode = XPOSettings.Session.ExecuteScalar(lastArticleSql).ToString();
                    return lastArticleCode;
                }
                else if (DatabaseSettings.DatabaseType.IsMySql())
                {
                    string lastArticleSql = string.Format("SELECT MAX(code) as max FROM {0}", tableName);
                    lastArticleCode = XPOSettings.Session.ExecuteScalar(lastArticleSql).ToString();
                }
                return lastArticleCode;
            }
            catch (Exception ex)
            {
                _logger.Error("Error:  " + ex.Message, ex);
            }
            return lastArticleCode;
        }


        public static int SaveArticles(DataTable dtImport, Gtk.Window pSourceWindow)
        {
            var result = 0;
            string queryOrderedString = "SELECT * FROM fin_article";
            int indexDesignation = 0;
            bool flagImport = false;
            var DT = new DataTable();
            string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string articleFamily = "0";
            string articleSubFamily = "0";

            Dictionary<string, string> articlesInDbCollection = new Dictionary<string, string>();
            var articlesInDb = XPOSettings.Session.ExecuteQueryWithMetadata(queryOrderedString);


            for (int w = 0; w < articlesInDb.ResultSet[0].Rows.Length; w++)
            {
                if (articlesInDb.ResultSet[0].Rows[w].Values[0].Equals("Designation"))
                {
                    indexDesignation = w;
                    break;
                }

            }
            for (int j = 1; j < articlesInDb.ResultSet[1].Rows.Length; j++)
            {
                articlesInDbCollection.Add(articlesInDb.ResultSet[1].Rows[j].Values[indexDesignation].ToString(), articlesInDb.ResultSet[1].Rows[j].Values[0].ToString());
            }

            try
            {
                //string dc = articlesInDb.ResultSet[1].Rows[2].Values[0].ToString();
                fin_article article = new fin_article();
                bool firstRow = true;
                foreach (DataRow row in dtImport.Rows)
                {

                    try
                    {
                        //Ignore firse row that contains column names
                        if (firstRow)
                        {
                            firstRow = false;
                            continue;
                        }
                        Guid oid = Guid.NewGuid();
                        Guid oidFamily = Guid.NewGuid();
                        Guid oidSubFamily = Guid.NewGuid();

                        article.Disabled = false;
                        article.Notes = null;
                        string articleCreatedAt = dateTime;
                        string articleCreatedBy = XPOSettings.LoggedUser.Oid.ToString().Replace("logicpos.datalayer.DataLayer.Xpo.sys_userdetail", "");
                        article.CreatedWhere = null;
                        string articleUpdatedAt = dateTime;
                        string articleUpdatedBy = articleCreatedBy;
                        article.UpdatedWhere = null;
                        article.ButtonImage = null;
                        var count = row.ItemArray.GetValue(4).ToString().Length;
                        if (row.ItemArray.GetValue(4).ToString() != "")
                        {
                            var priceF = row.ItemArray.GetValue(4).ToString();
                            article.Price1 = Convert.ToDecimal(priceF);
                        }
                        else
                        {
                            article.Price1 = 0;
                        }
                        article.Price1Promotion = 0;
                        article.Price2 = 0;
                        article.Price2Promotion = 0;
                        article.Price3 = 0;
                        article.Price3Promotion = 0;
                        article.Price4 = 0;
                        article.Price4Promotion = 0;
                        article.Price5 = 0;
                        article.Price5Promotion = 0;
                        article.Discount = 0;
                        article.DefaultQuantity = 0;
                        string articleFavorite = "0";
                        string articleType = "edf4841e-e451-4c7b-9bd0-ee02860ba937";
                        string articleClass = "6924945d-f99e-476b-9c4d-78fb9e2b30a3";
                        string articleUnitMeasure = "4c81aa20-98ec-4497-b740-165cdb5fa395";
                        string articleUnitSize = "18f564aa-7da5-4a1c-9091-8014638b818c";
                        string articleVatOnTable = "cee00590-7317-41b8-af46-66560401096b";
                        string articleVatDirectSelling = "cee00590-7317-41b8-af46-66560401096b";

                        string sql = "";



                        if (row.ItemArray.GetValue(0).ToString() != "") { article.Code = row.ItemArray.GetValue(0).ToString(); } else { article.Code = (Convert.ToInt32(GetLastCodeFromTable("fin_article")) + 10).ToString(); }

                        if (row.ItemArray.GetValue(0).ToString() != "" && row.ItemArray.GetValue(0).ToString().Any(char.IsDigit))
                        {

                            article.Ord = Convert.ToUInt32(Regex.Match(row.ItemArray.GetValue(0).ToString(), @"\d+").Value.ToString());
                        }
                        else
                        {
                            article.Ord = (Convert.ToUInt32(GetLastCodeFromTable("fin_article")) + 10);
                        }
                        article.Designation = row.ItemArray.GetValue(1).ToString();
                        try
                        {
                            if (row.ItemArray.GetValue(5) != null || row.ItemArray.GetValue(5).ToString() != "")
                            {
                                sql = string.Format("SELECT OID FROM fin_configurationvatrate where Value = '{0}'", Convert.ToDecimal(row.ItemArray.GetValue(5)).ToString("F").Replace(",", "."));
                                articleVatOnTable = XPOSettings.Session.ExecuteScalar(sql).ToString();
                                articleVatDirectSelling = articleVatOnTable;
                            }

                            //Verifica se a Familia existe, se não existe, cria
                            if (row.ItemArray.GetValue(2) != null || row.ItemArray.GetValue(2).ToString() != "")
                            {
                                sql = string.Format("SELECT OID FROM fin_articlefamily where Designation = '{0}'", row.ItemArray.GetValue(2));
                                var sqlquery = XPOSettings.Session.ExecuteScalar(sql);
                                if (sqlquery == null && row.ItemArray.GetValue(2) != null)
                                {
                                    string tableName = "fin_articlefamily";
                                    string lastCode = (Convert.ToInt32(GetLastCodeFromTable(tableName)) + 10).ToString();

                                    string sqlInsertFamily = string.Format(@"INSERT INTO fin_articlefamily (oid, createdat, createdby, 
                                                        updatedat, updatedby, Ord, Code, Designation) values (
                                                         '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')",
                                                                oidFamily, articleCreatedAt, articleCreatedBy, articleUpdatedAt,
                                                                articleUpdatedBy, lastCode, lastCode, row.ItemArray.GetValue(2));
                                    XPOSettings.Session.ExecuteNonQuery(sqlInsertFamily);
                                    articleFamily = oidFamily.ToString();
                                }
                                else { articleFamily = sqlquery.ToString(); }
                            }
                            else articleFamily = null;


                            //Verifica se a Sub Familia existe, se não existe, cria
                            if (row.ItemArray.GetValue(3) != null)
                            {
                                sql = string.Format("SELECT OID FROM fin_articlesubfamily where Designation = '{0}'", row.ItemArray.GetValue(3));
                                var sqlquery = XPOSettings.Session.ExecuteScalar(sql);
                                if (sqlquery == null && row.ItemArray.GetValue(3) != null)
                                {
                                    string tableName = "fin_articlesubfamily";
                                    string lastCode = (Convert.ToInt32(GetLastCodeFromTable(tableName)) + 10).ToString();
                                    string sqlInsertFamily = string.Format(@"INSERT INTO fin_articlesubfamily (oid, createdat, createdby, 
                                                        updatedat, updatedby, Ord, Code, Designation, Family) values (
                                                         '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')",
                                                                oidSubFamily, articleCreatedAt, articleCreatedBy, articleUpdatedAt,
                                                                articleUpdatedBy, lastCode, lastCode, row.ItemArray.GetValue(3), articleFamily);
                                    XPOSettings.Session.ExecuteNonQuery(sqlInsertFamily);
                                    articleSubFamily = oidSubFamily.ToString();
                                }
                                else { articleSubFamily = sqlquery.ToString(); }
                            }
                            else articleSubFamily = null;
                            try
                            {

                                if (!articlesInDbCollection.ContainsKey(row.ItemArray.GetValue(1).ToString()))
                                {
                                    string user = XPOSettings.LoggedUser.ToString();
                                    sql = string.Format(@"insert into fin_article (Oid, Notes, CreatedAt, CreatedBy, CreatedWhere, UpdatedAt, 
                                                UpdatedBy, UpdatedWhere, ButtonImage, Price1, Price1Promotion, Price2, Price2Promotion,
                                                Price3, Price3Promotion, Price4, Price4Promotion, Price5, Price5Promotion, Discount, DefaultQuantity, 
                                                Type, Class, UnitMeasure, UnitSize, VatOnTable, 
                                                VatDirectSelling, code, ord, Designation, family, subfamily) 
                                                values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}'
                                                ,'{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{22}','{23}','{24}'
                                                ,'{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}')", oid, article.Notes, articleCreatedAt, articleCreatedBy,
                                                                article.CreatedWhere, articleUpdatedAt, articleUpdatedBy, article.UpdatedWhere,
                                                                article.ButtonImage, article.Price1, article.Price1Promotion, article.Price2, article.Price2Promotion,
                                                                article.Price3, article.Price3Promotion, article.Price4, article.Price4Promotion, article.Price5,
                                                                article.Price5Promotion, article.Discount, article.DefaultQuantity, articleFavorite, articleType,
                                                                articleClass, articleUnitMeasure, articleUnitSize, articleVatOnTable, articleVatDirectSelling,
                                                                article.Code, article.Ord, article.Designation, articleFamily, articleSubFamily).Replace("''", "NULL");
                                    XPOSettings.Session.ExecuteNonQuery(sql);
                                }
                                else
                                {
                                    string Eoid = articlesInDbCollection[(row.ItemArray.GetValue(1).ToString())].ToString();
                                    sql = string.Format(@"update fin_article set Notes='{1}', 
                                                UpdatedAt='{5}',UpdatedBy='{6}', UpdatedWhere='{7}', Price1='{9}',
                                                code='{28}', ord='{29}', Designation='{30}', family='{31}', subfamily='{32}' WHERE OID='{33}'", oid, article.Notes, articleCreatedAt, articleCreatedBy,
                                                                article.CreatedWhere, articleUpdatedAt, articleUpdatedBy, article.UpdatedWhere,
                                                                article.ButtonImage, article.Price1, article.Price1Promotion, article.Price2, article.Price2Promotion,
                                                                article.Price3, article.Price3Promotion, article.Price4, article.Price4Promotion, article.Price5,
                                                                article.Price5Promotion, article.Discount, article.DefaultQuantity, articleFavorite, articleType,
                                                                articleClass, articleUnitMeasure, articleUnitSize, articleVatOnTable, articleVatDirectSelling,
                                                                article.Code, article.Ord, article.Designation, articleFamily, articleSubFamily, Eoid).Replace("''", "NULL");
                                    XPOSettings.Session.ExecuteNonQuery(sql);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.Error("Error:  " + ex.Message, ex);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Error("Error:  " + ex.Message, ex);
                            flagImport = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Error:  " + ex.Message, ex);
                    }

                }

                if (!flagImport) { return 1; }
                else { return -1; }

            }
            catch (Exception ex)
            {
                _logger.Error("Error:  " + ex.Message, ex);
                return result;
            }
            finally
            {
                //Notify WakeupMain and Call ReadyEvent
                GlobalApp.DialogThreadNotify.WakeupMain();
            }
        }

        public static int SaveCostumers(DataTable dtImport, Gtk.Window pSourceWindow)
        {
            int indexFiscalNumber = 0;
            bool flagImport = false;
            string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string queryCostumerString = "SELECT * FROM erp_customer";
            Dictionary<string, string> costumersInDbCollection = new Dictionary<string, string>();
            var costumersInDb = XPOSettings.Session.ExecuteQueryWithMetadata(queryCostumerString);

            for (int w = 0; w < costumersInDb.ResultSet[0].Rows.Length; w++)
            {
                if (costumersInDb.ResultSet[0].Rows[w].Values[0].Equals("FiscalNumber"))
                {
                    indexFiscalNumber = w;
                    break;
                }

            }
            for (int j = 1; j < costumersInDb.ResultSet[1].Rows.Length; j++)
            {
                costumersInDbCollection.Add(CryptographyUtils.Decrypt(costumersInDb.ResultSet[1].Rows[j].Values[indexFiscalNumber].ToString(), true, PluginSettings.SecretKey), costumersInDb.ResultSet[1].Rows[j].Values[0].ToString());
            }

            try
            {

                erp_customer costumer = new erp_customer();
                bool firstRow = true;
                foreach (DataRow row in dtImport.Rows)
                {
                    //Ignore firse row that contains column names
                    if (firstRow)
                    {
                        firstRow = false;
                        continue;
                    }
                    Guid oid = Guid.NewGuid();
                    Guid oidFamily = Guid.NewGuid();
                    Guid oidSubFamily = Guid.NewGuid();

                    costumer.Disabled = false;
                    costumer.Notes = null;
                    string costumerCreatedAt = dateTime;
                    string costumerCreatedBy = XPOSettings.LoggedUser.Oid.ToString().Replace("logicpos.datalayer.DataLayer.Xpo.sys_userdetail", "");
                    costumer.CreatedWhere = null;
                    string costumerUpdatedAt = dateTime;
                    string costumerUpdatedBy = costumerCreatedBy;
                    string customerType = "a4b3811f-9851-430d-810e-f8be7ac3f392";
                    string priceType = "cf17a218-b687-4b82-a8f4-0905594ac1f5";
                    string country = "e7e8c325-a0d4-4908-b148-508ed750676a";
                    costumer.UpdatedWhere = null;

                    if (row.ItemArray.GetValue(0).ToString() != "") { costumer.Code = Convert.ToUInt32(row.ItemArray.GetValue(0)); } else { costumer.Code = (Convert.ToUInt32(GetLastCodeFromTable("erp_customer")) + 10); }
                    if (row.ItemArray.GetValue(0).ToString() != "") { costumer.Ord = Convert.ToUInt32(row.ItemArray.GetValue(0)); } else { costumer.Ord = (Convert.ToUInt32(GetLastCodeFromTable("erp_customer")) + 10); }
                    if (row.ItemArray.GetValue(1).ToString() != "") costumer.FiscalNumber = CryptographyUtils.Encrypt(row.ItemArray.GetValue(1).ToString(), true, PluginSettings.SecretKey); else costumer.FiscalNumber = CryptographyUtils.Encrypt("", true, PluginSettings.SecretKey);
                    if (row.ItemArray.GetValue(2).ToString() != "") costumer.Name = CryptographyUtils.Encrypt(row.ItemArray.GetValue(2).ToString(), true, PluginSettings.SecretKey); else costumer.Name = CryptographyUtils.Encrypt("", true, PluginSettings.SecretKey);
                    if (row.ItemArray.GetValue(3).ToString() != "") costumer.Address = CryptographyUtils.Encrypt(row.ItemArray.GetValue(3).ToString(), true, PluginSettings.SecretKey); else costumer.Address = CryptographyUtils.Encrypt("", true, PluginSettings.SecretKey);
                    if (row.ItemArray.GetValue(4).ToString() != "") costumer.Locality = CryptographyUtils.Encrypt(row.ItemArray.GetValue(4).ToString(), true, PluginSettings.SecretKey); else costumer.Locality = CryptographyUtils.Encrypt("", true, PluginSettings.SecretKey);
                    if (row.ItemArray.GetValue(5).ToString() != "") costumer.ZipCode = CryptographyUtils.Encrypt(row.ItemArray.GetValue(5).ToString(), true, PluginSettings.SecretKey); else costumer.ZipCode = CryptographyUtils.Encrypt("", true, PluginSettings.SecretKey);
                    if (row.ItemArray.GetValue(6).ToString() != "") costumer.City = CryptographyUtils.Encrypt(row.ItemArray.GetValue(6).ToString(), true, PluginSettings.SecretKey); else costumer.City = CryptographyUtils.Encrypt("", true, PluginSettings.SecretKey);
                    if (row.ItemArray.GetValue(7).ToString() != "") costumer.Phone = CryptographyUtils.Encrypt(row.ItemArray.GetValue(7).ToString(), true, PluginSettings.SecretKey); else costumer.Phone = CryptographyUtils.Encrypt("", true, PluginSettings.SecretKey);
                    if (row.ItemArray.GetValue(8).ToString() != "") costumer.MobilePhone = CryptographyUtils.Encrypt(row.ItemArray.GetValue(8).ToString(), true, PluginSettings.SecretKey); else costumer.MobilePhone = CryptographyUtils.Encrypt("", true, PluginSettings.SecretKey);
                    if (row.ItemArray.GetValue(9).ToString() != "") costumer.Email = CryptographyUtils.Encrypt(row.ItemArray.GetValue(9).ToString(), true, PluginSettings.SecretKey); else costumer.Email = CryptographyUtils.Encrypt("", true, PluginSettings.SecretKey);

                    try
                    {

                        if (!costumersInDbCollection.ContainsKey(row.ItemArray.GetValue(1).ToString()) && costumer.FiscalNumber != "")
                        {
                            string user = XPOSettings.LoggedUser.ToString();
                            string sql = string.Format(@"insert into erp_customer (Oid, CreatedAt, CreatedBy, CreatedWhere, UpdatedAt, 
                                                UpdatedBy, UpdatedWhere, code, ord, Name, Address, locality, ZipCode, City, Phone, MobilePhone, Email,
                                                CustomerType, PriceType, Country, FiscalNumber, CodeInternal) 
                                                values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}'
                                                ,'{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}')", oid, costumerCreatedAt, costumerCreatedBy, costumer.CreatedWhere,
                                                costumerUpdatedAt, costumerUpdatedBy, costumer.UpdatedWhere, costumer.Code, costumer.Ord, costumer.Name,
                                                costumer.Address, costumer.Locality, costumer.ZipCode, costumer.City, costumer.Phone,
                                                costumer.MobilePhone, costumer.Email, customerType, priceType, country, costumer.FiscalNumber, oid.ToString().Replace("-", "").Substring(0, 21)
                                                ).Replace("''", "NULL");
                            XPOSettings.Session.ExecuteNonQuery(sql);
                        }
                        else
                        {
                            string Eoid = costumersInDbCollection[(row.ItemArray.GetValue(1).ToString())].ToString();
                            string sql = string.Format(@"update erp_customer set CreatedAt='{0}', CreatedBy='{1}', CreatedWhere='{2}', UpdatedAt='{3}', 
                                                UpdatedBy='{4}', UpdatedWhere='{5}', code='{6}', ord='{7}', Name='{8}', Address='{9}', locality='{10}', ZipCode='{11}', 
                                                City='{12}', Phone='{13}', MobilePhone='{14}', Email='{15}',CustomerType='{16}', PriceType='{17}', Country='{18}',
                                                FiscalNumber='{19}' WHERE OID='{20}'", costumerCreatedAt, costumerCreatedBy, costumer.CreatedWhere,
                                                costumerUpdatedAt, costumerUpdatedBy, costumer.UpdatedWhere, costumer.Code, costumer.Ord, costumer.Name,
                                                costumer.Address, costumer.Locality, costumer.ZipCode, costumer.City, costumer.Phone,
                                                costumer.MobilePhone, costumer.Email, customerType, priceType, country, costumer.FiscalNumber, Eoid).Replace("''", "NULL");
                            XPOSettings.Session.ExecuteNonQuery(sql);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Error:  " + ex.Message, ex);
                        flagImport = true;
                    }
                }
                if (!flagImport) { return 1; }
                else { return -1; }
            }
            catch
            {
                return 0;
            }
            finally
            {
                GlobalApp.DialogThreadNotify.WakeupMain();
            }
        }

        public static void CreateDataTableFromDB(string path, Gtk.Window pSourceWindow, ImportExportFileOpen pImportFrom)
        {
            DataTable importFromDBdataTable = new DataTable();

            string queryOrderedString = @"SELECT t1.Code, t1.Designation, t2.Designation As Family, t3.Designation As SubFamily, t1.Price1, t4.Value AS VAT
                                        FROM fin_article t1 , fin_articlefamily t2 , fin_articlesubfamily t3,  fin_configurationvatrate t4
                                        WHERE t1.Family = t2.Oid AND t1.SubFamily = t3.Oid AND t1.VatOnTable = t4.Oid";

            if (AppOperationModeSettings.AppMode == AppOperationMode.Retail)
            {
                queryOrderedString = @"SELECT t1.Code, t1.Designation, t2.Designation As Family, t3.Designation As SubFamily, t1.Price1, t4.Value AS VAT
                                        FROM fin_article t1 , fin_articlefamily t2 , fin_articlesubfamily t3,  fin_configurationvatrate t4
                                        WHERE t1.Family = t2.Oid AND t1.SubFamily = t3.Oid AND t1.VatDirectSelling = t4.Oid";
            }

            string queryCostumerString = "SELECT Code, FiscalNumber, Name, Address, Locality, ZipCode, City, Phone, MobilePhone, Email FROM erp_customer";

            int indexCode = 0;
            int indexFiscalNumber = 0;
            int indexName = 0;
            int indexAddress = 0;
            int indexLocality = 0;
            int indexZipCode = 0;
            int indexCity = 0;
            int indexPhone = 0;
            int indexMobilePhone = 0;
            int indexEmail = 0;

            int indexDesignation = 0;
            int indexFamily = 0;
            int indexSubFamily = 0;
            int indexPrice = 0;
            int indexVAT = 0;

            try
            {
                switch (pImportFrom)
                {
                    case ImportExportFileOpen.ExportArticles:
                        var articlesInDb = XPOSettings.Session.ExecuteQueryWithMetadata(queryOrderedString);

                        for (int w = 0; w < articlesInDb.ResultSet[0].Rows.Length; w++)
                        {
                            if (articlesInDb.ResultSet[0].Rows[w].Values[0].Equals("Code"))
                            {
                                importFromDBdataTable.Columns.Add(articlesInDb.ResultSet[0].Rows[w].Values[0].ToString(), typeof(string));
                                indexCode = w;
                            }
                            if (articlesInDb.ResultSet[0].Rows[w].Values[0].Equals("Designation"))
                            {
                                importFromDBdataTable.Columns.Add(articlesInDb.ResultSet[0].Rows[w].Values[0].ToString(), typeof(string));
                                indexDesignation = w;
                            }
                            if (articlesInDb.ResultSet[0].Rows[w].Values[0].Equals("Family"))
                            {
                                importFromDBdataTable.Columns.Add(articlesInDb.ResultSet[0].Rows[w].Values[0].ToString(), typeof(string));
                                indexFamily = w;
                            }
                            if (articlesInDb.ResultSet[0].Rows[w].Values[0].Equals("SubFamily"))
                            {
                                importFromDBdataTable.Columns.Add(articlesInDb.ResultSet[0].Rows[w].Values[0].ToString(), typeof(string));
                                indexSubFamily = w;
                            }
                            if (articlesInDb.ResultSet[0].Rows[w].Values[0].Equals("Price1"))
                            {
                                importFromDBdataTable.Columns.Add(articlesInDb.ResultSet[0].Rows[w].Values[0].ToString(), typeof(string));
                                indexPrice = w;
                            }
                            if (articlesInDb.ResultSet[0].Rows[w].Values[0].Equals("VAT"))
                            {
                                importFromDBdataTable.Columns.Add(articlesInDb.ResultSet[0].Rows[w].Values[0].ToString(), typeof(string));
                                indexVAT = w;
                            }

                        }
                        for (int j = 0; j < articlesInDb.ResultSet[1].Rows.Length; j++)
                        {
                            importFromDBdataTable.Rows.Add(articlesInDb.ResultSet[1].Rows[j].Values[indexCode],
                            articlesInDb.ResultSet[1].Rows[j].Values[indexDesignation] == null ? "" : articlesInDb.ResultSet[1].Rows[j].Values[indexDesignation].ToString(),
                            articlesInDb.ResultSet[1].Rows[j].Values[indexFamily] == null ? "" : articlesInDb.ResultSet[1].Rows[j].Values[indexFamily].ToString(),
                            articlesInDb.ResultSet[1].Rows[j].Values[indexSubFamily] == null ? "" : articlesInDb.ResultSet[1].Rows[j].Values[indexSubFamily].ToString(),
                            articlesInDb.ResultSet[1].Rows[j].Values[indexPrice] == null ? "" : articlesInDb.ResultSet[1].Rows[j].Values[indexPrice].ToString(),
                            articlesInDb.ResultSet[1].Rows[j].Values[indexVAT] == null ? "" : articlesInDb.ResultSet[1].Rows[j].Values[indexVAT].ToString()
                            );
                        }
                        bool result = false;
                        _threadExport = new Thread(() => result = ExportExcel(importFromDBdataTable, path, true, pSourceWindow));
                        Utils.ThreadStart(pSourceWindow, _threadExport, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_export_articles"));
                        if (result)
                        {
                            Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, MessageType.Info, ButtonsType.Close, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_message_operation_successfully"), CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_message_exported_successfully"));
                        }
                        else
                        {
                            Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, MessageType.Error, ButtonsType.Close, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_error"), "Empty Database");
                        }
                        break;

                    case ImportExportFileOpen.ExportCustomers:

                        var customersInDb = XPOSettings.Session.ExecuteQueryWithMetadata(queryCostumerString);

                        for (int w = 0; w < customersInDb.ResultSet[0].Rows.Length; w++)
                        {
                            if (customersInDb.ResultSet[0].Rows[w].Values[0].Equals("Code"))
                            {
                                importFromDBdataTable.Columns.Add(customersInDb.ResultSet[0].Rows[w].Values[0].ToString(), typeof(string));
                                indexCode = w;
                            }
                            if (customersInDb.ResultSet[0].Rows[w].Values[0].Equals("FiscalNumber"))
                            {
                                importFromDBdataTable.Columns.Add(customersInDb.ResultSet[0].Rows[w].Values[0].ToString(), typeof(string));
                                indexFiscalNumber = w;
                            }
                            if (customersInDb.ResultSet[0].Rows[w].Values[0].Equals("Name"))
                            {
                                importFromDBdataTable.Columns.Add(customersInDb.ResultSet[0].Rows[w].Values[0].ToString(), typeof(string));
                                indexName = w;
                            }
                            if (customersInDb.ResultSet[0].Rows[w].Values[0].Equals("Address"))
                            {
                                importFromDBdataTable.Columns.Add(customersInDb.ResultSet[0].Rows[w].Values[0].ToString(), typeof(string));
                                indexAddress = w;
                            }
                            if (customersInDb.ResultSet[0].Rows[w].Values[0].Equals("Locality"))
                            {
                                importFromDBdataTable.Columns.Add(customersInDb.ResultSet[0].Rows[w].Values[0].ToString(), typeof(string));
                                indexLocality = w;
                            }
                            if (customersInDb.ResultSet[0].Rows[w].Values[0].Equals("ZipCode"))
                            {
                                importFromDBdataTable.Columns.Add(customersInDb.ResultSet[0].Rows[w].Values[0].ToString(), typeof(string));
                                indexZipCode = w;
                            }
                            if (customersInDb.ResultSet[0].Rows[w].Values[0].Equals("City"))
                            {
                                importFromDBdataTable.Columns.Add(customersInDb.ResultSet[0].Rows[w].Values[0].ToString(), typeof(string));
                                indexCity = w;
                            }
                            if (customersInDb.ResultSet[0].Rows[w].Values[0].Equals("Phone"))
                            {
                                importFromDBdataTable.Columns.Add(customersInDb.ResultSet[0].Rows[w].Values[0].ToString(), typeof(string));
                                indexPhone = w;
                            }
                            if (customersInDb.ResultSet[0].Rows[w].Values[0].Equals("MobilePhone"))
                            {
                                importFromDBdataTable.Columns.Add(customersInDb.ResultSet[0].Rows[w].Values[0].ToString(), typeof(string));
                                indexMobilePhone = w;
                            }
                            if (customersInDb.ResultSet[0].Rows[w].Values[0].Equals("Email"))
                            {
                                importFromDBdataTable.Columns.Add(customersInDb.ResultSet[0].Rows[w].Values[0].ToString(), typeof(string));
                                indexEmail = w;
                            }

                        }
                        for (int j = 2; j < customersInDb.ResultSet[1].Rows.Length; j++)
                        {
                            importFromDBdataTable.Rows.Add(customersInDb.ResultSet[1].Rows[j].Values[indexCode],
                            customersInDb.ResultSet[1].Rows[j].Values[indexFiscalNumber] == null ? "" : CryptographyUtils.Decrypt(customersInDb.ResultSet[1].Rows[j].Values[indexFiscalNumber].ToString(), true, PluginSettings.SecretKey),
                            customersInDb.ResultSet[1].Rows[j].Values[indexName] == null ? "" : CryptographyUtils.Decrypt(customersInDb.ResultSet[1].Rows[j].Values[indexName].ToString(), true, PluginSettings.SecretKey),
                            customersInDb.ResultSet[1].Rows[j].Values[indexAddress] == null ? "" : CryptographyUtils.Decrypt(customersInDb.ResultSet[1].Rows[j].Values[indexAddress].ToString(), true, PluginSettings.SecretKey),
                            customersInDb.ResultSet[1].Rows[j].Values[indexLocality] == null ? "" : CryptographyUtils.Decrypt(customersInDb.ResultSet[1].Rows[j].Values[indexLocality].ToString(), true, PluginSettings.SecretKey),
                            customersInDb.ResultSet[1].Rows[j].Values[indexZipCode] == null ? "" : CryptographyUtils.Decrypt(customersInDb.ResultSet[1].Rows[j].Values[indexZipCode].ToString(), true, PluginSettings.SecretKey),
                            customersInDb.ResultSet[1].Rows[j].Values[indexCity] == null ? "" : CryptographyUtils.Decrypt(customersInDb.ResultSet[1].Rows[j].Values[indexCity].ToString(), true, PluginSettings.SecretKey),
                            customersInDb.ResultSet[1].Rows[j].Values[indexPhone] == null ? "" : CryptographyUtils.Decrypt(customersInDb.ResultSet[1].Rows[j].Values[indexPhone].ToString(), true, PluginSettings.SecretKey),
                            customersInDb.ResultSet[1].Rows[j].Values[indexMobilePhone] == null ? "" : CryptographyUtils.Decrypt(customersInDb.ResultSet[1].Rows[j].Values[indexMobilePhone].ToString(), true, PluginSettings.SecretKey),
                            customersInDb.ResultSet[1].Rows[j].Values[indexEmail] == null ? "" : CryptographyUtils.Decrypt(customersInDb.ResultSet[1].Rows[j].Values[indexEmail].ToString(), true, PluginSettings.SecretKey));
                        }
                        result = false;
                        _threadExport = new Thread(() => result = ExportExcel(importFromDBdataTable, path, true, pSourceWindow));
                        Utils.ThreadStart(pSourceWindow, _threadExport, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_export_customers"));
                        Utils.ThreadStart(pSourceWindow, _threadExport, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_export_articles"));
                        if (result)
                        {
                            Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, MessageType.Info, ButtonsType.Close, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_message_operation_successfully"), CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_message_exported_successfully"));
                        }
                        else
                        {
                            Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, MessageType.Error, ButtonsType.Close, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_error"), "Empty Database");
                        }
                        break;

                    default:
                        break;

                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, MessageType.Error, ButtonsType.Close, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_error"), CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_message_export_error"));
            }

        }


        public static bool ExportExcel(DataTable dtExport, string path, bool openFile, Gtk.Window pSourceWindow)
        {
            try
            {
                if (dtExport != null && dtExport.Rows.Count > 0)
                {

                    _logger.Debug("FileName: " + path);

                    XLWorkbook wb = new XLWorkbook();
                    wb.Worksheets.Add(dtExport, "1");
                    wb.SaveAs(path);

                    if (openFile)
                    {
                        System.Diagnostics.Process.Start(path);
                    }
                    return true;
                }
                else
                {
                    _logger.Debug("ExportExcel: DataTable has no rows to export");
                    return false;
                }

            }
            catch (Exception ex)
            {
                _logger.Error("ExportExcel: Error creating file " + ex.Message, ex);
                return false;
            }
            finally
            {
                GlobalApp.DialogThreadNotify.WakeupMain();
            }
        }
    }
}
