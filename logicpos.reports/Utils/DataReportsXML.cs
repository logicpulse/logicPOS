using DevExpress.Xpo.DB;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.reports.App;
using logicpos.reports.Resources.Localization;
using Mono.Data.Sqlite;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Xml.Linq;

namespace logicpos.reports.Utils
{
    public class DataReportsXML
    {
        private static DataSet dsGetDataSet;
        private static SqlDataAdapter dtsqlserver;
        private static MySqlDataAdapter dtmysql;
        private static SqliteDataAdapter dtsqlite;
        private static DataSet dsXMLReports;
        private static SqlDataAdapter dtsqlserverXMLReports;
        private static MySqlDataAdapter dtmysqlXMLReports;
        private static SqliteDataAdapter dtsqliteXMLReports;
        private static string _currency;
        private static DataSet FDataSet = new DataSet();
        private static string FReportsFolder;
        private static string xpoOidDocumentFinanceTypeCurrentAccountInput = GlobalFramework.Settings["xpoOidDocumentFinanceTypeCurrentAccountInput"];

        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public static DataTable GetParameters()
        {
            DataTable table = new DataTable();
            string nameTable = "";

            string strSql = @"SELECT Value, Token FROM configurationpreferenceparameter WHERE Token = 'REPORT_FOOTER_LINE1' 
                              union
                              SELECT Value, Token FROM configurationpreferenceparameter WHERE Token = 'REPORT_FOOTER_LINE2'
                              union
                              SELECT Value, Token FROM configurationpreferenceparameter WHERE Token = 'REPORT_FILENAME_LOGO' order by token";

            strSql = string.Format(strSql);

            XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(strSql);

            table.Columns.Add("Parameters_Report");
            try
            {
                foreach (SelectStatementResultRow parentRow in xPSelectData.Data)
                {
                    if (parentRow.Values[0] == null)
                    {
                        nameTable = "NULL";
                    }
                    else
                    {
                        nameTable = parentRow.Values[0].ToString();
                    }
                    table.Rows.Add(nameTable);

                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetPayment(e.Message): {0}", ex.Message), ex);

            }

            return table;
        }


        public static void GetParametersReport()
        {
            FReportsFolder = (string.Format("{0}", GlobalFramework.Path["reports"]));

            DataTable dataReport = new DataTable();

            dataReport = GetParameters();

            string[] filePaths = Directory.GetFiles(FReportsFolder, "*.frx");
            string base64String = null;
            try
            {
                
                for (int i = 0; i < filePaths.Length; i++)
                {
                    string fileReport = filePaths[i].ToString();

                    XDocument doc = XDocument.Load(fileReport);
                    IEnumerable<XElement> elements = doc.Elements("Report");

                    foreach (XElement node in elements)
                    {
                        XElement attachment;

                        if (dataReport.Rows[0].ItemArray[0].ToString() != "NULL")
                        {
                            if (node.Element("ReportPage").Element("ReportTitleBand").Element("PictureObject") != null)
                            {
                                attachment = node.Element("ReportPage").Element("ReportTitleBand");

                                foreach (XElement child in attachment.Elements("PictureObject"))
                                {
                                    if (child.Attribute("Name").Value.Equals("Picture1"))
                                    {

                                        Bitmap picture = new Bitmap(new MemoryStream(File.ReadAllBytes(dataReport.Rows[0].ItemArray[0].ToString())));
                                        // Picture1.SizeMode = PictureBoxSizeMode.AutoSize;

                                        using (MemoryStream ms = new MemoryStream())
                                        {
                                            picture.Save(ms, ImageFormat.Png);
                                            byte[] imageBytes = ms.ToArray();
                                            base64String = Convert.ToBase64String(imageBytes);

                                        }

                                        child.Attribute("Image").SetValue(base64String);


                                    }
                                    doc.Save(fileReport);

                                }
                                doc.Save(fileReport);
                            }
                        }
                        if (dataReport.Rows[1].ItemArray[0].ToString() != "NULL")
                        {
                            if (node.Element("ReportPage").Element("PageFooterBand") != null)
                            {
                                attachment = node.Element("ReportPage").Element("PageFooterBand");

                                foreach (XElement child in attachment.Elements("TextObject"))
                                {
                                    if (child.Attribute("Name").Value.Equals("TxtLine1"))
                                    {

                                        child.Attribute("Text").SetValue(dataReport.Rows[1].ItemArray[0].ToString());

                                    }
                                }
                                doc.Save(fileReport);
                            }

                        }
                        if (dataReport.Rows[2].ItemArray[0].ToString() != "NULL")
                        {
                            if (node.Element("ReportPage").Element("PageFooterBand") != null)
                            {
                                attachment = node.Element("ReportPage").Element("PageFooterBand");

                                foreach (XElement child in attachment.Elements("TextObject"))
                                {
                                    if (child.Attribute("Name").Value.Equals("TxtLine2"))
                                    {

                                        child.Attribute("Text").SetValue(dataReport.Rows[2].ItemArray[0].ToString());

                                    }
                                }
                                doc.Save(fileReport);
                            }

                        }

                    }
                }

            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetParametersReport(e.Message): {0}", ex.Message), ex);

            }

        }

        public static void GetCurrency()
        {
            _currency = GlobalFramework.Settings["CURRENCY_SYMBOL"];

            FReportsFolder = (string.Format("{0}", GlobalFramework.Path["reports"]));

            string[] filePaths = Directory.GetFiles(FReportsFolder, "*.frx");

            try
            {
                for (int i = 0; i < filePaths.Length; i++)
                {
                    string fileReport = filePaths[i].ToString();

                    XDocument doc = XDocument.Load(fileReport);
                    IEnumerable<XElement> elements = doc.Elements("Report");

                    foreach (XElement node in elements)
                    {
                        XElement attachment;

                        if (node.Element("ReportPage").Element("GroupHeaderBand") == null &&
                            node.Element("ReportPage").Element("DataBand").Element("DataFooterBand") != null)
                        {
                            if (node.Element("ReportPage").Element("DataBand") != null)
                            {
                                attachment = node.Element("ReportPage").Element("DataBand");

                                foreach (XElement child in attachment.Elements("TextObject"))
                                {
                                    if (child.Attribute("Format.CurrencySymbol") != null)
                                    {
                                        child.Attribute("Format.CurrencySymbol").SetValue(_currency);

                                    }

                                }

                            }
                            if (node.Element("ReportPage").Element("DataBand").Element("DataFooterBand") != null)
                            {
                                attachment = node.Element("ReportPage").Element("DataBand").Element("DataFooterBand");

                                foreach (XElement child in attachment.Elements("TextObject"))
                                {
                                    if (child.Attribute("Format.CurrencySymbol") != null)
                                    {
                                        child.Attribute("Format.CurrencySymbol").SetValue(_currency);

                                    }

                                }
                            }
                            doc.Save(fileReport);

                        }
                        if (fileReport != "Reports/Conta_corrente_clientes_Detalhado.frx" )
                        {
                            if (fileReport != "Reports/Stock_artigos.frx")
                            {
                                if (node.Element("ReportPage").Element("DataBand") == null && node.Element("ReportPage").Element("GroupHeaderBand").Element("DataBand").Element("DataFooterBand") != null
                                    || node.Element("ReportPage").Element("DataBand") == null && node.Element("ReportPage").Element("GroupHeaderBand").Element("DataBand").Element("DataHeaderBand") != null)
                                {
                                    if (node.Element("ReportPage").Element("GroupHeaderBand").Element("DataBand").Element("DataFooterBand") != null)
                                    {

                                        attachment = node.Element("ReportPage").Element("GroupHeaderBand").Element("DataBand").Element("DataFooterBand");
                                    }
                                    else
                                    {
                                        attachment = node.Element("ReportPage").Element("GroupHeaderBand").Element("DataBand").Element("DataHeaderBand");

                                    }
                                    foreach (XElement child in attachment.Elements("TextObject"))
                                    {
                                        if (child.Attribute("Format.CurrencySymbol") != null)
                                        {
                                            child.Attribute("Format.CurrencySymbol").SetValue(_currency);

                                        }

                                    }

                                }
                            }

                        }
                        else
                        {
                            if (node.Element("ReportPage").Element("DataBand") == null && node.Element("ReportPage").Element("GroupHeaderBand").Element("GroupHeaderBand").Element("DataBand") != null)
                            {
                                if (node.Element("ReportPage").Element("GroupHeaderBand").Element("GroupHeaderBand").Element("GroupFooterBand") != null)
                                {
                                    attachment = node.Element("ReportPage").Element("GroupHeaderBand").Element("GroupHeaderBand").Element("GroupFooterBand");

                                    foreach (XElement child in attachment.Elements("TextObject"))
                                    {
                                        if (child.Attribute("Format.CurrencySymbol") != null)
                                        {
                                            child.Attribute("Format.CurrencySymbol").SetValue(_currency);

                                        }

                                    }

                                }

                                if (node.Element("ReportPage").Element("GroupHeaderBand").Element("GroupFooterBand") != null)
                                {
                                    attachment = node.Element("ReportPage").Element("GroupHeaderBand").Element("GroupFooterBand");

                                    foreach (XElement child in attachment.Elements("TextObject"))
                                    {
                                        if (child.Attribute("Format.CurrencySymbol") != null)
                                        {
                                            child.Attribute("Format.CurrencySymbol").SetValue(_currency);

                                        }

                                    }

                                }
                                doc.Save(fileReport);

                            }
                            doc.Save(fileReport);
                        }

                        if (node.Element("ReportPage").Element("GroupHeaderBand") != null)
                        {
                            if (node.Element("ReportPage").Element("DataBand") == null && node.Element("ReportPage").Element("GroupHeaderBand").Element("GroupFooterBand") != null
                                || node.Element("ReportPage").Element("DataBand") == null && node.Element("ReportPage").Element("GroupHeaderBand").Element("DataBand") != null)
                            {
                                if (node.Element("ReportPage").Element("GroupHeaderBand").Element("GroupFooterBand") != null)
                                {
                                    attachment = node.Element("ReportPage").Element("GroupHeaderBand").Element("GroupFooterBand");

                                    foreach (XElement child in attachment.Elements("TextObject"))
                                    {
                                        if (child.Attribute("Format.CurrencySymbol") != null)
                                        {
                                            child.Attribute("Format.CurrencySymbol").SetValue(_currency);

                                        }

                                    }


                                }
                                if (node.Element("ReportPage").Element("GroupHeaderBand").Element("DataFooterBand") != null)
                                {
                                    attachment = node.Element("ReportPage").Element("GroupHeaderBand").Element("DataFooterBand");

                                    foreach (XElement child in attachment.Elements("TextObject"))
                                    {
                                        if (child.Attribute("Format.CurrencySymbol") != null)
                                        {
                                            child.Attribute("Format.CurrencySymbol").SetValue(_currency);

                                        }

                                    }


                                }
                                if (node.Element("ReportPage").Element("GroupHeaderBand").Element("DataBand") != null)
                                {
                                    if (node.Element("ReportPage").Element("GroupHeaderBand").Element("DataBand").Element("DataFooterBand") != null)
                                    {
                                        attachment = node.Element("ReportPage").Element("GroupHeaderBand").Element("DataBand").Element("DataFooterBand");

                                        foreach (XElement child in attachment.Elements("TextObject"))
                                        {
                                            if (child.Attribute("Format.CurrencySymbol") != null)
                                            {
                                                child.Attribute("Format.CurrencySymbol").SetValue(_currency);

                                            }

                                        }
                                    }


                                }
                                if (node.Element("ReportPage").Element("GroupHeaderBand").Element("DataBand") != null)
                                {
                                    attachment = node.Element("ReportPage").Element("GroupHeaderBand").Element("DataBand");

                                    foreach (XElement child in attachment.Elements("TextObject"))
                                    {
                                        if (child.Attribute("Format.CurrencySymbol") != null)
                                        {
                                            child.Attribute("Format.CurrencySymbol").SetValue(_currency);

                                        }

                                    }

                                }

                                doc.Save(fileReport);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetCurrency(e.Message): {0}", ex.Message), ex);

            }

        }


        public static void ConfigureXMLReports()
        {
            string con = GlobalFramework.SessionXpo.ConnectionString.ToString();

            dsXMLReports = new DataSet();

            File.Delete(string.Format("{0}{1}", GlobalFramework.Path["reports"], "sales.xml"));


            string nameTable = String.Empty, parentLabel = String.Empty, parentResource = String.Empty;

            XPSelectData xPSelectData = Utils.GetAllNameTable();

            try
            {
                foreach (SelectStatementResultRow parentRow in xPSelectData.Data)
                {
                    nameTable = parentRow.Values[xPSelectData.GetFieldIndex("TABLE_NAME")].ToString();

                    // string executeSqlArticle = string.Format("SELECT * FROM {0} where 1 = 0 ", nameTable);
                    string executeSqlArticle = string.Format("SELECT * FROM {0}", nameTable);
                    executeSqlArticle.ToLower();


                    if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                    {
                        dtsqlserverXMLReports = new SqlDataAdapter(executeSqlArticle, con);
                        dtsqlserverXMLReports.Fill(dsXMLReports, nameTable);

                    }
                    else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                    {
                        dtmysqlXMLReports = new MySqlDataAdapter(executeSqlArticle, con);
                        dtmysqlXMLReports.Fill(dsXMLReports, nameTable);
                    }
                    else
                    {
                        dtsqliteXMLReports = new SqliteDataAdapter(executeSqlArticle, con);
                        dtsqliteXMLReports.Fill(dsXMLReports, nameTable);
                    }

                    
                    for (int i = 0; i < dsXMLReports.Tables.Count; i++)
                    {
                        dsXMLReports.Tables[i].TableName = dsXMLReports.Tables[i].TableName.ToLower();
                        
                        for (int j = 0; j < dsXMLReports.Tables[i].Columns.Count; j++)
                        {
                            dsXMLReports.Tables[i].Columns[j].ColumnName = dsXMLReports.Tables[i].Columns[j].ColumnName.ToLower();
                        }

                    }


                }

            }
            catch (Exception ex)
            {
                _log.Error(string.Format("ConfigureXMLReports(e.Message): {0}", ex.Message), ex);

            }

            string thisFolder = (string.Format("{0}", GlobalFramework.Path["reports"]));

            string folderpath_sqlserver = Path.GetFullPath(thisFolder) + "relationshipTables.txt";
            string folderpath_mysql = Path.GetFullPath(thisFolder) + "relationshipTables_MySQL.txt";

            //string[] tmp_sqlserver = File.ReadAllLines(folderpath_sqlserver);
            //string[] tmp_mysql = File.ReadAllLines(folderpath_mysql);
            string[] tmp_sql = File.ReadAllLines(folderpath_sqlserver);


            try
            {
                foreach (string line in tmp_sql)
                {
                    tmp_sql = line.Split('\t');

                    string foreignKey = tmp_sql[0].ToLower();
                    string tableName = tmp_sql[1].ToLower();
                    string columnName = tmp_sql[2].ToLower();
                    string referenceTable = tmp_sql[3].ToLower();
                    string referenceColumnName = tmp_sql[4].ToLower();

                    dsXMLReports.Relations.Add(new DataRelation(foreignKey,
                            dsXMLReports.Tables[referenceTable].Columns[referenceColumnName],
                           dsXMLReports.Tables[tableName].Columns[columnName], false));

                }

            }
            catch (Exception ex)
            {
                _log.Error(string.Format("ConfigureXMLReports_relationshipTables(e.Message): {0}", ex.Message), ex);

            }


            //           IMPORTANT! SELECT ForeignKey BETWEENN TABLES - SQL SERVER
//            relationship = @"SELECT f.name AS ForeignKey, 
//                                                OBJECT_NAME(f.parent_object_id) AS TableName, 
//                                                COL_NAME(fc.parent_object_id, fc.parent_column_id) AS ColumnName, 
//                                                OBJECT_NAME (f.referenced_object_id) AS ReferenceTableName, 
//                                                COL_NAME(fc.referenced_object_id, fc.referenced_column_id) AS ReferenceColumnName 
//                                                FROM sys.foreign_keys AS f 
//                                                INNER JOIN sys.foreign_key_columns AS fc 
//                                                ON f.OBJECT_ID = fc.constraint_object_id";


            // IMPORTANT! SELECT ForeignKey BETWEENN TABLES - MYSQL
            //SELECT f.ID AS foreignKey,
            //        f.for_name AS TableName,
            //        fc.for_col_name as ColumnName,
            //        f.ref_name as ReferenceTableName,
            //        fc.ref_col_name as ReferenceColumnName
            //        FROM information_schema.INNODB_SYS_FOREIGN as f
            //        inner join information_schema.INNODB_SYS_FOREIGN_COLS as fc
            //       on f.ID = fc.ID;

            
            dsXMLReports.WriteXml((string.Format("{0}{1}", GlobalFramework.Path["reports"], "sales.xml")), XmlWriteMode.WriteSchema);

            _log.Debug("dsXMLReports.WriteXml");

        }


      


        public static DataSet GetDataSystemAudit(List<string> oidSystemAudit, string _startDate, string _endDate)
        {
            string con = GlobalFramework.SessionXpo.ConnectionString.ToString();
            string executeSqlData = "";
            List<string> dates = new List<string>();

            dsGetDataSet = new DataSet();

            dates.Add(_startDate);
            dates.Add(_endDate);
            // dates = Data.SplitDate(_startDate, _endDate);

            try
            {
                List<string> Tables = new List<string>();

                Tables.Add("systemaudit");
                Tables.Add("systemaudittype");
                Tables.Add("configurationplaceterminal");
                Tables.Add("userdetail");

                for (int j = 0; j < Tables.Count; j++)
                {
                    executeSqlData = string.Format("SELECT * FROM {0}", Tables[j]);

                    if (Tables[j].Equals("systemaudit"))
                    {
                        executeSqlData = string.Format("SELECT * FROM {0}", Tables[j]);

                        for (int i = 0; i < oidSystemAudit.Count; i++)
                        {

                           
                                executeSqlData = string.Format(@"SELECT * FROM {0} f where f.Oid = '{1}' and CAST(Date as DATE) between '{2}' and '{3}'", Tables[j], oidSystemAudit[i], dates[0], dates[1]);
                         

                            if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                            {
                                dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                                dtsqlserver.Fill(dsGetDataSet, Tables[j]);
                            }
                            else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                            {
                                dtmysql = new MySqlDataAdapter(executeSqlData, con);
                                dtmysql.Fill(dsGetDataSet, Tables[j]);
                            }
                            else
                            {
                                dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                                dtsqlite.Fill(dsGetDataSet, Tables[j]);
                            }
                        }
                    }
                    else
                    {

                        if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                        {
                            dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                            dtsqlserver.Fill(dsGetDataSet, Tables[j]);
                        }
                        else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                        {
                            dtmysql = new MySqlDataAdapter(executeSqlData, con);
                            dtmysql.Fill(dsGetDataSet, Tables[j]);
                        }
                        else
                        {
                            dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                            dtsqlite.Fill(dsGetDataSet, Tables[j]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetDataSystemAudit(e.Message): {0}", ex.Message), ex);

            }
  
            _log.Debug("dsGetDataSystemAudit.WriteXml");

            return dsGetDataSet;
        }


        public static DataSet GetDataDocumentOrderDetail(List<string> oidDocumentFinancialDetail, string _startDate, string _endDate)
        {
            string con = GlobalFramework.SessionXpo.ConnectionString.ToString();
            string executeSqlData = "";
            List<string> dates = new List<string>();

            dsGetDataSet = new DataSet();

            dates.Add(_startDate);
            dates.Add(_endDate);
            // dates = Data.SplitDate(_startDate, _endDate);

            try
            {
                List<string> Tables = new List<string>();

                Tables.Add("documentfinancedetail");
                Tables.Add("documentfinancemaster");
                Tables.Add("article");
                Tables.Add("articlesubfamily");
                Tables.Add("articlefamily");
                Tables.Add("configurationplaceterminal");
                Tables.Add("userdetail");

                for (int j = 0; j < Tables.Count; j++)
                {
                    executeSqlData = string.Format("SELECT * FROM {0}", Tables[j]);

                    if (Tables[j].Equals("documentfinancedetail"))
                    {

                        executeSqlData = string.Format("SELECT * FROM {0}", Tables[j]);

                        for (int i = 0; i < oidDocumentFinancialDetail.Count; i++)
                        {
                            executeSqlData = string.Format(@"SELECT * FROM {0} f where f.Oid = '{1}' and CAST(CreatedAt as DATE) between '{2}' and '{3}'", Tables[j], oidDocumentFinancialDetail[i], dates[0], dates[1]);
                           
                            if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                            {
                                dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                                dtsqlserver.Fill(dsGetDataSet, Tables[j]);
                            }
                            else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                            {
                                dtmysql = new MySqlDataAdapter(executeSqlData, con);
                                dtmysql.Fill(dsGetDataSet, Tables[j]);
                            }
                            else
                            {
                                dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                                dtsqlite.Fill(dsGetDataSet, Tables[j]);
                            }
                        }
                    }
                    else
                    {
                        if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                        {
                            dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                            dtsqlserver.Fill(dsGetDataSet, Tables[j]);
                        }
                        else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                        {
                            dtmysql = new MySqlDataAdapter(executeSqlData, con);
                            dtmysql.Fill(dsGetDataSet, Tables[j]);
                        }
                        else
                        {
                            dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                            dtsqlite.Fill(dsGetDataSet, Tables[j]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetDataDocumentOrderDetail(e.Message): {0}", ex.Message), ex);
            }

            _log.Debug("dsGetDocumentFinanceDetail.WriteXml");
            return dsGetDataSet;
        }

        public static DataSet GetDataDocumenFinanceDetail(List<string> oidDocumentFinancialDetail, string _startDate, string _endDate)
        {
            string con = GlobalFramework.SessionXpo.ConnectionString.ToString();
            string executeSqlData = "";
            List<string> dates = new List<string>();

            dsGetDataSet = new DataSet();

            dates.Add(_startDate);
            dates.Add(_endDate);
            // dates = Data.SplitDate(_startDate, _endDate);

            try
            {
                List<string> Tables = new List<string>();

                Tables.Add("documentfinancedetail");
                Tables.Add("documentfinancemaster");
                Tables.Add("userdetail");

                for (int j = 0; j < Tables.Count; j++)
                {
                    executeSqlData = string.Format("SELECT * FROM {0}", Tables[j]);

                    if (Tables[j].Equals("documentfinancemaster"))
                    {

                        executeSqlData = string.Format("SELECT * FROM {0}", Tables[j]);

                        for (int i = 0; i < oidDocumentFinancialDetail.Count; i++)
                        {
                           
                                executeSqlData = string.Format(@"SELECT * FROM {0} f where f.Oid = '{1}' and CAST(CreatedAt as DATE) between '{2}' and '{3}'", Tables[j], oidDocumentFinancialDetail[i], dates[0], dates[1]);
                           
                            if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                            {
                                dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                                dtsqlserver.Fill(dsGetDataSet, Tables[j]);
                            }
                            else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                            {
                                dtmysql = new MySqlDataAdapter(executeSqlData, con);
                                dtmysql.Fill(dsGetDataSet, Tables[j]);
                            }
                            else
                            {
                                dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                                dtsqlite.Fill(dsGetDataSet, Tables[j]);
                            }
                        }
                    }
                    else
                    {
                        if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                        {
                            dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                            dtsqlserver.Fill(dsGetDataSet, Tables[j]);
                        }
                        else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                        {
                            dtmysql = new MySqlDataAdapter(executeSqlData, con);
                            dtmysql.Fill(dsGetDataSet, Tables[j]);
                        }
                        else
                        {
                            dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                            dtsqlite.Fill(dsGetDataSet, Tables[j]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetDataDocumentFinanceMaster(e.Message): {0}", ex.Message), ex);

            }
            _log.Debug("dsGetDataDocumentFinanceMaster.WriteXml");
            return dsGetDataSet;
        }


        public static DataSet GetDataDocumentFinanceDetail_Clients(List<string> oidDocumentFinancialDetail, string _startDate, string _endDate)
        {
            string con = GlobalFramework.SessionXpo.ConnectionString.ToString();
            string executeSqlData = "";
            List<string> dates = new List<string>();

            dsGetDataSet = new DataSet();

            dates.Add(_startDate);
            dates.Add(_endDate);
            // dates = Data.SplitDate(_startDate, _endDate);

            try
            {
                List<string> Tables = new List<string>();

                Tables.Add("documentfinancemaster");
                Tables.Add("documentfinancedetail");

                for (int j = 0; j < Tables.Count; j++)
                {
                    executeSqlData = string.Format("SELECT * FROM {0}", Tables[j]);

                    if (Tables[j].Equals("documentfinancemaster"))
                    {
                        executeSqlData = string.Format("SELECT * FROM {0}", Tables[j]);

                        for (int i = 0; i < oidDocumentFinancialDetail.Count; i++)
                        {

                            executeSqlData = string.Format(@"select m.Oid, m.DocumentNumber, m.EntityName, m.Date, m.Payed, m.PayedDate 
                                        from DocumentFinanceMaster m where
                                        CAST(m.DocumentDate as DATE) between '{0}' and '{1}'", dates[0], dates[1]);

                            if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                            {
                                dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                                dtsqlserver.Fill(dsGetDataSet, Tables[j]);
                            }
                            else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                            {
                                dtmysql = new MySqlDataAdapter(executeSqlData, con);
                                dtmysql.Fill(dsGetDataSet, Tables[j]);
                            }
                            else
                            {
                                dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                                dtsqlite.Fill(dsGetDataSet, Tables[j]);
                            }
                        }
                    }
                    if (Tables[j].Equals("documentfinancedetail"))
                    {
                        executeSqlData = string.Format("SELECT * FROM {0}", Tables[j]);

                        for (int i = 0; i < oidDocumentFinancialDetail.Count; i++)
                        {
                            executeSqlData = string.Format(@"select d.Oid, d.Code, d.Designation, d.Quantity, d.TotalFinal, d.DocumentMaster, d.Article, d.UpdatedWhere, d.UpdatedBy
                                        from DocumentFinanceDetail d where CAST(d.CreatedAt as DATE) between '{0}' and '{1}' and  d.Oid = '{2}'", dates[0], dates[1], oidDocumentFinancialDetail[i]);


                            if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                            {
                                dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                                dtsqlserver.Fill(dsGetDataSet, Tables[j]);
                            }
                            else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                            {
                                dtmysql = new MySqlDataAdapter(executeSqlData, con);
                                dtmysql.Fill(dsGetDataSet, Tables[j]);
                            }
                            else
                            {
                                dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                                dtsqlite.Fill(dsGetDataSet, Tables[j]);
                            }
                        }
                    }
                    else
                    {
                        if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                        {
                            dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                            dtsqlserver.Fill(dsGetDataSet, Tables[j]);
                        }
                        else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                        {
                            dtmysql = new MySqlDataAdapter(executeSqlData, con);
                            dtmysql.Fill(dsGetDataSet, Tables[j]);
                        }
                        else
                        {
                            dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                            dtsqlite.Fill(dsGetDataSet, Tables[j]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetDataDocumentFinanceMaster(e.Message): {0}", ex.Message), ex);

            }
            _log.Debug("dsGetDataDocumentFinanceMaster.WriteXml");
            dsGetDataSet.WriteXml((string.Format("{0}{1}", GlobalFramework.Path["reports"], "DATA.xml")), XmlWriteMode.WriteSchema);

            return dsGetDataSet;
        }

        public static void GetDataDocumentFinanceDetail(List<string> oidDocumentFinanceDetail, string _startDate, string _endDate)
        {
            string con = GlobalFramework.SessionXpo.ConnectionString.ToString();
            string executeSqlData = "";
            List<string> dates = new List<string>();

            dsGetDataSet = new DataSet();

            dates.Add(_startDate);
            dates.Add(_endDate);
            // dates = Data.SplitDate(_startDate, _endDate);

            try
            {
                string nameTable = "DocumentFinanceMaster";
                executeSqlData = string.Format("SELECT * FROM {0}", nameTable);

                if (nameTable.ToLower().Equals("documentfinancedetail"))
                {
                    for (int i = 0; i < oidDocumentFinanceDetail.Count; i++)
                    {
                       
                            executeSqlData = string.Format(@"SELECT * FROM {0} where Oid = '{1}' and CAST(CreatedAt as DATE) between '{2}' and '{3}'", nameTable, oidDocumentFinanceDetail[i], dates[0], dates[1]);
                        
                     

                        if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                        {
                            dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                            dtsqlserver.Fill(dsGetDataSet, nameTable);
                        }
                        else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                        {
                            dtmysql = new MySqlDataAdapter(executeSqlData, con);
                            dtmysql.Fill(dsGetDataSet, nameTable);
                        }
                        else
                        {
                            dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                            dtsqlite.Fill(dsGetDataSet, nameTable);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetDataDocumentFinanceDetail(e.Message): {0}", ex.Message), ex);

            }

            string thisFolder = (string.Format("{0}", GlobalFramework.Path["reports"]));

            // string folderpath_sqlserver = Path.GetFullPath(thisFolder) + "relationshipTables.txt";
            string folderpath_sql = Path.GetFullPath(thisFolder) + "relationshipTables_MySQL.txt";

            // string[] tmp_sqlserver = File.ReadAllLines(folderpath_sqlserver);
            string[] tmp_sql = File.ReadAllLines(folderpath_sql);

            try
            {

                foreach (string line in tmp_sql)
                {
                    tmp_sql = line.Split('\t');

                    string foreignKey = tmp_sql[0];
                    string tableName = tmp_sql[1];
                    string columnName = tmp_sql[2];
                    string referenceTable = tmp_sql[3];
                    string referenceColumnName = tmp_sql[4];

                    dsGetDataSet.Relations.Add(new DataRelation(foreignKey,
                            dsGetDataSet.Tables[referenceTable].Columns[referenceColumnName],
                           dsGetDataSet.Tables[tableName].Columns[columnName], false));
                }

            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetDataDocumentFinanceDetail_relationshipTables(e.Message): {0}", ex.Message), ex);

            }

            dsGetDataSet.WriteXml((string.Format("{0}{1}", GlobalFramework.Path["reports"], "DocumentFinanceDetail.xml")), XmlWriteMode.WriteSchema);

            _log.Debug("dsGetDocumentFinancialDetail.WriteXml");

        }



        public static DataSet GetDataDocumentFinanceMaster(List<string> oidDocumentFinanceMaster, string _startDate, string _endDate)
        {
            string con = GlobalFramework.SessionXpo.ConnectionString.ToString();
            string executeSqlData = "";
            List<string> dates = new List<string>();
            
            dsGetDataSet = new DataSet();

            dates.Add(_startDate);
            dates.Add(_endDate);
            // dates = Data.SplitDate(_startDate, _endDate);

            try
            {
                List<string> Tables = new List<string>();

                Tables.Add("userdetail");
                Tables.Add("documentfinancemaster");
                Tables.Add("documentordermain");
                Tables.Add("configurationplaceterminal");
                Tables.Add("configurationpaymentmethod");
                Tables.Add("configurationplacetable");
                Tables.Add("configurationplace");
                Tables.Add("article");


                for (int j = 0; j < Tables.Count; j++)
                {
                    executeSqlData = string.Format("SELECT * FROM {0}", Tables[j]);

                    if (Tables[j].Equals("documentfinancemaster"))
                    {
                        for (int i = 0; i < oidDocumentFinanceMaster.Count; i++)
                        {
                           
                                executeSqlData = string.Format(@"SELECT * FROM {0} where Oid = '{1}' and CAST(Date as DATE) between '{2}' and '{3}'", Tables[j], oidDocumentFinanceMaster[i], dates[0], dates[1]);
                           

                            if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                            {
                                dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                                dtsqlserver.Fill(dsGetDataSet, Tables[j]);
                            }
                            else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                            {
                                dtmysql = new MySqlDataAdapter(executeSqlData, con);
                                dtmysql.Fill(dsGetDataSet, Tables[j]);
                            }
                            else
                            {
                                dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                                dtsqlite.Fill(dsGetDataSet, Tables[j]);
                            }
                        }
                    }
                    else
                    {
                        if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                        {
                            dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                            dtsqlserver.Fill(dsGetDataSet, Tables[j]);
                        }
                        else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                        {
                            dtmysql = new MySqlDataAdapter(executeSqlData, con);
                            dtmysql.Fill(dsGetDataSet, Tables[j]);
                        }
                        else
                        {
                            dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                            dtsqlite.Fill(dsGetDataSet, Tables[j]);
                        }
                    }

                    for (int o = 0; o < dsGetDataSet.Tables.Count; o++)
                    {
                        dsGetDataSet.Tables[o].TableName = dsGetDataSet.Tables[o].TableName.ToLower();
                        for (int e = 0; e < dsGetDataSet.Tables[o].Columns.Count; e++)
                        {
                            dsGetDataSet.Tables[o].Columns[e].ColumnName = dsGetDataSet.Tables[o].Columns[e].ColumnName.ToLower();
                        }



                    }

                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetDataDocumentFinanceMaster(e.Message): {0}", ex.Message), ex);

            }
            _log.Debug("dsGetDocumentFinancialMaster2.WriteXml");
          
            return dsGetDataSet;

        }


        public static DataSet GetDataWorkSessionMovement(List<string> oidWorkSessionMovement, string _startDate, string _endDate)
        {
            string con = GlobalFramework.SessionXpo.ConnectionString.ToString();
            string executeSqlData = "";

            List<string> dates = new List<string>();
            List<string> Tables = new List<string>();
            dsGetDataSet = new DataSet();

            dates.Add(_startDate);
            dates.Add(_endDate);
           // dates = Data.SplitDate(_startDate, _endDate);
       
            try
            {
                Tables.Add("worksessionmovement");
                Tables.Add("worksessionmovementtype");
                Tables.Add("worksessionperiod");
                Tables.Add("userdetail");
                Tables.Add("documentfinancemaster");
                Tables.Add("configurationpaymentmethod");
                Tables.Add("configurationplaceterminal");

                for (int j = 0; j < Tables.Count; j++)
                {
                    executeSqlData = string.Format("SELECT * FROM {0}", Tables[j]);

                    if (Tables[j].Equals("worksessionmovement"))
                    {
                        for (int i = 0; i < oidWorkSessionMovement.Count; i++)
                        {
                              executeSqlData = string.Format(@"SELECT * FROM {0} where Oid = '{1}' and CAST(Date as DATE) between '{2}' and '{3}'", Tables[j], oidWorkSessionMovement[i], dates[0], dates[1]);
                           

                            if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                            {
                                dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                                dtsqlserver.Fill(dsGetDataSet, Tables[j]);
                            }
                            else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                            {
                                dtmysql = new MySqlDataAdapter(executeSqlData, con);
                                dtmysql.Fill(dsGetDataSet, Tables[j]);
                            }
                            else
                            {
                                dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                                dtsqlite.Fill(dsGetDataSet, Tables[j]);
                            }
                        }
                    }
                    else
                    {
                        if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                        {
                            dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                            dtsqlserver.Fill(dsGetDataSet, Tables[j]);
                        }
                        else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                        {
                            dtmysql = new MySqlDataAdapter(executeSqlData, con);
                            dtmysql.Fill(dsGetDataSet, Tables[j]);
                        }
                        else
                        {
                            dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                            dtsqlite.Fill(dsGetDataSet, Tables[j]);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetDataWorkSessionMovement(e.Message): {0}", ex.Message), ex);

            }

            _log.Debug("dsGetWorkSessionMovement.WriteXml");

            return dsGetDataSet;
        }


        public static DataSet GetDataArticles(List<string> oidArticles)
        {
            string con = GlobalFramework.SessionXpo.ConnectionString.ToString();
            string executeSqlData = "";

            dsGetDataSet = new DataSet();

            List<string> Tables = new List<string>();

            try
            {
                Tables.Add("article");
                Tables.Add("articlesubfamily");
                Tables.Add("articlefamily");
                Tables.Add("articletype");

                for (int j = 0; j < Tables.Count; j++)
                {
                    executeSqlData = string.Format("SELECT * FROM {0}", Tables[j]);

                    if (Tables[j].Equals("article"))
                    {
                        for (int i = 0; i < oidArticles.Count; i++)
                        {
                            executeSqlData = string.Format(@"SELECT * FROM {0} where Oid = '{1}'", Tables[j], oidArticles[i]);

                            if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                            {
                                dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                                dtsqlserver.Fill(dsGetDataSet, Tables[j]);
                            }
                            else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                            {
                                dtmysql = new MySqlDataAdapter(executeSqlData, con);
                                dtmysql.Fill(dsGetDataSet, Tables[j]);
                            }
                            else
                            {
                                dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                                dtsqlite.Fill(dsGetDataSet, Tables[j]);
                            }
                        }
                    }
                    else
                    {
                        if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                        {
                            dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                            dtsqlserver.Fill(dsGetDataSet, Tables[j]);
                        }
                        else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                        {
                            dtmysql = new MySqlDataAdapter(executeSqlData, con);
                            dtmysql.Fill(dsGetDataSet, Tables[j]);
                        }
                        else
                        {
                            dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                            dtsqlite.Fill(dsGetDataSet, Tables[j]);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetDataWorkSessionMovement(e.Message): {0}", ex.Message), ex);

            }

            _log.Debug("dsGetWorkSessionMovement.WriteXml");

            return dsGetDataSet;
        }

        public static DataSet GetData_TotalDays(string _startDate, string _endDate)
        {
            string con = GlobalFramework.SessionXpo.ConnectionString.ToString();
            string executeSqlData = "";

            dsGetDataSet = new DataSet();

            List<string> Tables = new List<string>();

            Tables.Add("documentfinancemaster");
            Tables.Add("documentordermain");
            Tables.Add("configurationplacetable");
            Tables.Add("configurationplace");
            Tables.Add("configurationpaymentmethod");
            Tables.Add("configurationplaceterminal");

            for (int i = 0; i < Tables.Count; i++)
            {
                executeSqlData = string.Format("SELECT * FROM {0}", Tables[i]);

               
                    if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                    {
                        dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                        dtsqlserver.Fill(dsGetDataSet, Tables[i]);
                    }
                    else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                    {
                        dtmysql = new MySqlDataAdapter(executeSqlData, con);
                        dtmysql.Fill(dsGetDataSet, Tables[i]);
                    }
                    else
                    {
                        dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                        dtsqlite.Fill(dsGetDataSet, Tables[i]);
                    }
            }

            return dsGetDataSet;

        }


        public static DataSet GetData_TotalZonesTables(string _startDate, string _endDate)
        {
            string con = GlobalFramework.SessionXpo.ConnectionString.ToString();
            string executeSqlData = "";


            dsGetDataSet = new DataSet();

            List<string> Tables = new List<string>();

            Tables.Add("documentfinancemaster");
            Tables.Add("documentordermain");
            Tables.Add("configurationplaceterminal");
            Tables.Add("configurationpaymentmethod");
            Tables.Add("configurationplacetable");
            Tables.Add("configurationplace");

            List<string> dates = new List<string>();
            dates.Add(_startDate);
            dates.Add(_endDate);
            // dates = Data.SplitDate(_startDate, _endDate);


            for (int i = 0; i < Tables.Count; i++)
            {
                executeSqlData = string.Format("SELECT * FROM {0}", Tables[i]);

                if (Tables[i].Equals("documentfinancemaster"))
                {
                   
                        executeSqlData = string.Format("SELECT * FROM {0}  where CAST(Date as DATE) between '{1}' and '{2}'", Tables[i], dates[0], dates[1]);
                   
                    if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                    {
                        dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                        dtsqlserver.Fill(dsGetDataSet, Tables[i]);
                    }
                    else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                    {
                        dtmysql = new MySqlDataAdapter(executeSqlData, con);
                        dtmysql.Fill(dsGetDataSet, Tables[i]);
                    }
                    else
                    {
                        dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                        dtsqlite.Fill(dsGetDataSet, Tables[i]);
                    }

                }
                else
                {

                    if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                    {
                        dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                        dtsqlserver.Fill(dsGetDataSet, Tables[i]);
                    }
                    else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                    {
                        dtmysql = new MySqlDataAdapter(executeSqlData, con);
                        dtmysql.Fill(dsGetDataSet, Tables[i]);
                    }
                    else
                    {
                        dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                        dtsqlite.Fill(dsGetDataSet, Tables[i]);
                    }

                }
            }
            return dsGetDataSet;
        }

        public static DataSet GetData_TotalZones(string _startDate, string _endDate)
        {
            string con = GlobalFramework.SessionXpo.ConnectionString.ToString();
            string executeSqlData = "";

            dsGetDataSet = new DataSet();

            List<string> Tables = new List<string>();

            Tables.Add("documentfinancemaster");
            Tables.Add("documentordermain");
            Tables.Add("configurationplaceterminal");
            Tables.Add("configurationpaymentmethod");
            Tables.Add("configurationplacetable");
            Tables.Add("configurationplace");

            List<string> dates = new List<string>();
            dates.Add(_startDate);
            dates.Add(_endDate);
            // dates = Data.SplitDate(_startDate, _endDate);

            for (int i = 0; i < Tables.Count; i++)
            {
                executeSqlData = string.Format("SELECT * FROM {0}", Tables[i]);

                if (Tables[i].Equals("documentfinancemaster"))
                {
                    
                        executeSqlData = string.Format("SELECT * FROM {0}  where CAST(Date as DATE) between '{1}' and '{2}'", Tables[i], dates[0], dates[1]);
                   

                    if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                    {
                        dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                        dtsqlserver.Fill(dsGetDataSet, Tables[i]);
                    }
                    else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                    {
                        dtmysql = new MySqlDataAdapter(executeSqlData, con);
                        dtmysql.Fill(dsGetDataSet, Tables[i]);
                    }
                    else
                    {
                        dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                        dtsqlite.Fill(dsGetDataSet, Tables[i]);
                    }

                }
                else
                {
                   
                    if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                    {
                        dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                        dtsqlserver.Fill(dsGetDataSet, Tables[i]);
                    }
                    else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                    {
                        dtmysql = new MySqlDataAdapter(executeSqlData, con);
                        dtmysql.Fill(dsGetDataSet, Tables[i]);
                    }
                    else
                    {
                        dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                        dtsqlite.Fill(dsGetDataSet, Tables[i]);
                    }

                }
            }
            return dsGetDataSet;

        }

        public static DataSet GetData_OccupationZonestTables(string _startDate, string _endDate)
        {
            string con = GlobalFramework.SessionXpo.ConnectionString.ToString();
            string executeSqlData = "";

            dsGetDataSet = new DataSet();

            List<string> Tables = new List<string>();

            Tables.Add("documentfinancemaster");
            Tables.Add("documentordermain");
            Tables.Add("configurationplacetable");
            Tables.Add("configurationplace");

            List<string> dates = new List<string>();
            dates.Add(_startDate);
            dates.Add(_endDate);
            // dates = Data.SplitDate(_startDate, _endDate);


            for (int i = 0; i < Tables.Count; i++)
            {
                executeSqlData = string.Format("SELECT * FROM {0}", Tables[i]);

                if (Tables[i].Equals("documentfinancemaster"))
                {
                   
                        executeSqlData = string.Format("SELECT * FROM {0}  where CAST(Date as DATE) between '{1}' and '{2}'", Tables[i], dates[0], dates[1]);
                    

                    if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                    {
                        dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                        dtsqlserver.Fill(dsGetDataSet, Tables[i]);
                    }
                    else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                    {
                        dtmysql = new MySqlDataAdapter(executeSqlData, con);
                        dtmysql.Fill(dsGetDataSet, Tables[i]);
                    }
                    else
                    {
                        dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                        dtsqlite.Fill(dsGetDataSet, Tables[i]);
                    }

                }
                else
                {

                    if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                    {
                        dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                        dtsqlserver.Fill(dsGetDataSet, Tables[i]);
                    }
                    else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                    {
                        dtmysql = new MySqlDataAdapter(executeSqlData, con);
                        dtmysql.Fill(dsGetDataSet, Tables[i]);
                    }
                    else
                    {
                        dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                        dtsqlite.Fill(dsGetDataSet, Tables[i]);
                    }

                }
            }
            return dsGetDataSet;
        }


        public static DataSet GetData_TotalClients(string _statePay, string _startDate, string _endDate, string _report)
        {
            string con = GlobalFramework.SessionXpo.ConnectionString.ToString();
            string executeSqlData = "";

            dsGetDataSet = new DataSet();

            List<string> dates = new List<string>();
            dates.Add(_startDate);
            dates.Add(_endDate);
            // dates = Data.SplitDate(_startDate, _endDate);

            List<string> Tables = new List<string>();

          
           string fileReport_CurrentAccount = "Reports/Conta_corrente_clientes.frx";
           string fileReport_CurrentAccountDetails = "Reports/Conta_corrente_clientes_Detalhado.frx";
          
            Tables.Add("documentfinancemaster");
            Tables.Add("documentfinancedetail");

            for (int j = 0; j < Tables.Count; j++)
            {
                executeSqlData = string.Format("SELECT * FROM {0}", Tables[j]);

                if (_report.Equals(fileReport_CurrentAccount))
                {
                    if (Tables[j].Equals("documentfinancemaster"))
                    {
                        if (_statePay == Resx.NotPaid)
                        {
                            executeSqlData = string.Format(@"SELECT * FROM {0} where CAST(Date as DATE) between '{1}' and '{2}' and Payed = '0' and DocumentType = '{3}'", Tables[j], dates[0], dates[1], xpoOidDocumentFinanceTypeCurrentAccountInput);

                        }
                        else if (_statePay == Resx.Paid)
                        {
                            executeSqlData = string.Format(@"SELECT * FROM {0} where CAST(Date as DATE) between '{1}' and '{2}' and Payed = '1' and DocumentType = '{3}'", Tables[j], dates[0], dates[1], xpoOidDocumentFinanceTypeCurrentAccountInput);
                        }
                        else
                        {
                            executeSqlData = string.Format(@"SELECT * FROM {0} where CAST(Date as DATE) between '{1}' and '{2}' and DocumentType = '{3}'", Tables[j], dates[0], dates[1], xpoOidDocumentFinanceTypeCurrentAccountInput);

                        }

                        if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                        {
                            dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                            dtsqlserver.Fill(dsGetDataSet, Tables[j]);
                        }
                        else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                        {
                            dtmysql = new MySqlDataAdapter(executeSqlData, con);
                            dtmysql.Fill(dsGetDataSet, Tables[j]);
                        }
                        else
                        {
                            dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                            dtsqlite.Fill(dsGetDataSet, Tables[j]);
                        }
                    }
                    else
                    {

                        if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                        {
                            dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                            dtsqlserver.Fill(dsGetDataSet, Tables[j]);
                        }
                        else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                        {
                            dtmysql = new MySqlDataAdapter(executeSqlData, con);
                            dtmysql.Fill(dsGetDataSet, Tables[j]);
                        }
                        else
                        {
                            dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                            dtsqlite.Fill(dsGetDataSet, Tables[j]);
                        }
                    }

                }
                else if (_report.Equals(fileReport_CurrentAccountDetails))
                {
                    if (Tables[j].Equals("documentfinancedetail"))
                    {
                        if (_statePay == Resx.NotPaid)
                        {
                            executeSqlData = string.Format(@"select d.Oid as OidDetail, d.Code, d.Designation, d.Quantity, d.TotalFinal, d.DocumentMaster, d.Article, d.UpdatedWhere, d.UpdatedBy, m.Oid as OidMaster, m.DocumentNumber, m.EntityName, m.DocumentDate, m.Payed, m.PayedDate
                                                        from DocumentFinanceDetail d, DocumentFinanceMaster m where d.DocumentMaster = m.Oid and 
                                                        CAST(m.Date as DATE) between '{1}' and '{2}' and m.Payed = '0' and m.DocumentType = '{3}'",
                                                            Tables[j], dates[0], dates[1], xpoOidDocumentFinanceTypeCurrentAccountInput);

                        }
                        else if (_statePay == Resx.Paid)
                        {
                            executeSqlData = string.Format(@"select d.Oid as OidDetail, d.Code, d.Designation, d.Quantity, d.TotalFinal, d.DocumentMaster, d.Article, d.UpdatedWhere, d.UpdatedBy, m.Oid as OidMaster, m.DocumentNumber, m.EntityName, m.DocumentDate, m.Payed, m.PayedDate
                                                        from DocumentFinanceDetail d, DocumentFinanceMaster m where d.DocumentMaster = m.Oid and 
                                                        CAST(m.Date as DATE) between '{1}' and '{2}' and m.Payed = '1' and m.DocumentType = '{3}'",
                                                            Tables[j], dates[0], dates[1], xpoOidDocumentFinanceTypeCurrentAccountInput);
                        }
                        else
                        {
                            executeSqlData = string.Format(@"select d.Oid as OidDetail, d.Code, d.Designation, d.Quantity, d.TotalFinal, d.DocumentMaster, d.Article, d.UpdatedWhere, d.UpdatedBy, m.Oid as OidMaster, m.DocumentNumber, m.EntityName, m.DocumentDate, m.Payed, m.PayedDate
                                                        from DocumentFinanceDetail d, DocumentFinanceMaster m where d.DocumentMaster = m.Oid and 
                                                        CAST(m.Date as DATE) between '{1}' and '{2}' and m.DocumentType = '{3}'",
                                                            Tables[j], dates[0], dates[1], xpoOidDocumentFinanceTypeCurrentAccountInput);

                        }

                        if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                        {
                            dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                            dtsqlserver.Fill(dsGetDataSet, Tables[j]);
                        }
                        else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                        {
                            dtmysql = new MySqlDataAdapter(executeSqlData, con);
                            dtmysql.Fill(dsGetDataSet, Tables[j]);
                        }
                        else
                        {
                            dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                            dtsqlite.Fill(dsGetDataSet, Tables[j]);
                        }
                    }
                    else
                    {

                        if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                        {
                            dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                            dtsqlserver.Fill(dsGetDataSet, Tables[j]);
                        }
                        else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                        {
                            dtmysql = new MySqlDataAdapter(executeSqlData, con);
                            dtmysql.Fill(dsGetDataSet, Tables[j]);
                        }
                        else
                        {
                            dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                            dtsqlite.Fill(dsGetDataSet, Tables[j]);
                        }
                    }
                }

            }

          
            return dsGetDataSet;

        }


        public static DataSet GetData_TotalTerminal(string _startDate, string _endDate)
        {
            string con = GlobalFramework.SessionXpo.ConnectionString.ToString();
            string executeSqlData = "";

            dsGetDataSet = new DataSet();

            List<string> dates = new List<string>();
            dates.Add(_startDate);
            dates.Add(_endDate);
            // dates = Data.SplitDate(_startDate, _endDate);

            List<string> Tables = new List<string>();

            Tables.Add("documentfinancemaster");
            Tables.Add("documentordermain");
            Tables.Add("configurationplaceterminal");
            Tables.Add("configurationpaymentmethod");
            Tables.Add("configurationplacetable");
            Tables.Add("configurationplace");

            for (int j = 0; j < Tables.Count; j++)
            {
                executeSqlData = string.Format("SELECT * FROM {0}", Tables[j]);

                if (Tables[j].Equals("documentfinancemaster"))
                {

                   
                        executeSqlData = string.Format(@"SELECT * FROM {0} where CAST(Date as DATE) between '{1}' and '{2}'", Tables[j], dates[0], dates[1]);
                   

                    if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                    {
                        dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                        dtsqlserver.Fill(dsGetDataSet, Tables[j]);
                    }
                    else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                    {
                        dtmysql = new MySqlDataAdapter(executeSqlData, con);
                        dtmysql.Fill(dsGetDataSet, Tables[j]);
                    }
                    else
                    {
                        dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                        dtsqlite.Fill(dsGetDataSet, Tables[j]);
                    }
                }
                else
                {

                    if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                    {
                        dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                        dtsqlserver.Fill(dsGetDataSet, Tables[j]);
                    }
                    else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                    {
                        dtmysql = new MySqlDataAdapter(executeSqlData, con);
                        dtmysql.Fill(dsGetDataSet, Tables[j]);
                    }
                    else
                    {
                        dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                        dtsqlite.Fill(dsGetDataSet, Tables[j]);
                    }
                }

            }

            return dsGetDataSet;
        }


        public static void GetAllData()
        {
            string con = GlobalFramework.SessionXpo.ConnectionString.ToString();

            dsXMLReports = new DataSet();


            string nameTable = String.Empty, parentLabel = String.Empty, parentResource = String.Empty;

            XPSelectData xPSelectData = Utils.GetAllNameTable();

            try
            {
                foreach (SelectStatementResultRow parentRow in xPSelectData.Data)
                {
                    nameTable = parentRow.Values[xPSelectData.GetFieldIndex("TABLE_NAME")].ToString();

                    string executeSqlArticle = string.Format("SELECT * FROM {0}", nameTable);

                    executeSqlArticle.ToLower();


                    if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                    {
                        dtsqlserverXMLReports = new SqlDataAdapter(executeSqlArticle, con);
                        dtsqlserverXMLReports.Fill(dsXMLReports, nameTable);

                    }
                    else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                    {
                        dtmysqlXMLReports = new MySqlDataAdapter(executeSqlArticle, con);
                        dtmysqlXMLReports.Fill(dsXMLReports, nameTable);
                    }
                    else
                    {
                        dtsqliteXMLReports = new SqliteDataAdapter(executeSqlArticle, con);
                        dtsqliteXMLReports.Fill(dsXMLReports, nameTable);
                    }

                    for (int i = 0; i < dsXMLReports.Tables.Count; i++)
                    {
                        dsXMLReports.Tables[i].TableName = dsXMLReports.Tables[i].TableName.ToLower();
                        for (int j = 0; j < dsXMLReports.Tables[i].Columns.Count; j++)
                        {
                            dsXMLReports.Tables[i].Columns[j].ColumnName = dsXMLReports.Tables[i].Columns[j].ColumnName.ToLower();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("ConfigureXMLReports(e.Message): {0}", ex.Message), ex);

            }

            string thisFolder = (string.Format("{0}", GlobalFramework.Path["reports"]));

            string folderpath_sqlserver = Path.GetFullPath(thisFolder) + "relationshipTables.txt";
            string folderpath_mysql = Path.GetFullPath(thisFolder) + "relationshipTables_MySQL.txt";

            //string[] tmp_sqlserver = File.ReadAllLines(folderpath_sqlserver);
            //string[] tmp_mysql = File.ReadAllLines(folderpath_mysql);
            string[] tmp_sql = File.ReadAllLines(folderpath_sqlserver);


            try
            {
                foreach (string line in tmp_sql)
                {
                    tmp_sql = line.Split('\t');

                    string foreignKey = tmp_sql[0].ToLower();
                    string tableName = tmp_sql[1].ToLower();
                    string columnName = tmp_sql[2].ToLower();
                    string referenceTable = tmp_sql[3].ToLower();
                    string referenceColumnName = tmp_sql[4].ToLower();

                    dsXMLReports.Relations.Add(new DataRelation(foreignKey,
                            dsXMLReports.Tables[referenceTable].Columns[referenceColumnName],
                           dsXMLReports.Tables[tableName].Columns[columnName], false));

                }

            }
            catch (Exception ex)
            {
                _log.Error(string.Format("ConfigureXMLReports_relationshipTables(e.Message): {0}", ex.Message), ex);

            }

            dsXMLReports.WriteXml((string.Format("{0}{1}", GlobalFramework.Path["reports"], "sales.xml")), XmlWriteMode.WriteSchema);
 
        }



        public static DataSet GetData_TotalCash(string _startDate, string _endDate)
        {
            string con = GlobalFramework.SessionXpo.ConnectionString.ToString();
            string executeSqlData = "";

            dsGetDataSet = new DataSet();

            List<string> Tables = new List<string>();

            List<string> dates = new List<string>();
            dates.Add(_startDate);
            dates.Add(_endDate);
            // dates = Data.SplitDate(_startDate, _endDate);

            Tables.Add("worksessionmovement");
            Tables.Add("worksessionmovementtype");
            Tables.Add("worksessionperiod");
            Tables.Add("userdetail");
            Tables.Add("documentfinancemaster");
            Tables.Add("configurationpaymentmethod");
            Tables.Add("configurationplaceterminal");

            for (int j = 0; j < Tables.Count; j++)
            {
                executeSqlData = string.Format("SELECT * FROM {0}", Tables[j]);

                if (Tables[j].Equals("worksessionmovement"))
                {

                   
                        executeSqlData = string.Format(@"SELECT * FROM {0} where CAST(Date as DATE) between '{1}' and '{2}'", Tables[j], dates[0], dates[1]);
                   

                    if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                    {
                        dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                        dtsqlserver.Fill(dsGetDataSet, Tables[j]);
                    }
                    else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                    {
                        dtmysql = new MySqlDataAdapter(executeSqlData, con);
                        dtmysql.Fill(dsGetDataSet, Tables[j]);
                    }
                    else
                    {
                        dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                        dtsqlite.Fill(dsGetDataSet, Tables[j]);
                    }

                }
                else
                {
                    if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                    {
                        dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                        dtsqlserver.Fill(dsGetDataSet, Tables[j]);
                    }
                    else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                    {
                        dtmysql = new MySqlDataAdapter(executeSqlData, con);
                        dtmysql.Fill(dsGetDataSet, Tables[j]);
                    }
                    else
                    {
                        dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                        dtsqlite.Fill(dsGetDataSet, Tables[j]);
                    }
                }
            }

            return dsGetDataSet;
        }

        public static DataSet GetData_TotalCustomer(string _startDate, string _endDate)
        {
            string con = GlobalFramework.SessionXpo.ConnectionString.ToString();
            string executeSqlData = "";

            dsGetDataSet = new DataSet();

            
            List<string> dates = new List<string>();
            dates.Add(_startDate);
            dates.Add(_endDate);
            // dates = Data.SplitDate(_startDate, _endDate);

            List<string> Tables = new List<string>();

            Tables.Add("documentfinancemaster");
            Tables.Add("documentordermain");
            Tables.Add("configurationplacetable");
            Tables.Add("configurationplaceterminal");
            Tables.Add("configurationpaymentmethod");
            Tables.Add("configurationplace");
            Tables.Add("userdetail");


                for (int j = 0; j < Tables.Count; j++)
                {
                    executeSqlData = string.Format("SELECT * FROM {0}", Tables[j]);

                    if (Tables[j].Equals("documentfinancemaster"))
                    {
                        
                           
                                executeSqlData = string.Format(@"SELECT * FROM {0} where CAST(Date as DATE) between '{1}' and '{2}'", Tables[j], dates[0], dates[1]);
                           

                            if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                            {
                                dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                                dtsqlserver.Fill(dsGetDataSet, Tables[j]);
                            }
                            else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                            {
                                dtmysql = new MySqlDataAdapter(executeSqlData, con);
                                dtmysql.Fill(dsGetDataSet, Tables[j]);
                            }
                            else
                            {
                                dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                                dtsqlite.Fill(dsGetDataSet, Tables[j]);
                            }
                        
                    }
                    else
                    {
                        if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                        {
                            dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                            dtsqlserver.Fill(dsGetDataSet, Tables[j]);
                        }
                        else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                        {
                            dtmysql = new MySqlDataAdapter(executeSqlData, con);
                            dtmysql.Fill(dsGetDataSet, Tables[j]);
                        }
                        else
                        {
                            dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                            dtsqlite.Fill(dsGetDataSet, Tables[j]);
                        }
                    }
                }

            return dsGetDataSet;
        }



        public static DataSet GetData_MovCustomers(string _startDate, string _endDate)
        {
            string con = GlobalFramework.SessionXpo.ConnectionString.ToString();
            string executeSqlData = "";

            dsGetDataSet = new DataSet();


            List<string> dates = new List<string>();
            dates.Add(_startDate);
            dates.Add(_endDate);
            // dates = Data.SplitDate(_startDate, _endDate);

            List<string> Tables = new List<string>();

            Tables.Add("systemaudit");
            Tables.Add("systemaudittype");
            Tables.Add("configurationplaceterminal");
            Tables.Add("userdetail");

            for (int j = 0; j < Tables.Count; j++)
            {
                executeSqlData = string.Format("SELECT * FROM {0}", Tables[j]);

                if (Tables[j].Equals("systemaudit"))
                {
                    executeSqlData = string.Format("SELECT * FROM {0}", Tables[j]);


                   
                        executeSqlData = string.Format(@"SELECT * FROM {0} f where CAST(Date as DATE) between '{1}' and '{2}'", Tables[j], dates[0], dates[1]);
                   

                    if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                    {
                        dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                        dtsqlserver.Fill(dsGetDataSet, Tables[j]);
                    }
                    else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                    {
                        dtmysql = new MySqlDataAdapter(executeSqlData, con);
                        dtmysql.Fill(dsGetDataSet, Tables[j]);
                    }
                    else
                    {
                        dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                        dtsqlite.Fill(dsGetDataSet, Tables[j]);
                    }

                }
                else
                {

                    if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                    {
                        dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                        dtsqlserver.Fill(dsGetDataSet, Tables[j]);
                    }
                    else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                    {
                        dtmysql = new MySqlDataAdapter(executeSqlData, con);
                        dtmysql.Fill(dsGetDataSet, Tables[j]);
                    }
                    else
                    {
                        dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                        dtsqlite.Fill(dsGetDataSet, Tables[j]);
                    }
                }
            }


            return dsGetDataSet;
        }



        public static DataSet GetData_RecordsCustomer(string _startDate, string _endDate)
        {
            string con = GlobalFramework.SessionXpo.ConnectionString.ToString();
            string executeSqlData = "";

            dsGetDataSet = new DataSet();

            List<string> dates = new List<string>();
            dates.Add(_startDate);
            dates.Add(_endDate);
            // dates = Data.SplitDate(_startDate, _endDate);

           
                List<string> Tables = new List<string>();

                Tables.Add("documentfinancedetail");
                Tables.Add("documentfinancemaster");
                Tables.Add("userdetail");

                for (int j = 0; j < Tables.Count; j++)
                {
                    executeSqlData = string.Format("SELECT * FROM {0}", Tables[j]);

                    if (Tables[j].Equals("documentfinancemaster"))
                    {

                        executeSqlData = string.Format("SELECT * FROM {0}", Tables[j]);

                            
                                executeSqlData = string.Format(@"SELECT * FROM {0} f where CAST(CreatedAt as DATE) between '{1}' and '{2}'", Tables[j], dates[0], dates[1]);
                            
                            if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                            {
                                dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                                dtsqlserver.Fill(dsGetDataSet, Tables[j]);
                            }
                            else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                            {
                                dtmysql = new MySqlDataAdapter(executeSqlData, con);
                                dtmysql.Fill(dsGetDataSet, Tables[j]);
                            }
                            else
                            {
                                dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                                dtsqlite.Fill(dsGetDataSet, Tables[j]);
                            }
                        
                    }
                    else
                    {
                        if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                        {
                            dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                            dtsqlserver.Fill(dsGetDataSet, Tables[j]);
                        }
                        else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                        {
                            dtmysql = new MySqlDataAdapter(executeSqlData, con);
                            dtmysql.Fill(dsGetDataSet, Tables[j]);
                        }
                        else
                        {
                            dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                            dtsqlite.Fill(dsGetDataSet, Tables[j]);
                        }
                    }
                }

            return dsGetDataSet;
        }


        public static DataSet GetData_TotalFamily(string _startDate, string _endDate)
        {
            string con = GlobalFramework.SessionXpo.ConnectionString.ToString();
            string executeSqlData = "";

            List<string> dates = new List<string>();

            dsGetDataSet = new DataSet();

            dates.Add(_startDate);
            dates.Add(_endDate);
            // dates = Data.SplitDate(_startDate, _endDate);


            List<string> Tables = new List<string>();

            Tables.Add("documentfinancedetail");
            Tables.Add("documentfinancemaster");
            Tables.Add("article");
            Tables.Add("articlesubfamily");
            Tables.Add("articlefamily");
            Tables.Add("configurationplaceterminal");
            Tables.Add("userdetail");

            for (int j = 0; j < Tables.Count; j++)
            {
                executeSqlData = string.Format("SELECT * FROM {0}", Tables[j]);

                if (Tables[j].Equals("documentfinancedetail"))
                {
                    executeSqlData = string.Format("SELECT * FROM {0}", Tables[j]);

                    executeSqlData = string.Format(@"SELECT * FROM {0} f where CAST(CreatedAt as DATE) between '{1}' and '{2}'", Tables[j], dates[0], dates[1]);
                    
                    if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                    {
                        dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                        dtsqlserver.Fill(dsGetDataSet, Tables[j]);
                    }
                    else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                    {
                        dtmysql = new MySqlDataAdapter(executeSqlData, con);
                        dtmysql.Fill(dsGetDataSet, Tables[j]);
                    }
                    else
                    {
                        dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                        dtsqlite.Fill(dsGetDataSet, Tables[j]);
                    }
                }

                else
                {
                    if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                    {
                        dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                        dtsqlserver.Fill(dsGetDataSet, Tables[j]);
                    }
                    else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                    {
                        dtmysql = new MySqlDataAdapter(executeSqlData, con);
                        dtmysql.Fill(dsGetDataSet, Tables[j]);
                    }
                    else
                    {
                        dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                        dtsqlite.Fill(dsGetDataSet, Tables[j]);
                    }
                }
            }
            return dsGetDataSet;
        }



        public static DataSet GetData_ListArticles()
        {
          
            string con = GlobalFramework.SessionXpo.ConnectionString.ToString();
            string executeSqlData = "";

            dsGetDataSet = new DataSet();

            List<string> Tables = new List<string>();

            Tables.Add("article");
            Tables.Add("articlesubfamily");
            Tables.Add("articlefamily");
            Tables.Add("articletype");

            for (int i = 0; i < Tables.Count; i++)
            {
                executeSqlData = string.Format("SELECT * FROM {0}", Tables[i]);

                if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                {
                    dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                    dtsqlserver.Fill(dsGetDataSet, Tables[i]);
                }
                else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                {
                    dtmysql = new MySqlDataAdapter(executeSqlData, con);
                    dtmysql.Fill(dsGetDataSet, Tables[i]);
                }
                else
                {
                    dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                    dtsqlite.Fill(dsGetDataSet, Tables[i]);
                }

            }
           _log.Debug(string.Format("return GetData_ListArticles"));

            return dsGetDataSet;
            
        }

      

        public static void ChangeItemsClosingBox(DataSet _items, string _fileReport, string _itemSearch)
        {
            string _code = "";
            string _designation = "";

            FReportsFolder = (string.Format("{0}", GlobalFramework.Path["reports"]));

            
            string fileReport  = FReportsFolder + _fileReport;
           

            try
            {
                for (int i = 0; i < _items.Tables.Count; i++)
                {
                    _code = _items.Tables[i].Columns[0].Caption;
                    _designation = _items.Tables[i].Columns[1].Caption;

                    XDocument doc = XDocument.Load(fileReport);
                    IEnumerable<XElement> elements = doc.Elements("Report");

                    foreach (XElement node in elements)
                    {
                        XElement attachment;
                        if (node.Element("ReportPage").Element("GroupHeaderBand").Element("DataBand") != null)
                        {
                            attachment = node.Element("ReportPage").Element("GroupHeaderBand").Element("DataBand");

                            foreach (XElement child in attachment.Elements("TextObject"))
                            {
                                if (child.Attribute("Name") != null && child.Attribute("Name").Value.Equals("Text_Designation"))
                                {
                                    child.Attribute("Text").SetValue("[view_worksessionmovementresume." + _designation.ToLower() + "]");
                                }
                                if (child.Attribute("Text") != null && child.Attribute("Name").Value.Equals("Text_Code"))
                                {
                                    child.Attribute("Text").SetValue("[view_worksessionmovementresume." + _code.ToLower() + "]");
                                }

                                if (child.Attribute("Format.CurrencySymbol") != null)
                                {
                                    child.Attribute("Format.CurrencySymbol").SetValue("");

                                }

                            }
                            doc.Save(fileReport);
                        }

                        if (node.Element("ReportPage").Element("GroupHeaderBand").Element("DataBand").Element("DataFooterBand") != null)
                        {
                            attachment = node.Element("ReportPage").Element("GroupHeaderBand").Element("DataBand").Element("DataFooterBand");

                            foreach (XElement child in attachment.Elements("TextObject"))
                            {
                                if (child.Attribute("Text") != null && child.Attribute("Text").Value.Equals("[Total1]"))
                                {
                                    child.Attribute("Format.CurrencySymbol").SetValue("");

                                }

                            }
                            doc.Save(fileReport);
                        }

                        if (node.Element("ReportPage").Element("ReportTitleBand") != null)
                        {
                            attachment = node.Element("ReportPage").Element("ReportTitleBand");

                            foreach (XElement child in attachment.Elements("TextObject"))
                            {
                                if (child.Attribute("Name") != null && child.Attribute("Name").Value.Equals("Text_TypeSearch"))
                                {
                                    child.Attribute("Text").SetValue(_itemSearch);
                                }

                            }
                            doc.Save(fileReport);
                        }

                        doc.Save(fileReport);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetCurrency(e.Message): {0}", ex.Message), ex);

            }
        }


        public static DataSet GetClosingBox_Day(string _startDate, string _endDate, DataTable _terminals, string _itemSelected)
        {
            string con = GlobalFramework.SessionXpo.ConnectionString.ToString();
            string executeSqlData = "";
            string executeSqlPeriodType = "";
            DataTable _periodType = new DataTable();
            dsGetDataSet = new DataSet();
            DataSet dsGetDataSetPeriodType = new DataSet();
            List<string> dates = new List<string>();
            string sqlQuery = "";

            try{

            string templateQuery = (@"SELECT Period FROM view_worksessionmovementresume where PeriodType = 1 and TerminalDesignation = '{0}' Group BY Period;");

            for (int i = 0; i < _terminals.Rows.Count; i++)
             {
                 executeSqlPeriodType= string.Format(templateQuery, _terminals.Rows[i].ItemArray[0].ToString());
                 
                sqlQuery += executeSqlPeriodType;

                if (i < _terminals.Rows.Count - 1)
                 {
                     sqlQuery += " union ";
                 }
             }

            XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(sqlQuery);

            _periodType.Columns.Add("Oid");

            dates.Add(_startDate);
            dates.Add(_endDate);
            // dates = Data.SplitDate(_startDate, _endDate);

            foreach (SelectStatementResultRow parentRow in xPSelectData.Data)
            {
                string Oid = parentRow.Values[xPSelectData.GetFieldIndex("Period")].ToString();


                if (_itemSelected == null || _itemSelected.Equals("Família") || _itemSelected.Equals("Family"))
                {
                    executeSqlData = string.Format(@"Select MIN(FamilyCode) as Code, FamilyDesignation, TerminalDesignation,
                                                SUM(Quantity) as Quantity, SUM(TotalGross) as TotalGross, 
                                                SUM(TotalTax) as TotalTax, SUM(TotalFinal) as TotalFinal
                                              FROM 
                                                view_worksessionmovementresume
                                              WHERE 
                                                Document IS NOT NULL
                                                AND MovementTypeToken = 'FINANCE_DOCUMENT'
                                                AND Period = '{0}'
                                                AND CAST(MovementDate as DATE) between '{1}' and '{2}'
                                              GROUP BY 
                                                FamilyDesignation, TerminalDesignation
                                              ORDER BY 
                                                MIN(FamilyCode)", Oid, dates[0], dates[1]);
                }
                else if (_itemSelected.Equals("Subfamília") || _itemSelected.Equals("Subfamily"))
                {
                    executeSqlData = string.Format(@"Select MIN(SubFamilyCode) as Code, SubFamilyDesignation, TerminalDesignation,
                                                SUM(Quantity) as Quantity, SUM(TotalGross) as TotalGross, 
                                                SUM(TotalTax) as TotalTax, SUM(TotalFinal) as TotalFinal
                                              FROM 
                                                view_worksessionmovementresume
                                              WHERE 
                                                Document IS NOT NULL
                                                AND MovementTypeToken = 'FINANCE_DOCUMENT'
                                                AND Period = '{0}'
                                                AND CAST(MovementDate as DATE) between '{1}' and '{2}'
                                              GROUP BY 
                                                SubFamilyDesignation, TerminalDesignation
                                              ORDER BY 
                                                MIN(SubFamilyCode)", Oid, dates[0], dates[1]);
                }

                else if (_itemSelected.Equals("Artigo") || _itemSelected.Equals("Article"))
                {
                    executeSqlData = string.Format(@"Select MIN(Code) as Code, Designation, TerminalDesignation,
                                                SUM(Quantity) as Quantity, SUM(TotalGross) as TotalGross, 
                                                SUM(TotalTax) as TotalTax, SUM(TotalFinal) as TotalFinal
                                              FROM 
                                                view_worksessionmovementresume
                                              WHERE 
                                                Document IS NOT NULL
                                                AND MovementTypeToken = 'FINANCE_DOCUMENT'
                                                AND Period = '{0}'
                                                AND CAST(MovementDate as DATE) between '{1}' and '{2}'
                                              GROUP BY 
                                                Designation, TerminalDesignation
                                              ORDER BY 
                                                MIN(Code)", Oid, dates[0], dates[1]);
                }
                else if (_itemSelected.Equals("Taxa de IVA") || _itemSelected.Equals("VAT rate"))
                {
                    executeSqlData = string.Format(@"Select MIN(VatCode) as Code, VatDesignation, TerminalDesignation,
                                                SUM(Quantity) as Quantity, SUM(TotalGross) as TotalGross, 
                                                SUM(TotalTax) as TotalTax, SUM(TotalFinal) as TotalFinal
                                              FROM 
                                                view_worksessionmovementresume
                                              WHERE 
                                                Document IS NOT NULL
                                                AND MovementTypeToken = 'FINANCE_DOCUMENT'
                                                AND Period = '{0}'
                                                AND CAST(MovementDate as DATE) between '{1}' and '{2}'
                                              GROUP BY 
                                                VatDesignation, TerminalDesignation
                                              ORDER BY 
                                                MIN(VatCode)", Oid, dates[0], dates[1]);
                }
                else if (_itemSelected.Equals("Método de pagamento") || _itemSelected.Equals("Payment method"))
                {
                    executeSqlData = string.Format(@"Select MIN(PaymentMethodCode) as Code, PaymentMethodDesignation, TerminalDesignation,
                                                SUM(Quantity) as Quantity, SUM(TotalGross) as TotalGross, 
                                                SUM(TotalTax) as TotalTax, SUM(TotalFinal) as TotalFinal
                                              FROM 
                                                view_worksessionmovementresume
                                              WHERE 
                                                Document IS NOT NULL
                                                AND MovementTypeToken = 'FINANCE_DOCUMENT'
                                                AND Period = '{0}'
                                                AND CAST(MovementDate as DATE) between '{1}' and '{2}'
                                              GROUP BY 
                                                PaymentMethodDesignation, TerminalDesignation
                                              ORDER BY 
                                                MIN(PaymentMethodCode)", Oid, dates[0], dates[1]);
                }
                
                if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
                {
                    dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                    dtsqlserver.Fill(dsGetDataSet, "view_worksessionmovementresume");
                }
                else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
                {
                    dtmysql = new MySqlDataAdapter(executeSqlData, con);
                    dtmysql.Fill(dsGetDataSet, "view_worksessionmovementresume");
                }
                else
                {
                    dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                    dtsqlite.Fill(dsGetDataSet, "view_worksessionmovementresume");
                }
            }

            _log.Debug(string.Format("return GetClosingBox_Day"));

            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetClosingBox_Day(e.Message): {0}", ex.Message), ex);

            }
            return dsGetDataSet;
        }

        internal static DataSet GetDataStockArticle(string _startDate, string _endDate)
        {
            string con = GlobalFramework.SessionXpo.ConnectionString.ToString();
            dsGetDataSet = new DataSet();

            string executeSqlData = string.Format(@"SELECT 
                                         afaCode AS FamilyCode, afaDesignation AS Family,
                                         asfCode AS SubFamilyCode, asfDesignation AS SubFamily,
                                         artCode AS ArticleCode, artDesignation AS Designation, 
                                         aumDesignation AS UnitMeasure,
                                         (SELECT SUM(Quantity) FROM articlestock WHERE article = artOid AND Quantity > 0) AS StockIn,
                                         (SELECT SUM(Quantity) FROM articlestock WHERE article = artOid AND Quantity < 0) AS StockOut,
                                         (SELECT SUM(Quantity) FROM articlestock WHERE article = artOid) AS StockBalance
                                        FROM 
                                         view_articlestock 
                                        WHERE 
                                         stkDate >= '{0}' AND stkDate <= '{1}' 
                                        GROUP BY 
                                         artOid
                                        ORDER BY 
                                         afaOrd, asfOrd, artOrd
                                        ;", _startDate, _endDate);

            if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
            {
                dtsqlserver = new SqlDataAdapter(executeSqlData, con);
                dtsqlserver.Fill(dsGetDataSet, "view_articlestock");
            }
            else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
            {
                dtmysql = new MySqlDataAdapter(executeSqlData, con);
                dtmysql.Fill(dsGetDataSet, "view_articlestock");
            }
            else
            {
                dtsqlite = new SqliteDataAdapter(executeSqlData, con);
                dtsqlite.Fill(dsGetDataSet, "view_articlestock");
            }
            return dsGetDataSet;
        }
    }
}
