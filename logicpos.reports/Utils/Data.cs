using DevExpress.Xpo.DB;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.reports.App;
using logicpos.reports.Resources.Localization;
using logicpos.reports.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace logicpos.reports
{
    public class Data
    {
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static string FReportsFolder = (string.Format("{0}", GlobalFramework.Path["reports"]));

        private static string xpoOidDocumentFinanceTypeCurrentAccountInput = GlobalFramework.Settings["xpoOidDocumentFinanceTypeCurrentAccountInput"];



        public static DataTable GetTerminal_MovementResume()
        {
            string terminal = "";

            DataTable table = new DataTable();


            string strSql = @"SELECT TerminalDesignation FROM view_worksessionmovementresume Where PeriodType = 1 GROUP BY TerminalDesignation";

            strSql = string.Format(strSql);

            XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(strSql);

            table.Columns.Add("TerminalDesignation");

            try
            {
                foreach (SelectStatementResultRow parentRow in xPSelectData.Data)
                {
                    terminal = parentRow.Values[xPSelectData.GetFieldIndex("TerminalDesignation")].ToString();
                    table.Rows.Add(terminal);
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetPayment(e.Message): {0}", ex.Message), ex);

            }
            return table;
        }


        public static string GetUser(string _code)
        {
            string result = "";

            string strSql = @"SELECT Oid FROM userdetail WHERE AccessPin = '{0}' AND (Disabled <> 1 OR Disabled IS NULL);";

            strSql = string.Format(strSql, _code);

            XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(strSql);

            try
            {
                foreach (SelectStatementResultRow parentRow in xPSelectData.Data)
                {
                    result = parentRow.Values[xPSelectData.GetFieldIndex("Oid")].ToString();

                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetMovements(e.Message): {0}", ex.Message), ex);

            }
            return result;
        }

        public static DataTable GetPayment()
        {
            string nameTable = "";

            DataTable table = new DataTable();


            string strSql = @"SELECT p.Designation FROM worksessionmovement m, documentfinancemaster f, configurationpaymentmethod p
                              where m.DocumentFinanceMaster = f.Oid and f.PaymentMethod = p.Oid group by p.Designation";

            strSql = string.Format(strSql);

            XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(strSql);

            table.Columns.Add("Name");

            table.Rows.Add(Resx.All);


            try
            {
                foreach (SelectStatementResultRow parentRow in xPSelectData.Data)
                {

                    nameTable = parentRow.Values[xPSelectData.GetFieldIndex("Designation")].ToString();

                    table.Rows.Add(nameTable);

                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetPayment(e.Message): {0}", ex.Message), ex);

            }

            return table;
        }



        public static DataTable GetClients()
        {
            string nameTable = "";

            DataTable table = new DataTable();


            string strSql = @"SELECT entityname FROM documentfinancemaster
                                WHERE documenttype = '" + xpoOidDocumentFinanceTypeCurrentAccountInput + "' GROUP BY entityname";

            strSql = string.Format(strSql);

            XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(strSql);

            table.Columns.Add("Name");


            table.Rows.Add(Resx.All);


            try
            {
                foreach (SelectStatementResultRow parentRow in xPSelectData.Data)
                {

                    nameTable = parentRow.Values[xPSelectData.GetFieldIndex("entityname")].ToString();

                    table.Rows.Add(nameTable);

                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetMovements(e.Message): {0}", ex.Message), ex);

            }

            return table;
        }

        public static DataTable GetMovements()
        {
            string nameTable = "";

            DataTable table = new DataTable();


            string strSql = @"SELECT t.Designation FROM worksessionmovement m, worksessionmovementtype t
                                where m.WorkSessionMovementType = t.Oid group by t.Designation";

            strSql = string.Format(strSql);

            XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(strSql);

            table.Columns.Add("Name");

            table.Rows.Add(Resx.All);



            try
            {
                foreach (SelectStatementResultRow parentRow in xPSelectData.Data)
                {

                    nameTable = parentRow.Values[xPSelectData.GetFieldIndex("Designation")].ToString();

                    table.Rows.Add(nameTable);

                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetMovements(e.Message): {0}", ex.Message), ex);

            }

            return table;
        }


        public static DataTable GetCustomerRecords()
        {
            string nameTable = "";

            DataTable table = new DataTable();


            string strSql = @"select u.name as name from userdetail u, documentfinancedetail a where a.UpdatedBy = u.Oid GROUP BY u.name;";

            strSql = string.Format(strSql);

            XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(strSql);

            table.Columns.Add("Name");


            table.Rows.Add(Resx.All);

            try
            {
                foreach (SelectStatementResultRow parentRow in xPSelectData.Data)
                {

                    nameTable = parentRow.Values[xPSelectData.GetFieldIndex("name")].ToString();

                    table.Rows.Add(nameTable);

                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetAllCustomersRecord(e.Message): {0}", ex.Message), ex);

            }

            return table;
        }

        public static DataTable GetCustomer()
        {
            string nameTable = "";

            DataTable table = new DataTable();


            string strSql = @"select u.name as name from userdetail u, systemaudit a where a.UserDetail = u.Oid GROUP BY u.name";

            strSql = string.Format(strSql);

            XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(strSql);

            table.Columns.Add("Name");

            table.Rows.Add(Resx.All);



            try
            {
                foreach (SelectStatementResultRow parentRow in xPSelectData.Data)
                {

                    nameTable = parentRow.Values[xPSelectData.GetFieldIndex("name")].ToString();

                    table.Rows.Add(nameTable);

                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetAllFamilyArticles(e.Message): {0}", ex.Message), ex);

            }

            return table;
        }

        public static DataTable GetFamilyArticles()
        {
            string nameTable = "";

            DataTable table = new DataTable();


            string strSql = @"select f.Designation from Article a, ArticleSubFamily s,
                            ArticleFamily f where f.Oid = s.Family and s.Oid = a.SubFamily GROUP BY f.Designation";

            strSql = string.Format(strSql);

            XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(strSql);

            table.Columns.Add("Designation");

            table.Rows.Add(Resx.All);


            try
            {
                foreach (SelectStatementResultRow parentRow in xPSelectData.Data)
                {

                    nameTable = parentRow.Values[xPSelectData.GetFieldIndex("Designation")].ToString();

                    table.Rows.Add(nameTable);

                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetFamilyArticles(e.Message): {0}", ex.Message), ex);

            }

            return table;

        }

        public static DataTable GetAllFamilyArticles()
        {
            string nameTable = "";

            DataTable table = new DataTable();


            string strSql = @"select f.Designation from documentfinancedetail o, Article a, ArticleSubFamily s,
                            ArticleFamily f where f.Oid = s.Family and s.Oid = a.SubFamily and a.Oid = o.Article GROUP BY f.Designation";

            strSql = string.Format(strSql);

            XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(strSql);

            table.Columns.Add("Designation");


            table.Rows.Add(Resx.All);



            try
            {
                foreach (SelectStatementResultRow parentRow in xPSelectData.Data)
                {

                    nameTable = parentRow.Values[xPSelectData.GetFieldIndex("Designation")].ToString();

                    table.Rows.Add(nameTable);

                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetAllFamilyArticles(e.Message): {0}", ex.Message), ex);

            }

            return table;

        }

        public static DataTable GetTerminal()
        {
            string nameTable = "";

            DataTable table = new DataTable();

            string strSql = @"select t.Designation from DocumentFinanceMaster f, ConfigurationPlaceTerminal t where f.UpdatedWhere = t.Oid GROUP BY t.Designation";

            strSql = string.Format(strSql);

            XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(strSql);

            table.Columns.Add("Designation");

            table.Rows.Add(Resx.All);



            try
            {
                foreach (SelectStatementResultRow parentRow in xPSelectData.Data)
                {

                    nameTable = parentRow.Values[xPSelectData.GetFieldIndex("Designation")].ToString();

                    table.Rows.Add(nameTable);


                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetTerminal(e.Message): {0}", ex.Message), ex);

            }

            return table;

        }


        public static DataTable GetAllTables(string _zone)
        {
            string nameTable = "";

            DataTable table = new DataTable();
            table.Columns.Add("Designation");
            try
            {
                if (_zone != "Todos" || _zone != "All")
                {
                    string strSql = @"select t.Designation from ConfigurationPlaceTable t, DocumentFinanceMaster f, DocumentOrderMain o,
                            ConfigurationPlace p where f.SourceOrderMain = o.Oid and o.PlaceTable = t.Oid and t.Place = p.Oid and p.Designation ='" + _zone + "' GROUP BY t.Designation";

                    strSql = string.Format(strSql);

                    XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(strSql);

                    foreach (SelectStatementResultRow parentRow in xPSelectData.Data)
                    {
                        nameTable = parentRow.Values[xPSelectData.GetFieldIndex("Designation")].ToString();

                        table.Rows.Add(nameTable);
                    }
                }
                else
                {

                    table.Rows.Add(Resx.All);


                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetAllTables(e.Message): {0}", ex.Message), ex);

            }
            return table;

        }


        public static DataSet GetTablesByDesignation(string _tables, string _zones, string _startDate, string _endDate)
        {
            DataSet data = new DataSet();
            string strSql = "";
            XPSelectData xPSelectData = null;
            string OidDocumentFinanceMaster;
            List<string> listOidZonesTables = new List<string>();
            List<string> result = new List<string>();


            try
            {
                if (_tables == null)
                {
                    strSql = string.Format(@"select dm.Oid from DocumentFinanceMaster dm, ConfigurationPlace p, ConfigurationPlaceTable pt, DocumentOrderMain d
                                          where p.Designation = '" + _zones + "'and pt.Place = p.Oid and d.PlaceTable = pt.Oid and dm.SourceOrderMain = d.Oid");

                }
                else
                {
                    strSql = string.Format(@"select dm.Oid from DocumentFinanceMaster dm, ConfigurationPlace p, ConfigurationPlaceTable pt, DocumentOrderMain d
                                          where p.Designation = '" + _zones + "' and pt.Designation = '" + _tables + "' and pt.Place = p.Oid and d.PlaceTable = pt.Oid and dm.SourceOrderMain = d.Oid");
                }
                xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(strSql);

                foreach (SelectStatementResultRow parentRow in xPSelectData.Data)
                {
                    OidDocumentFinanceMaster = parentRow.Values[xPSelectData.GetFieldIndex("Oid")].ToString();
                    listOidZonesTables.Add(OidDocumentFinanceMaster);
                }


                result = listOidZonesTables.Union(listOidZonesTables).ToList();

            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetTablesByDesignation_SelectOidZonesTables(e.Message): {0}", ex.Message), ex);

            }

            if (result.Count != 0)
            {
                data = DataReportsXML.GetDataDocumentFinanceMaster(result, _startDate, _endDate);
            }


            return data;

        }


        public static DataTable GetAllTablesZones()
        {
            string nameTable = "";

            DataTable table = new DataTable();


            string strSql = @"select p.Designation from ConfigurationPlaceTable t, DocumentFinanceMaster f, DocumentOrderMain o,
                            ConfigurationPlace p where f.SourceOrderMain = o.Oid and o.PlaceTable = t.Oid and t.Place = p.Oid GROUP BY p.Designation";

            strSql = string.Format(strSql);

            XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(strSql);

            table.Columns.Add("Designation");


            table.Rows.Add(Resx.All);


            try
            {
                foreach (SelectStatementResultRow parentRow in xPSelectData.Data)
                {

                    nameTable = parentRow.Values[xPSelectData.GetFieldIndex("Designation")].ToString();

                    table.Rows.Add(nameTable);


                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetAllTablesZones(e.Message): {0}", ex.Message), ex);

            }

            return table;

        }


        public static DataTable GetZones()
        {
            string nameTable = "";

            DataTable table = new DataTable();


            string strSql = @"select p.Designation from ConfigurationPlaceTable t, DocumentFinanceMaster f, DocumentOrderMain o,
                            ConfigurationPlace p where f.SourceOrderMain = o.Oid and o.PlaceTable = t.Oid and t.Place = p.Oid GROUP BY p.Designation";

            strSql = string.Format(strSql);

            XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(strSql);

            table.Columns.Add("Designation");


            table.Rows.Add(Resx.All);

            try
            {
                foreach (SelectStatementResultRow parentRow in xPSelectData.Data)
                {

                    nameTable = parentRow.Values[xPSelectData.GetFieldIndex("Designation")].ToString();

                    table.Rows.Add(nameTable);


                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetAllTablesZones(e.Message): {0}", ex.Message), ex);

            }

            return table;
        }


        public static DataSet GetClientsByName(string _client, string _statePay, string _startDate, string _endDate, string _report)
        {
            DataSet data = new DataSet();
            string strSql = "";
            XPSelectData xPSelectData = null;
            string OidDocumentFinanceMaster = "";
            List<string> listDocumentFinanceMaster = new List<string>();
            List<string> result = new List<string>();

            string fileReport_CurrentAccount = "Reports/Conta_corrente_clientes.frx";

            try
            {
                if (_report.Equals(fileReport_CurrentAccount))
                {

                    if (_statePay == Resx.NotPaid)
                    {
                        strSql = string.Format(@"select Oid, EntityName from DocumentFinanceMaster where DocumentType = '{0}' 
                                            AND EntityName = '{1}' AND Payed = 0
                                            GROUP BY Oid, EntityName", xpoOidDocumentFinanceTypeCurrentAccountInput, _client);



                    }
                    else if (_statePay == Resx.Paid)
                    {
                        strSql = string.Format(@"select Oid, EntityName from DocumentFinanceMaster where DocumentType = '{0}'
                                        AND EntityName = '{1}' AND Payed = 1
                                        GROUP BY Oid, EntityName", xpoOidDocumentFinanceTypeCurrentAccountInput, _client);

                    }
                    else
                    {
                        strSql = string.Format(@"select Oid, EntityName from DocumentFinanceMaster where DocumentType = '{0}'
                                        AND EntityName = '{1}' GROUP BY Oid, EntityName", xpoOidDocumentFinanceTypeCurrentAccountInput, _client);

                    }
                }
                else
                {
                    if (_statePay == Resx.NotPaid)
                    {
                        strSql = string.Format(@"select d.Oid, m.EntityName from DocumentFinanceDetail d, DocumentFinanceMaster m  
                                                where d.DocumentMaster = m.Oid and m.Payed = '0' and m.DocumentType = '{0}'
                                                AND m.EntityName = '{1}'
                                                GROUP BY d.Oid, m.EntityName", xpoOidDocumentFinanceTypeCurrentAccountInput, _client);


                    }
                    else if (_statePay == Resx.Paid)
                    {
                        strSql = string.Format(@"select d.Oid, m.EntityName from DocumentFinanceDetail d, DocumentFinanceMaster m 
                                                where d.DocumentMaster = m.Oid and m.Payed = '1' and m.DocumentType = '{0}'
                                                AND m.EntityName = '{1}'
                                                GROUP BY d.Oid, m.EntityName", xpoOidDocumentFinanceTypeCurrentAccountInput, _client);

                    }
                    else
                    {
                        strSql = string.Format(@"select d.Oid, m.EntityName from DocumentFinanceDetail d, DocumentFinanceMaster m 
                                                where d.DocumentMaster = m.Oid and m.DocumentType = '{0}'
                                                AND m.EntityName = '{1}'
                                                GROUP BY d.Oid, m.EntityName", xpoOidDocumentFinanceTypeCurrentAccountInput, _client);

                    }

                }


                xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(strSql);

                foreach (SelectStatementResultRow parentRow in xPSelectData.Data)
                {
                    OidDocumentFinanceMaster = parentRow.Values[xPSelectData.GetFieldIndex("Oid")].ToString();

                    listDocumentFinanceMaster.Add(OidDocumentFinanceMaster);

                }

                result = listDocumentFinanceMaster.Union(listDocumentFinanceMaster).ToList();


            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetClientsByName(e.Message): {0}", ex.Message), ex);

            }

            if (result.Count != 0)
            {
                if (_report.Equals(fileReport_CurrentAccount))
                {

                    data = DataReportsXML.GetDataDocumentFinanceMaster(result, _startDate, _endDate);
                }
                else
                {
                    data = DataReportsXML.GetDataDocumentFinanceDetail_Clients(result, _startDate, _endDate);
                }
            }


            return data;

        }

        public static DataSet GetTerminalByDesignation(string _terminal, string _startDate, string _endDate)
        {
            DataSet data = new DataSet();
            string strSql = "";
            XPSelectData xPSelectData = null;
            string OidTerminal = "";
            List<string> listTerminal = new List<string>();
            List<string> result = new List<string>();

            try
            {

                strSql = string.Format(@"select m.Oid from ConfigurationPlaceTerminal t, DocumentFinanceMaster m  where t.Designation = '" + _terminal + "' and m.UpdatedWhere = t.Oid");

                xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(strSql);

                foreach (SelectStatementResultRow parentRow in xPSelectData.Data)
                {
                    OidTerminal = parentRow.Values[xPSelectData.GetFieldIndex("Oid")].ToString();

                    listTerminal.Add(OidTerminal);

                }

                result = listTerminal.Union(listTerminal).ToList();


            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetTerminalByDesignation_OidTerminal(e.Message): {0}", ex.Message), ex);

            }

            if (result.Count != 0)
            {
                data = DataReportsXML.GetDataDocumentFinanceMaster(result, _startDate, _endDate);

            }


            return data;

        }


        public static DataSet GetArticles(string _family)
        {
            DataSet data = new DataSet();
            string strSql = "";
            XPSelectData xPSelectData = null;
            string OidArticle = "";
            List<string> listArticle = new List<string>();
            List<string> result = new List<string>();

            try
            {

                strSql = string.Format(@"select a.Oid from ArticleFamily f, ArticleSubFamily sf, Article a where f.Designation = '" + _family + "' and f.Oid = a.Family");

                xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(strSql);

                foreach (SelectStatementResultRow parentRow in xPSelectData.Data)
                {
                    OidArticle = parentRow.Values[xPSelectData.GetFieldIndex("Oid")].ToString();

                    listArticle.Add(OidArticle);
                }

                result = listArticle.Union(listArticle).ToList();
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetArticleFamilyByDesignation_OidArticleFamily(e.Message): {0}", ex.Message), ex);

            }


            if (result.Count != 0)
            {
                data = DataReportsXML.GetDataArticles(result);

            }

            return data;

        }


        public static DataSet GetArticleFamilyByDesignation(string _family, string _startDate, string _endDate)
        {
            DataSet data = new DataSet();
            string strSql = "";
            XPSelectData xPSelectData = null;
            string OidDocumentFinanceDetail = "";
            List<string> listOidDocumentFinanceDetail = new List<string>();
            List<string> result = new List<string>();

            try
            {

                strSql = string.Format(@"select d.Oid from DocumentFinanceDetail d, ArticleFamily f, ArticleSubFamily sf, Article a where f.Designation = '" + _family + "' and f.Oid = a.Family and a.Oid = d.Article");

                xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(strSql);

                foreach (SelectStatementResultRow parentRow in xPSelectData.Data)
                {
                    OidDocumentFinanceDetail = parentRow.Values[xPSelectData.GetFieldIndex("Oid")].ToString();
                    listOidDocumentFinanceDetail.Add(OidDocumentFinanceDetail);

                }


                result = listOidDocumentFinanceDetail.Union(listOidDocumentFinanceDetail).ToList();


            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetArticleFamilyByDesignation_OidArticleFamily(e.Message): {0}", ex.Message), ex);

            }



            if (result.Count != 0)
            {
                data = DataReportsXML.GetDataDocumentOrderDetail(result, _startDate, _endDate);
                return data;
            }

            return data;

        }

        //public static void GetTotalCustomers(string _customer, string _startDate, string _endDate)
        //{
        //    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
        //    doc.Load(FReportsFolder + "pos.xml");


        //    string docfm = "documentfinancemaster";
        //    string ud = "userdetail";


        //    XmlNodeList documentfinancemaster = doc.GetElementsByTagName(docfm);
        //    XmlNodeList userdetail = doc.GetElementsByTagName(ud);


        //    DataTable dDocumentFinanceMaster = new DataTable();
        //    DataTable dUserdetail = new DataTable();
        //    DataRow r;
        //    string value;
        //    string name;

        //    dDocumentFinanceMaster.Columns.Add("name");
        //    dDocumentFinanceMaster.Columns.Add("Value");
        //    dUserdetail.Columns.Add("name");
        //    dUserdetail.Columns.Add("Value");

        //    try
        //    {
        //        //search in xml - DocumentFinanceMaster
        //        foreach (XmlNode sDocFinancMaster in documentfinancemaster)
        //        {
        //            for (int i = 0; i < sDocFinancMaster.ChildNodes.Count; i++)
        //            {
        //                if (sDocFinancMaster.ChildNodes[i].LocalName != null)
        //                {
        //                    name = sDocFinancMaster.ChildNodes[i].LocalName;
        //                    if (sDocFinancMaster.ChildNodes[i].LastChild != null)
        //                    {
        //                        value = sDocFinancMaster.ChildNodes[i].LastChild.Value;
        //                    }
        //                    else
        //                    {
        //                        value = "";
        //                    }
        //                    r = dDocumentFinanceMaster.NewRow();
        //                    r[0] = name;
        //                    r[1] = value;

        //                    dDocumentFinanceMaster.Rows.Add(r);

        //                }
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _log.Error(string.Format("GetTotalCustomers_documentfinancemaster(e.Message): {0}", ex.Message), ex);

        //    }

        //    try
        //    {
        //        //search in xml - DocumentFinanceMaster
        //        foreach (XmlNode user in userdetail)
        //        {
        //            for (int i = 0; i < user.ChildNodes.Count; i++)
        //            {
        //                if (user.ChildNodes[i].LocalName != null)
        //                {
        //                    name = user.ChildNodes[i].LocalName;
        //                    if (user.ChildNodes[i].LastChild != null)
        //                    {
        //                        value = user.ChildNodes[i].LastChild.Value;
        //                    }
        //                    else
        //                    {
        //                        value = "";
        //                    }
        //                    r = dUserdetail.NewRow();
        //                    r[0] = name;
        //                    r[1] = value;

        //                    dUserdetail.Rows.Add(r);

        //                }
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _log.Error(string.Format("GetTotalCustomers_Userdetail(e.Message): {0}", ex.Message), ex);

        //    }



        //    string strSql = "";
        //    XPSelectData xPSelectData = null;
        //    string OidCustomer = "";
        //    string OidDocumentFinanceMaster = "";
        //    DataTable dDate = new DataTable();
        //    List<string> listOidCustomers = new List<string>();
        //    List<string> listOidDocumentFinanceMaster = new List<string>();
        //    List<string> resultCustomers = new List<string>();
        //    List<string> result = new List<string>();

        //    try
        //    {
        //        for (int f = 0; f < dUserdetail.Rows.Count; f++)
        //        {
        //            //select row Oid IN DocumentFinanceMaster
        //            if (dUserdetail.Rows[f].ItemArray[0].ToString().Equals("name"))
        //            {

        //                strSql = string.Format(@"select Oid from UserDetail where name = '" + _customer + "'");

        //                xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(strSql);

        //                foreach (SelectStatementResultRow parentRow in xPSelectData.Data)
        //                {

        //                    OidCustomer = parentRow.Values[xPSelectData.GetFieldIndex("Oid")].ToString();

        //                    listOidCustomers.Add(OidCustomer);



        //                }

        //                resultCustomers = listOidCustomers.Union(listOidCustomers).ToList();

        //            }
        //        }

        //        for (int f = 0; f < dDocumentFinanceMaster.Rows.Count; f++)
        //        {
        //            //select row Oid IN DocumentFinanceMaster
        //            if (dDocumentFinanceMaster.Rows[f].ItemArray[0].ToString().Equals("UserDetail"))
        //            {
        //                for (int i = 0; i < resultCustomers.Count; i++)
        //                {
        //                    strSql = string.Format(@"select Oid from DocumentFinanceMaster where UserDetail = '" + resultCustomers[i] + "'");

        //                    xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(strSql);

        //                    foreach (SelectStatementResultRow parentRow in xPSelectData.Data)
        //                    {
        //                        OidDocumentFinanceMaster = parentRow.Values[xPSelectData.GetFieldIndex("Oid")].ToString();

        //                        listOidDocumentFinanceMaster.Add(OidDocumentFinanceMaster);

        //                    }

        //                    result = listOidDocumentFinanceMaster.Union(listOidDocumentFinanceMaster).ToList();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _log.Error(string.Format("GetTotalCustomers_listOidDocumentFinanceMaster(e.Message): {0}", ex.Message), ex);

        //    }

        //    if (result.Count != 0)
        //    {
        //        DataReportsXML.GetDataDocumentFinanceMaster(result, _startDate, _endDate);
        //    }
        //    else
        //    {
        //        FormMessageNotFound frmMessageNotFound = new FormMessageNotFound(_startDate, _endDate);
        //        frmMessageNotFound.ShowDialog();

        //        //if (GlobalFramework.CurrentCulture.Name != "pt-PT")
        //        //{
        //        //    MessageBox.Show(Utils.GetWindowTitle(Resx_en_US.Msg_NoRecordsWereFound), "POS.ON", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        //}
        //        //else
        //        //{
        //        //    MessageBox.Show(Utils.GetWindowTitle(Resx_pt_PT.Msg_NoRecordsWereFound), "POS.ON", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        //}
        //    }

        //}


        public static DataSet GetMovCustomers(string _customer, string _startDate, string _endDate)
        {
            string strSql = "";
            XPSelectData xPSelectData = null;
            string OidSystemAudit = "";
            DataSet data = new DataSet();
            List<string> listOidSystemAudit = new List<string>();
            List<string> result = new List<string>();

            try
            {
                strSql = string.Format(@"select sa.Oid from UserDetail u, SystemAudit sa where u.name = '" + _customer + "' and sa.Userdetail = u.Oid");

                xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(strSql);

                foreach (SelectStatementResultRow parentRow in xPSelectData.Data)
                {
                    OidSystemAudit = parentRow.Values[xPSelectData.GetFieldIndex("Oid")].ToString();

                    listOidSystemAudit.Add(OidSystemAudit);

                }

                result = listOidSystemAudit.Union(listOidSystemAudit).ToList();

            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetSystemAudit_listOidSystemAudit(e.Message): {0}", ex.Message), ex);

            }

            if (result.Count != 0)
            {
                data = DataReportsXML.GetDataSystemAudit(result, _startDate, _endDate);
            }


            return data;

        }

        public static List<string> SplitDate(string _startDate, string _endDate)
        {
            string startDate = "";
            string endDate = "";
            List<string> dates = new List<string>();
            try
            {
                string[] splitStartDate = _startDate.Split(' ');
                string[] splitEndDate = _endDate.Split(' ');

                string startDateSplit = splitStartDate[0];
                string endDateSplit = splitEndDate[0];

                string[] splitSD = startDateSplit.Split('/');
                string[] splitED = endDateSplit.Split('/');

                string daySD = splitSD[0];
                string monthSD = splitSD[1];
                string yearSD = splitSD[2];

                string dayED = splitED[0];
                string monthED = splitED[1];
                string yearED = splitED[2];


                startDate = Convert.ToString(yearSD + "-" + monthSD + "-" + daySD);
                endDate = Convert.ToString(yearED + "-" + monthED + "-" + dayED);

                dates.Add(startDate);
                dates.Add(endDate);


            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
            return dates;
        }


        //public static string GetSystemAuditByDate(string _startDate, string _endDate)
        //{
        //    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
        //    doc.Load(FReportsFolder + "pos.xml");

        //    string sAudit = "systemaudit";


        //    XmlNodeList systemAudit = doc.GetElementsByTagName(sAudit);


        //    DataTable dSystemAudit = new DataTable();
        //    DataRow r;
        //    string value;
        //    string name;

        //    dSystemAudit.Columns.Add("name");
        //    dSystemAudit.Columns.Add("Value");

        //    try
        //    {
        //        //search in xml - DocumentFinanceMaster
        //        foreach (XmlNode audit in systemAudit)
        //        {
        //            for (int i = 0; i < audit.ChildNodes.Count; i++)
        //            {
        //                if (audit.ChildNodes[i].LocalName != null)
        //                {
        //                    name = audit.ChildNodes[i].LocalName;
        //                    if (audit.ChildNodes[i].LastChild != null)
        //                    {
        //                        value = audit.ChildNodes[i].LastChild.Value;
        //                    }
        //                    else
        //                    {
        //                        value = "";
        //                    }
        //                    r = dSystemAudit.NewRow();
        //                    r[0] = name;
        //                    r[1] = value;

        //                    dSystemAudit.Rows.Add(r);

        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _log.Error(string.Format("GetSystemAuditByDate_systemaudit(e.Message): {0}", ex.Message), ex);

        //    }


        //    string strSql = "";
        //    XPSelectData xPSelectData = null;
        //    string OidDocumentFinanceMaster = "";
        //    string startDate = "";
        //    string endDate = "";
        //    DataTable dDate = new DataTable();
        //    List<string> listOidDates = new List<string>();
        //    List<string> result = new List<string>();

        //    try
        //    {
        //        for (int f = 0; f < dSystemAudit.Rows.Count; f++)
        //        {
        //            //select row Oid IN DocumentFinanceMaster
        //            if (dSystemAudit.Rows[f].ItemArray[0].ToString().Equals("date"))
        //            {
        //                // string dt = dDocumentFinanceDetail.Rows[f].ItemArray[1].ToString();

        //                string[] splitStartDate = _startDate.Split(' ');
        //                string[] splitEndDate = _endDate.Split(' ');

        //                string startDateSplit = splitStartDate[0];
        //                string endDateSplit = splitEndDate[0];

        //                string[] splitSD = startDateSplit.Split('/');
        //                string[] splitED = endDateSplit.Split('/');

        //                string daySD = splitSD[0];
        //                string monthSD = splitSD[1];
        //                string yearSD = splitSD[2];

        //                string dayED = splitED[0];
        //                string monthED = splitED[1];
        //                string yearED = splitED[2];


        //                startDate = Convert.ToString(yearSD + "-" + monthSD + "-" + daySD);
        //                endDate = Convert.ToString(yearED + "-" + monthED + "-" + dayED);

        //                if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
        //                {
        //                    strSql = @"select Oid from SystemAudit where CAST(Date AS DATE) between '" + startDate + "' and '" + endDate + "'";
        //                }
        //                else
        //                {
        //                    strSql = @"select Oid from SystemAudit where Date between '" + startDate + "' and '" + endDate + "'";

        //                }

        //                try
        //                {
        //                    xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(strSql);


        //                    foreach (SelectStatementResultRow parentRow in xPSelectData.Data)
        //                    {

        //                        OidDocumentFinanceMaster = parentRow.Values[xPSelectData.GetFieldIndex("Oid")].ToString();

        //                        listOidDates.Add(OidDocumentFinanceMaster);

        //                    }

        //                    result = listOidDates.Union(listOidDates).ToList();
        //                }
        //                catch (Exception ex)
        //                {
        //                    _log.Error(string.Format("GetPayment(e.Message): {0}", ex.Message), ex);

        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _log.Error(string.Format("GetSystemAuditByDate_listOidDates(e.Message): {0}", ex.Message), ex);

        //    }

        //    if (result.Count != 0)
        //    {
        //        DataReportsXML.GetDataSystemAudit(result, _startDate, _endDate);
        //    }

        //    return result.Count.ToString();
        //}


        //public static string GetWorkSessionMovementByDate(string _startDate, string _endDate)
        //{
        //    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
        //    doc.Load(FReportsFolder + "pos.xml");

        //    string sMov = "worksessionmovement";

        //    XmlNodeList workmovement = doc.GetElementsByTagName(sMov);


        //    DataTable dWorkMovement = new DataTable();
        //    DataRow r;
        //    string value;
        //    string name;

        //    dWorkMovement.Columns.Add("name");
        //    dWorkMovement.Columns.Add("Value");

        //    try
        //    {
        //        //search in xml - DocumentFinanceMaster
        //        foreach (XmlNode wmov in workmovement)
        //        {
        //            for (int i = 0; i < wmov.ChildNodes.Count; i++)
        //            {
        //                if (wmov.ChildNodes[i].LocalName != null)
        //                {
        //                    name = wmov.ChildNodes[i].LocalName;
        //                    if (wmov.ChildNodes[i].LastChild != null)
        //                    {
        //                        value = wmov.ChildNodes[i].LastChild.Value;
        //                    }
        //                    else
        //                    {
        //                        value = "";
        //                    }
        //                    r = dWorkMovement.NewRow();
        //                    r[0] = name;
        //                    r[1] = value;

        //                    dWorkMovement.Rows.Add(r);

        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _log.Error(string.Format("GetWorkSessionMovementByDate(e.Message): {0}", ex.Message), ex);

        //    }


        //    string strSql = "";
        //    XPSelectData xPSelectData = null;
        //    string OidWorkSessionMovement = "";
        //    string startDate = "";
        //    string endDate = "";
        //    DataTable dDate = new DataTable();
        //    List<string> listOidDates = new List<string>();
        //    List<string> result = new List<string>();

        //    try
        //    {
        //        for (int f = 0; f < dWorkMovement.Rows.Count; f++)
        //        {
        //            //select row Oid IN DocumentFinanceMaster
        //            if (dWorkMovement.Rows[f].ItemArray[0].ToString().Equals("date"))
        //            {
        //                // string dt = dDocumentFinanceDetail.Rows[f].ItemArray[1].ToString();

        //                string[] splitStartDate = _startDate.Split(' ');
        //                string[] splitEndDate = _endDate.Split(' ');

        //                string startDateSplit = splitStartDate[0];
        //                string endDateSplit = splitEndDate[0];

        //                string[] splitSD = startDateSplit.Split('/');
        //                string[] splitED = endDateSplit.Split('/');

        //                string daySD = splitSD[0];
        //                string monthSD = splitSD[1];
        //                string yearSD = splitSD[2];

        //                string dayED = splitED[0];
        //                string monthED = splitED[1];
        //                string yearED = splitED[2];


        //                startDate = Convert.ToString(yearSD + "-" + monthSD + "-" + daySD);
        //                endDate = Convert.ToString(yearED + "-" + monthED + "-" + dayED);

        //                if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
        //                {
        //                    strSql = @"select Oid from WorkSessionMovement where CAST(Date AS DATE) between '" + startDate + "' and '" + endDate + "'";
        //                }
        //                else
        //                {
        //                    strSql = @"select Oid from WorkSessionMovement where Date between '" + startDate + "' and '" + endDate + "'";

        //                }

        //                try
        //                {
        //                    xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(strSql);


        //                    foreach (SelectStatementResultRow parentRow in xPSelectData.Data)
        //                    {

        //                        OidWorkSessionMovement = parentRow.Values[xPSelectData.GetFieldIndex("Oid")].ToString();

        //                        listOidDates.Add(OidWorkSessionMovement);

        //                    }

        //                    result = listOidDates.Union(listOidDates).ToList();
        //                }
        //                catch (Exception ex)
        //                {
        //                    _log.Error(string.Format("GetPayment(e.Message): {0}", ex.Message), ex);

        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _log.Error(string.Format("GetWorkSessionMovementByDate_listOidDates(e.Message): {0}", ex.Message), ex);

        //    }

        //    if (result.Count != 0)
        //    {
        //        DataReportsXML.GetDataWorkSessionMovement(result, _startDate, _endDate);
        //    }

        //    return result.Count.ToString();
        //}



        public static DataSet GetFinanceDetailByDate(bool documentFiscal_CC, string _startDate, string _endDate)
        {
            // System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            // doc.Load(FReportsFolder + "pos.xml");
            //FormMessageNotFound frmMessageNotFound = new FormMessageNotFound(_startDate, _endDate);
            DataSet dataDocumentFinanceDetail = new DataSet();

            string strSql = "";
            XPSelectData xPSelectData = null;
            string OidDocumentFinanceDetail = "";
            string startDate = "";
            string endDate = "";
            DataTable dDate = new DataTable();
            List<string> listOidDates = new List<string>();
            List<string> result = new List<string>();

            try
            {
                string[] splitStartDate = _startDate.Split(' ');
                string[] splitEndDate = _endDate.Split(' ');

                string startDateSplit = splitStartDate[0];
                string endDateSplit = splitEndDate[0];

                string[] splitSD = startDateSplit.Split('/');
                string[] splitED = endDateSplit.Split('/');

                string daySD = splitSD[0];
                string monthSD = splitSD[1];
                string yearSD = splitSD[2];

                string dayED = splitED[0];
                string monthED = splitED[1];
                string yearED = splitED[2];

                startDate = Convert.ToString(yearSD + "-" + monthSD + "-" + daySD);
                endDate = Convert.ToString(yearED + "-" + monthED + "-" + dayED);


                if (documentFiscal_CC != true)
                {
                    strSql = string.Format(@"select d.Oid as OidDetail, d.Code, d.Designation, d.Quantity, d.TotalFinal, d.DocumentMaster, d.Article, m.Oid as OidMaster, m.DocumentNumber, m.EntityName, m.Date, m.Payed, m.PayedDate
                                        from DocumentFinanceDetail d, DocumentFinanceMaster m where d.DocumentMaster = m.Oid and 
                                        CAST(m.Date as DATE) between '" + startDate + "' and '" + endDate + "'");
                }

                else
                {
                    strSql = string.Format(@"select d.Oid as OidDetail, d.Code, d.Designation, d.Quantity, d.TotalFinal, d.DocumentMaster, d.Article, m.Oid as OidMaster, m.DocumentNumber, m.EntityName, m.Date, m.Payed, m.PayedDate
                                        from DocumentFinanceDetail d, DocumentFinanceMaster m where d.DocumentMaster = m.Oid and 
                                        CAST(m.Date as DATE) between '" + startDate + "' and '" + endDate + "' and DocumentType = '" + xpoOidDocumentFinanceTypeCurrentAccountInput + "'");
                }


                xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(strSql);

                foreach (SelectStatementResultRow parentRow in xPSelectData.Data)
                {

                    OidDocumentFinanceDetail = parentRow.Values[xPSelectData.GetFieldIndex("OidDetail")].ToString();

                    listOidDates.Add(OidDocumentFinanceDetail);



                }

                result = listOidDates.Union(listOidDates).ToList();

            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetFinanceMasterByDate_listOidDates(e.Message): {0}", ex.Message), ex);

            }

            if (result.Count != 0)
            {
                dataDocumentFinanceDetail = DataReportsXML.GetDataDocumentFinanceDetail_Clients(result, _startDate, _endDate);



                _log.Debug(string.Format("GetFinanceMasterByDate"));
            }


            return dataDocumentFinanceDetail;

        }

        public static DataSet GetFinanceMasterByDate(bool documentFiscal_CC, string _startDate, string _endDate)
        {
            // System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            // doc.Load(FReportsFolder + "pos.xml");
            //FormMessageNotFound frmMessageNotFound = new FormMessageNotFound(_startDate, _endDate);
            DataSet dataDocumentFinanceMaster = new DataSet();

            String xpoOidDocumentFinanceTypeCurrentAccountInput = GlobalFramework.Settings["xpoOidDocumentFinanceTypeCurrentAccountInput"];

            string strSql = "";
            XPSelectData xPSelectData = null;
            string OidDocumentFinanceMaster = "";
            string startDate = "";
            string endDate = "";
            DataTable dDate = new DataTable();
            List<string> listOidDates = new List<string>();
            List<string> result = new List<string>();

            try
            {
                string[] splitStartDate = _startDate.Split(' ');
                string[] splitEndDate = _endDate.Split(' ');

                string startDateSplit = splitStartDate[0];
                string endDateSplit = splitEndDate[0];

                string[] splitSD = startDateSplit.Split('/');
                string[] splitED = endDateSplit.Split('/');

                string daySD = splitSD[0];
                string monthSD = splitSD[1];
                string yearSD = splitSD[2];

                string dayED = splitED[0];
                string monthED = splitED[1];
                string yearED = splitED[2];

                startDate = Convert.ToString(yearSD + "-" + monthSD + "-" + daySD);
                endDate = Convert.ToString(yearED + "-" + monthED + "-" + dayED);

                if (documentFiscal_CC != true)
                {
                    strSql = string.Format(@"select Oid from DocumentFinanceMaster where CAST(Date as DATE) between '" + startDate + "' and '" + endDate + "'");
                }

                else
                {
                    strSql = string.Format(@"select Oid from DocumentFinanceMaster where CAST(Date as DATE) between '" + startDate + "' and '" + endDate + "' and DocumentType = '" + xpoOidDocumentFinanceTypeCurrentAccountInput + "'");
                }

                xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(strSql);

                foreach (SelectStatementResultRow parentRow in xPSelectData.Data)
                {

                    OidDocumentFinanceMaster = parentRow.Values[xPSelectData.GetFieldIndex("Oid")].ToString();

                    listOidDates.Add(OidDocumentFinanceMaster);



                }

                result = listOidDates.Union(listOidDates).ToList();

            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetFinanceMasterByDate_listOidDates(e.Message): {0}", ex.Message), ex);

            }

            if (result.Count != 0)
            {
                dataDocumentFinanceMaster = DataReportsXML.GetDataDocumentFinanceMaster(result, _startDate, _endDate);



                _log.Debug(string.Format("GetFinanceMasterByDate"));
            }


            return dataDocumentFinanceMaster;

        }


        public static DataSet GetTypePaymentByDesignation(string _typePayment, string _startDate, string _endDate)
        {
            string strSql = "";
            XPSelectData xPSelectData = null;
            string OidWorkSessionMovement = "";
            DataSet data = new DataSet();
            List<string> listOidWorkSessionMovement = new List<string>();
            List<string> result = new List<string>();

            try
            {

                strSql = string.Format(@"select w.Oid from ConfigurationPaymentMethod p, DocumentFinanceMaster m, WorkSessionMovement w where p.Designation = '" + _typePayment + "' and m.PaymentMethod = p.Oid and w.DocumentFinanceMaster = m.Oid");

                xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(strSql);

                foreach (SelectStatementResultRow parentRow in xPSelectData.Data)
                {
                    OidWorkSessionMovement = parentRow.Values[xPSelectData.GetFieldIndex("Oid")].ToString();

                    listOidWorkSessionMovement.Add(OidWorkSessionMovement);

                }

                result = listOidWorkSessionMovement.Union(listOidWorkSessionMovement).ToList();



            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetSystemAudit_listOidSystemAudit(e.Message): {0}", ex.Message), ex);

            }

            if (result.Count != 0)
            {
                data = DataReportsXML.GetDataWorkSessionMovement(result, _startDate, _endDate);
            }

            return data;

        }

        public static DataSet GetTypeMovementByDesignation(string _typeMovement, string _startDate, string _endDate)
        {
            string strSql = "";
            XPSelectData xPSelectData = null;
            string OidWorkSessionMovementType = "";
            DataSet data = new DataSet();
            List<string> resultMovementType = new List<string>();
            List<string> result = new List<string>();

            try
            {
                strSql = string.Format(@"select m.Oid from WorkSessionMovementType mt, worksessionmovement m where mt.Designation = '" + _typeMovement + "' and m.WorkSessionMovementType = mt.Oid'");

                xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(strSql);

                foreach (SelectStatementResultRow parentRow in xPSelectData.Data)
                {
                    OidWorkSessionMovementType = parentRow.Values[xPSelectData.GetFieldIndex("Oid")].ToString();

                    resultMovementType.Add(OidWorkSessionMovementType);

                }

                result = resultMovementType.Union(resultMovementType).ToList();

            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetWorkSessionMovement_listOidWorkSessionMovement(e.Message): {0}", ex.Message), ex);

            }

            if (result.Count != 0)
            {
                data = DataReportsXML.GetDataWorkSessionMovement(result, _startDate, _endDate);
            }


            return data;

        }


        public static DataTable GetTotalsCustomer()
        {
            string nameTable = "";

            DataTable table = new DataTable();


            string strSql = @"select u.name as name from userdetail u, documentfinancemaster a where a.UpdatedBy = u.Oid GROUP BY u.name;";

            strSql = string.Format(strSql);

            XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(strSql);

            table.Columns.Add("Name");


            table.Rows.Add(Resx.All);


            try
            {
                foreach (SelectStatementResultRow parentRow in xPSelectData.Data)
                {

                    nameTable = parentRow.Values[xPSelectData.GetFieldIndex("name")].ToString();

                    table.Rows.Add(nameTable);

                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetTotalsCustomer(e.Message): {0}", ex.Message), ex);

            }

            return table;
        }

        public static void DatesReport(string _startDate, string _endDate, string _filenameReport)
        {
            FReportsFolder = (string.Format("{0}", GlobalFramework.Path["reports"]));

            string[] filePaths = Directory.GetFiles(FReportsFolder, "*.frx");

            string startDate = Convert.ToDateTime(_startDate).Date.ToString(GlobalFramework.Settings["dateTimeFormatDocumentDate"]);
            string endDate = Convert.ToDateTime(_endDate).Date.ToString(GlobalFramework.Settings["dateTimeFormatDocumentDate"]);


            try
            {
                for (int i = 0; i < filePaths.Length; i++)
                {
                    if (_filenameReport.Equals(""))
                    {
                        string fileReport = filePaths[i].ToString();

                        XDocument doc = XDocument.Load(fileReport);
                        IEnumerable<XElement> elements = doc.Elements("Report");

                        foreach (XElement node in elements)
                        {
                            XElement attachment;

                            if (node.Element("ReportPage").Element("ReportTitleBand") != null)
                            {
                                attachment = node.Element("ReportPage").Element("ReportTitleBand");

                                foreach (XElement child in attachment.Elements("TextObject"))
                                {

                                    if (child.Attribute("Name").Value.Equals("Text_StartDate"))
                                    {
                                        child.Attribute("Text").Value = startDate;

                                    }

                                    if (child.Attribute("Name").Value.Equals("Text_EndDate"))
                                    {
                                        child.Attribute("Text").Value = endDate;

                                    }
                                }

                            }
                            doc.Save(fileReport);
                        }

                    }
                    else
                    {

                        if (filePaths[i].ToString().Equals(_filenameReport))
                        {
                            string fileReport = filePaths[i].ToString();

                            XDocument doc = XDocument.Load(fileReport);
                            IEnumerable<XElement> elements = doc.Elements("Report");

                            foreach (XElement node in elements)
                            {
                                XElement attachment;

                                if (node.Element("ReportPage").Element("ReportTitleBand") != null)
                                {
                                    attachment = node.Element("ReportPage").Element("ReportTitleBand");

                                    foreach (XElement child in attachment.Elements("TextObject"))
                                    {

                                        if (child.Attribute("Name").Value.Equals("Text_StartDate"))
                                        {
                                            child.Attribute("Text").Value = startDate;

                                        }

                                        if (child.Attribute("Name").Value.Equals("Text_EndDate"))
                                        {
                                            child.Attribute("Text").Value = endDate;

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
                _log.Error(string.Format("DatesReport(e.Message): {0}", ex.Message), ex);

            }


        }

        public static DataSet GetRecordsCustomerByDesignation(string _customer, string _startDate, string _endDate)
        {
            string strSql = "";
            XPSelectData xPSelectData = null;
            DataSet data = new DataSet();
            string OidDocumentFinanceDetail = "";
            List<string> listDocumentFinanceDetail = new List<string>();
            List<string> result = new List<string>();

            try
            {

                strSql = string.Format(@"select m.Oid from UserDetail u, DocumentFinanceMaster m where u.name ='" + _customer + "' and m.UpdatedBy = u.Oid");

                xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(strSql);

                foreach (SelectStatementResultRow parentRow in xPSelectData.Data)
                {
                    OidDocumentFinanceDetail = parentRow.Values[xPSelectData.GetFieldIndex("Oid")].ToString();
                    listDocumentFinanceDetail.Add(OidDocumentFinanceDetail);
                }

                result = listDocumentFinanceDetail.Union(listDocumentFinanceDetail).ToList();

            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetArticleFamilyByDesignation_OidArticleFamily(e.Message): {0}", ex.Message), ex);

            }



            if (result.Count != 0)
            {
                data = DataReportsXML.GetDataDocumenFinanceDetail(result, _startDate, _endDate);

            }

            return data;
        }

        public static DataSet GetCustomerByDesignation(string _customer, string _startDate, string _endDate)
        {
            string strSql = "";
            XPSelectData xPSelectData = null;
            DataSet data = new DataSet();
            string OidDocumentFinanceDetail = "";
            List<string> listDocumentFinanceDetail = new List<string>();
            List<string> result = new List<string>();

            try
            {

                strSql = string.Format(@"select m.Oid from UserDetail u, DocumentFinanceMaster m where u.name ='" + _customer + "' and m.UpdatedBy = u.Oid");

                xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(strSql);

                foreach (SelectStatementResultRow parentRow in xPSelectData.Data)
                {
                    OidDocumentFinanceDetail = parentRow.Values[xPSelectData.GetFieldIndex("Oid")].ToString();
                    listDocumentFinanceDetail.Add(OidDocumentFinanceDetail);

                }



                result = listDocumentFinanceDetail.Union(listDocumentFinanceDetail).ToList();


            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetArticleFamilyByDesignation_OidArticleFamily(e.Message): {0}", ex.Message), ex);

            }



            if (result.Count != 0)
            {
                data = DataReportsXML.GetDataDocumentFinanceMaster(result, _startDate, _endDate);

            }

            return data;
        }



        public static DataSet GetStockArticlesByDates(string _startDate, string _endDate)
        {
            DataSet data = new DataSet();

            data = DataReportsXML.GetDataStockArticle(_startDate, _endDate);

            return data;
        }
    }
}