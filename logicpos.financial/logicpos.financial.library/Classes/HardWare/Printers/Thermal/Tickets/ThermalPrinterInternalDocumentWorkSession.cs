using DevExpress.Xpo.DB;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.financial.library.App;
using logicpos.financial.library.Classes.Hardware.Printers.Thermal.Enums;
using logicpos.financial.library.Classes.WorkSession;
using logicpos.resources.Resources.Localization;
using logicpos.shared.Enums.ThermalPrinter;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace logicpos.financial.library.Classes.Hardware.Printers.Thermal.Tickets
{
    public class ThermalPrinterInternalDocumentWorkSession : ThermalPrinterBaseInternalTemplate
    {
        private pos_worksessionperiod _workSessionPeriod;
        private SplitCurrentAccountMode _splitCurrentAccountMode;

        public ThermalPrinterInternalDocumentWorkSession(sys_configurationprinters pPrinter, pos_worksessionperiod pWorkSessionPeriod, SplitCurrentAccountMode pSplitCurrentAccountMode)
            : base(pPrinter)
        {
            _workSessionPeriod = pWorkSessionPeriod;
            _splitCurrentAccountMode = pSplitCurrentAccountMode;

            //Define TicketTitle for Day
            if (_workSessionPeriod.PeriodType == WorkSessionPeriodType.Day)
            {
                _ticketTitle = (pWorkSessionPeriod.SessionStatus == WorkSessionPeriodStatus.Open) ? resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "ticket_title_worksession_day_resume") : resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "ticket_title_worksession_day_close");
            }
            //Define TicketTitle/TicketSubTitle for Terminal
            else
            {
                _ticketTitle = (pWorkSessionPeriod.SessionStatus == WorkSessionPeriodStatus.Open) ? resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "ticket_title_worksession_terminal_resume") : resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "ticket_title_worksession_terminal_close");
                _ticketSubTitle = (pWorkSessionPeriod.PeriodType == WorkSessionPeriodType.Terminal) ? _workSessionPeriod.Terminal.Designation : string.Empty;
            }

            //Add Extra text to TicketSubTitle
            string ticketSubTitleExtra = string.Empty;
            switch (_splitCurrentAccountMode)
            {
                case SplitCurrentAccountMode.All:
                    break;
                case SplitCurrentAccountMode.NonCurrentAcount:
                    //Nao imprimir sub-titulo para contas não corrente
                    ticketSubTitleExtra = "";
                    //ticketSubTitleExtra = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_without_current_acount");
                    break;
                case SplitCurrentAccountMode.CurrentAcount:
                    ticketSubTitleExtra = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_current_account");
                    break;
            }

            //Generate Final TicketSubTitle
            if (_ticketSubTitle != string.Empty && ticketSubTitleExtra != string.Empty)
            {
                _ticketSubTitle = string.Format("{0} : ({1})", _ticketSubTitle, ticketSubTitleExtra);
            }
            else if (_ticketSubTitle == string.Empty && ticketSubTitleExtra != string.Empty)
            {
                _ticketSubTitle = string.Format("({0})", ticketSubTitleExtra);
            }
        }

        //Override Parent Template
        public override void PrintContent()
        {
            try
            {
                //Call Base Template PrintHeader
                base.PrintTitles(_ticketTitle);

                //Align Center
                _thermalPrinterGeneric.SetAlignCenter();

                PrintDocumentDetails();

                //Reset to Left
                _thermalPrinterGeneric.SetAlignLeft();

                //Line Feed
                _thermalPrinterGeneric.LineFeed();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void PrintDocumentDetails()
        {
            //Call PrintWorkSessionMovement
            PrintWorkSessionMovement(_thermalPrinterGeneric.Printer, _workSessionPeriod, _splitCurrentAccountMode);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Helper Logic

        class DataTableGroupProperties
        {
            private string _title;
            public string Title
            {
                get { return _title; }
                set { _title = value; }
            }

            private string _sql;
            public string Sql
            {
                get { return _sql; }
                set { _sql = value; }
            }

            private bool _enabled;
            public bool Enabled
            {
                get { return _enabled; }
                set { _enabled = value; }
            }

            public DataTableGroupProperties(string pTitle, string pSql) : this(pTitle, pSql, true) { }
            public DataTableGroupProperties(string pTitle, string pSql, bool pEnabled)
            {
                _title = pTitle;
                _sql = pSql;
                _enabled = pEnabled;
            }
        }

        public bool PrintWorkSessionMovement(sys_configurationprinters pPrinter, pos_worksessionperiod pWorkSessionPeriod, SplitCurrentAccountMode pSplitCurrentAccountMode)
        {
            bool result = false;

            if (pPrinter != null)
            {
                sys_configurationprinters printer = pPrinter;
                sys_configurationprinterstemplates template = (sys_configurationprinterstemplates)FrameworkUtils.GetXPGuidObject(typeof(sys_configurationprinterstemplates), SettingsApp.XpoOidConfigurationPrintersTemplateWorkSessionMovement);
                string splitCurrentAccountFilter = string.Empty;
                string fileTicket = template.FileTemplate;

                switch (pSplitCurrentAccountMode)
                {
                    case SplitCurrentAccountMode.All:
                        break;
                    case SplitCurrentAccountMode.NonCurrentAcount:
                        //Diferent from DocumentType CC
                        splitCurrentAccountFilter = string.Format("AND DocumentType <> '{0}'", SettingsApp.XpoOidDocumentFinanceTypeCurrentAccountInput);
                        break;
                    case SplitCurrentAccountMode.CurrentAcount:
                        //Only DocumentType CC
                        splitCurrentAccountFilter = string.Format("AND DocumentType = '{0}'", SettingsApp.XpoOidDocumentFinanceTypeCurrentAccountInput);
                        break;
                }

                try
                {
                    //Shared Where for details and totals Queries
                    string sqlWhere = string.Empty;

                    if (pWorkSessionPeriod.PeriodType == WorkSessionPeriodType.Day)
                    {
                        sqlWhere = string.Format("PeriodParent = '{0}'{1}", pWorkSessionPeriod.Oid, splitCurrentAccountFilter);
                    }
                    else
                    {
                        sqlWhere = string.Format("Period = '{0}'{1}", pWorkSessionPeriod.Oid, splitCurrentAccountFilter);
                    }

                    //Shared for Both Modes
                    if (sqlWhere != string.Empty) sqlWhere = string.Format(" AND {0}", sqlWhere);

                    //Format to Display Vars
                    string dateCloseDisplay = (pWorkSessionPeriod.SessionStatus == WorkSessionPeriodStatus.Open) ? resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_in_progress") : pWorkSessionPeriod.DateEnd.ToString(SettingsApp.DateTimeFormat);

                    //Get Session Period Details
                    Hashtable resultHashTable = ProcessWorkSessionPeriod.GetSessionPeriodSummaryDetails(pWorkSessionPeriod);

                    //Print Header Summary
                    DataRow dataRow = null;
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add(new DataColumn("Label", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("Value", typeof(string)));
                    //Open DateTime
                    dataRow = dataTable.NewRow();
                    dataRow[0] = string.Format("{0}:", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_worksession_open_datetime"));
                    dataRow[1] = pWorkSessionPeriod.DateStart.ToString(SettingsApp.DateTimeFormat);
                    dataTable.Rows.Add(dataRow);
                    //Close DataTime
                    dataRow = dataTable.NewRow();
                    dataRow[0] = string.Format("{0}:", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_worksession_close_datetime"));
                    dataRow[1] = dateCloseDisplay;
                    dataTable.Rows.Add(dataRow);
                    //Open Total CashDrawer
                    dataRow = dataTable.NewRow();
                    dataRow[0] = string.Format("{0}:", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_worksession_open_total_cashdrawer"));
                    dataRow[1] = FrameworkUtils.DecimalToStringCurrency((decimal)resultHashTable["totalMoneyInCashDrawerOnOpen"]);
                    dataTable.Rows.Add(dataRow);
                    //Close Total CashDrawer
                    dataRow = dataTable.NewRow();
                    dataRow[0] = string.Format("{0}:", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_worksession_close_total_cashdrawer"));
                    dataRow[1] = FrameworkUtils.DecimalToStringCurrency((decimal)resultHashTable["totalMoneyInCashDrawer"]);
                    dataTable.Rows.Add(dataRow);
                    //Total Money In
                    dataRow = dataTable.NewRow();
                    dataRow[0] = string.Format("{0}:", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_worksession_total_money_in"));
                    dataRow[1] = FrameworkUtils.DecimalToStringCurrency((decimal)resultHashTable["totalMoneyIn"]);
                    dataTable.Rows.Add(dataRow);
                    //Total Money Out
                    dataRow = dataTable.NewRow();
                    dataRow[0] = string.Format("{0}:", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_worksession_total_money_out"));
                    dataRow[1] = FrameworkUtils.DecimalToStringCurrency((decimal)resultHashTable["totalMoneyOut"]);
                    dataTable.Rows.Add(dataRow);
                    //Configure Ticket Column Properties
                    List<TicketColumn> columns = new List<TicketColumn>();
                    columns.Add(new TicketColumn("Label", "", Convert.ToInt16(_maxCharsPerLineNormal / 2) - 2, TicketColumnsAlign.Right));
                    columns.Add(new TicketColumn("Value", "", Convert.ToInt16(_maxCharsPerLineNormal / 2) - 2, TicketColumnsAlign.Left));
                    TicketTable ticketTable = new TicketTable(dataTable, columns, _thermalPrinterGeneric.MaxCharsPerLineNormalBold);
                    //Print Ticket Table
                    ticketTable.Print(_thermalPrinterGeneric);
                    //Line Feed
                    _thermalPrinterGeneric.LineFeed();

                    //Get Final Rendered DataTable Groups
                    Dictionary<DataTableGroupPropertiesType, DataTableGroupProperties> dictGroupProperties = GenDataTableWorkSessionMovementResume(pWorkSessionPeriod.PeriodType, pSplitCurrentAccountMode, sqlWhere);

                    //Prepare Local vars for Group Loop
                    XPSelectData xPSelectData = null;
                    string designation = string.Empty;
                    decimal quantity = 0.0m;
                    decimal total = 0.0m;
                    string unitMeasure = string.Empty;
                    //Store Final Totals
                    decimal summaryTotalQuantity = 0.0m;
                    decimal summaryTotal = 0.0m;
                    //Used to Custom Print Table Ticket Rows
                    List<string> tableCustomPrint = new List<string>();

                    //Start to process Group
                    int groupPosition = -1;
                    //Assign Position to Print Payment Group Split Title
                    int groupPositionTitlePayments = (pWorkSessionPeriod.PeriodType == WorkSessionPeriodType.Day) ? 9 : 8;
                    //If CurrentAccount Mode decrease 1, it dont have PaymentMethods
                    if (pSplitCurrentAccountMode == SplitCurrentAccountMode.CurrentAcount) groupPositionTitlePayments--;


                    foreach (KeyValuePair<DataTableGroupPropertiesType, DataTableGroupProperties> item in dictGroupProperties)
                    //foreach (DataTableGroupProperties item in dictGroupProperties.Values)
                    {
                        if (item.Value.Enabled)
                        {
                            //Increment Group Position
                            groupPosition++;

                            //Print Group Titles (FinanceDocuments|Payments)
                            if (groupPosition == 0)
                            {
                                _thermalPrinterGeneric.WriteLine(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_worksession_resume_finance_documents"), WriteLineTextMode.Big);
                                _thermalPrinterGeneric.LineFeed();
                            }
                            else if (groupPosition == groupPositionTitlePayments)
                            {
                                //When finish FinanceDocuemnts groups, print Last Row, the Summary Totals Row
                                _thermalPrinterGeneric.WriteLine(tableCustomPrint[tableCustomPrint.Count - 1], WriteLineTextMode.DoubleHeight);
                                _thermalPrinterGeneric.LineFeed();

                                _thermalPrinterGeneric.WriteLine(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_worksession_resume_paymens_documents"), WriteLineTextMode.Big);
                                _thermalPrinterGeneric.LineFeed();
                            }

                            //Reset Totals
                            summaryTotalQuantity = 0.0m;
                            summaryTotal = 0.0m;

                            //Get Group Data from group Query
                            xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(item.Value.Sql);

                            //Generate Columns
                            columns = new List<TicketColumn>();
                            columns.Add(new TicketColumn("GroupTitle", item.Value.Title, 0, TicketColumnsAlign.Left));
                            columns.Add(new TicketColumn("Quantity", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_quantity_acronym"), 8, TicketColumnsAlign.Right, typeof(decimal), "{0:0.00}"));
                            //columns.Add(new TicketColumn("UnitMeasure", string.Empty, 3));
                            columns.Add(new TicketColumn("Total", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_totalfinal_acronym"), 10, TicketColumnsAlign.Right, typeof(decimal), "{0:0.00}"));

                            //Init DataTable
                            dataTable = new DataTable();
                            dataTable.Columns.Add(new DataColumn("GroupDesignation", typeof(string)));
                            dataTable.Columns.Add(new DataColumn("Quantity", typeof(decimal)));
                            //dataTable.Columns.Add(new DataColumn("UnitMeasure", typeof(string)));
                            dataTable.Columns.Add(new DataColumn("Total", typeof(decimal)));

                            //If Has data
                            if (xPSelectData.Data.Length > 0)
                            {
                                foreach (SelectStatementResultRow row in xPSelectData.Data)
                                {
                                    designation = Convert.ToString(row.Values[xPSelectData.GetFieldIndex("Designation")]);
                                    quantity = Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndex("Quantity")]);
                                    unitMeasure = Convert.ToString(row.Values[xPSelectData.GetFieldIndex("UnitMeasure")]);
                                    total = Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndex("Total")]);
                                    // Override Encrypted values
                                    if (GlobalFramework.PluginSoftwareVendor != null && item.Key.Equals(DataTableGroupPropertiesType.DocumentsUser) || item.Key.Equals(DataTableGroupPropertiesType.PaymentsUser))
                                    {
                                        designation = GlobalFramework.PluginSoftwareVendor.Decrypt(designation);
                                    }
                                    //Sum Summary Totals
                                    summaryTotalQuantity += quantity;
                                    summaryTotal += total;
                                    //_log.Debug(string.Format("Designation: [{0}], quantity: [{1}], unitMeasure: [{2}], total: [{3}]", designation, quantity, unitMeasure, total));
                                    //Create Row
                                    dataRow = dataTable.NewRow();
                                    dataRow[0] = designation;
                                    dataRow[1] = quantity;
                                    //dataRow[2] = unitMeasure;
                                    dataRow[2] = total;
                                    dataTable.Rows.Add(dataRow);
                                }
                            }
                            else
                            {
                                //Create Row
                                dataRow = dataTable.NewRow();
                                dataRow[0] = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_cashdrawer_without_movements");
                                dataRow[1] = 0.0m;
                                //dataRow[2] = string.Empty;//UnitMeasure
                                dataRow[2] = 0.0m;
                                dataTable.Rows.Add(dataRow);
                            }

                            //Add Final Summary Row
                            dataRow = dataTable.NewRow();
                            dataRow[0] = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_total");
                            dataRow[1] = summaryTotalQuantity;
                            //dataRow[2] = string.Empty;
                            dataRow[2] = summaryTotal;
                            dataTable.Rows.Add(dataRow);

                            //Prepare TicketTable
                            ticketTable = new TicketTable(dataTable, columns, _thermalPrinterGeneric.MaxCharsPerLineNormal);

                            //Custom Print Loop, to Print all Table Rows, and Detect Rows to Print in DoubleHeight (Title and Total)
                            tableCustomPrint = ticketTable.GetTable();
                            WriteLineTextMode rowTextMode;

                            //Dynamic Print All except Last One (Totals), Double Height in Titles
                            for (int i = 0; i < tableCustomPrint.Count - 1; i++)
                            {
                                //Prepare TextMode Based on Row
                                rowTextMode = (i == 0) ? WriteLineTextMode.DoubleHeight : WriteLineTextMode.Normal;
                                //Print Row
                                _thermalPrinterGeneric.WriteLine(tableCustomPrint[i], rowTextMode);
                            }

                            //Line Feed
                            _thermalPrinterGeneric.LineFeed();
                        }
                    }

                    //When finish all groups, print Last Row, the Summary Totals Row, Ommited in Custom Print Loop
                    _thermalPrinterGeneric.WriteLine(tableCustomPrint[tableCustomPrint.Count - 1], WriteLineTextMode.DoubleHeight);

                    result = true;
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                    throw new Exception(ex.Message);
                }
            }

            return result;
        }

        private static Dictionary<DataTableGroupPropertiesType, DataTableGroupProperties> GenDataTableWorkSessionMovementResume(WorkSessionPeriodType pPeriodType, SplitCurrentAccountMode pSplitCurrentAccountMode, string pSqlWhere)
        {
            //Parameters
            string sqlWhere = pSqlWhere;
            bool enabledGroupTerminal = (pPeriodType == WorkSessionPeriodType.Day) ? true : false; ;
            bool enabledGroupPaymentMethod = (pSplitCurrentAccountMode != SplitCurrentAccountMode.CurrentAcount) ? true : false; ; ;
            bool enabledGroupSubFamily = true;

            //Init DataTableGroupProperties Object
            Dictionary<DataTableGroupPropertiesType, DataTableGroupProperties> dictGroupProperties = new Dictionary<DataTableGroupPropertiesType, DataTableGroupProperties>();

            //WorkSessionMovementResumeQueryMode.FinanceDocuments Groups : Show FinanceDocuments

            //Family
            dictGroupProperties.Add(DataTableGroupPropertiesType.DocumentsFamily, new DataTableGroupProperties(
              resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_family"),
              GenWorkSessionMovementResumeQuery(
                "FamilyDesignation AS Designation, SUM(Quantity) AS Quantity, SUM(TotalFinal) AS Total, UnitMeasure",
                "UnitMeasure, FamilyDesignation",//Required UnitMeasure and used FieldName for SqlServer Group
                "MIN(FamilyCode)",
                sqlWhere
              )
            ));

            //SubFamily
            dictGroupProperties.Add(DataTableGroupPropertiesType.DocumentsSubFamily, new DataTableGroupProperties(
              resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_subfamily"),
              GenWorkSessionMovementResumeQuery(
                "SubFamilyDesignation AS Designation, SUM(Quantity) AS Quantity, SUM(TotalFinal) AS Total, UnitMeasure",
                "UnitMeasure, SubFamilyDesignation",//Required UnitMeasure and used FieldName for SqlServer Group
                "MIN(SubFamilyCode)",
                sqlWhere
              )
              , enabledGroupSubFamily
            ));

            //Article
            dictGroupProperties.Add(DataTableGroupPropertiesType.DocumentsArticle, new DataTableGroupProperties(
              resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_article"),
              GenWorkSessionMovementResumeQuery(
                "Designation AS Designation, SUM(Quantity) AS Quantity, SUM(TotalFinal) AS Total, UnitMeasure",
                "UnitMeasure, Designation",//Required UnitMeasure and used FieldName for SqlServer Group
                "MIN(Code)",
                sqlWhere
              )
            ));

            //Tax
            dictGroupProperties.Add(DataTableGroupPropertiesType.DocumentsTax, new DataTableGroupProperties(
              resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_tax"),
              GenWorkSessionMovementResumeQuery(
                "VatDesignation AS Designation, SUM(Quantity) AS Quantity, SUM(TotalFinal) AS Total, UnitMeasure",
                "UnitMeasure, VatDesignation",//Required UnitMeasure and used FieldName for SqlServer Group
                "MIN(VatCode)",
                sqlWhere
              )
            ));

            //PaymentMethod
            dictGroupProperties.Add(DataTableGroupPropertiesType.DocumentsPaymentMethod, new DataTableGroupProperties(
              resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_type_of_payment"),
              GenWorkSessionMovementResumeQuery(
                "PaymentMethodDesignation AS Designation, SUM(Quantity) AS Quantity, SUM(TotalFinal) AS Total, UnitMeasure",
                "UnitMeasure, PaymentMethodDesignation",//Required UnitMeasure and used FieldName for SqlServer Group
                "MIN(PaymentMethodCode)",
                sqlWhere
              )
              , enabledGroupPaymentMethod
            ));

            //DocumentType
            dictGroupProperties.Add(DataTableGroupPropertiesType.DocumentsDocumentType, new DataTableGroupProperties(
              resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_documentfinance_type"),
              GenWorkSessionMovementResumeQuery(
                "DocumentTypeDesignation AS Designation, SUM(Quantity) AS Quantity, SUM(TotalFinal) AS Total, UnitMeasure",
                "UnitMeasure, DocumentTypeDesignation",//Required UnitMeasure and used FieldName for SqlServer Group
                "MIN(DocumentTypeCode)",
                sqlWhere
              )
            ));

            //Hour
            string hourField = string.Empty;
            switch (GlobalFramework.DatabaseType)
            {
                case DatabaseType.SQLite:
                case DatabaseType.MonoLite:
                    hourField = "STRFTIME('%H', MovementDate)";
                    break;
                case DatabaseType.MSSqlServer:
                    hourField = "DATEPART(hh, MovementDate)";
                    break;
                case DatabaseType.MySql:
                    hourField = "HOUR(MovementDate)";
                    break;
            }
            dictGroupProperties.Add(DataTableGroupPropertiesType.DocumentsHour, new DataTableGroupProperties(
              resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_hour"),
              GenWorkSessionMovementResumeQuery(
                string.Format(@"{0} AS Designation, SUM(Quantity) AS Quantity, SUM(TotalFinal) AS Total, UnitMeasure", hourField),
                string.Format("UnitMeasure, {0}", hourField),//Required UnitMeasure and used FieldName for SqlServer Group
                string.Format("MIN({0})", hourField),
                sqlWhere
              )
            ));

            //Terminal
            dictGroupProperties.Add(DataTableGroupPropertiesType.DocumentsTerminal, new DataTableGroupProperties(
              resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_terminal"),
              GenWorkSessionMovementResumeQuery(
                "TerminalDesignation AS Designation, SUM(Quantity) AS Quantity, SUM(TotalFinal) AS Total, UnitMeasure",
                "UnitMeasure, TerminalDesignation",//Required UnitMeasure and used FieldName for SqlServer Group
                "MIN(TerminalCode)",
                sqlWhere
              )
              , enabledGroupTerminal
            ));

            //User
            dictGroupProperties.Add(DataTableGroupPropertiesType.DocumentsUser, new DataTableGroupProperties(
              resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_user"),
              GenWorkSessionMovementResumeQuery(
                @"UserDetailName AS Designation, SUM(Quantity) AS Quantity, SUM(TotalFinal) AS Total, UnitMeasure",
                "UnitMeasure, UserDetailName",//Required UnitMeasure and used FieldName for SqlServer Group
                "MIN(UserDetailCode)",
                sqlWhere
              )
            ));

            //WorkSessionMovementResumeQueryMode.Payments Groups : Show Payments
            //Diferences is "SUM(MovementAmount) AS Total"

            //PaymentsPaymentMethod
            dictGroupProperties.Add(DataTableGroupPropertiesType.PaymentsPaymentMethod, new DataTableGroupProperties(
                resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_type_of_payment"),
                GenWorkSessionMovementResumeQuery(
                "PaymentMethodDesignation AS Designation, 0 AS Quantity, SUM(MovementAmount) AS Total, UnitMeasure",
                "UnitMeasure, PaymentMethodDesignation",//Required UnitMeasure and used FieldName for SqlServer Group
                "MIN(PaymentMethodCode)",
                sqlWhere,
                WorkSessionMovementResumeQueryMode.Payments
                )
                , enabledGroupPaymentMethod
            ));

            //PaymentsHour
            dictGroupProperties.Add(DataTableGroupPropertiesType.PaymentsHour, new DataTableGroupProperties(
                resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_hour"),
                GenWorkSessionMovementResumeQuery(
                string.Format(@"{0} AS Designation, 0 AS Quantity, SUM(MovementAmount) AS Total, UnitMeasure", hourField),
                string.Format("UnitMeasure, {0}", hourField),//Required UnitMeasure and used FieldName for SqlServer Group
                string.Format("MIN({0})", hourField),
                sqlWhere,
                WorkSessionMovementResumeQueryMode.Payments
                )
            ));

            //PaymentsTerminal
            dictGroupProperties.Add(DataTableGroupPropertiesType.PaymentsTerminal, new DataTableGroupProperties(
              resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_terminal"),
              GenWorkSessionMovementResumeQuery(
                "TerminalDesignation AS Designation, 0 AS Quantity, SUM(MovementAmount) AS Total, UnitMeasure",
                "UnitMeasure, TerminalDesignation",//Required UnitMeasure and used FieldName for SqlServer Group
                "MIN(TerminalCode)",
                sqlWhere,
                WorkSessionMovementResumeQueryMode.Payments
              )
              , enabledGroupTerminal
            ));

            //PaymentsUser
            dictGroupProperties.Add(DataTableGroupPropertiesType.PaymentsUser, new DataTableGroupProperties(
                resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_user"),
                GenWorkSessionMovementResumeQuery(
                @"UserDetailName AS Designation, 0 AS Quantity, SUM(MovementAmount) AS Total, UnitMeasure",
                "UnitMeasure, UserDetailName",//Required UnitMeasure and used FieldName for SqlServer Group
                "MIN(UserDetailCode)",
                sqlWhere,
                WorkSessionMovementResumeQueryMode.Payments
                )
            ));

            return dictGroupProperties;
        }

        /// <summary>
        /// Used to generate Share Queries for WorkSessionMovement Resume
        /// </summary>
        private static string GenWorkSessionMovementResumeQuery(string pFields, string pGroupBy, string pOrderBy, string pFilter)
        {
            return GenWorkSessionMovementResumeQuery(pFields, pGroupBy, pOrderBy, pFilter, WorkSessionMovementResumeQueryMode.FinanceDocuments);
        }

        private static string GenWorkSessionMovementResumeQuery(string pFields, string pGroupBy, string pOrderBy, string pFilter, WorkSessionMovementResumeQueryMode pQueryModeWhere)
        {
            string filter = string.Empty;
            string queryModeWhere = string.Empty;

            if (pFilter != String.Empty) filter = pFilter;

            switch (pQueryModeWhere)
            {
                case WorkSessionMovementResumeQueryMode.FinanceDocuments:
                    queryModeWhere = "Document IS NOT NULL AND DocumentStatusStatus <> 'A'";
                    break;
                case WorkSessionMovementResumeQueryMode.Payments:
                    queryModeWhere = "Payment IS NOT NULL AND PaymentStatus <> 'A'";
                    break;
            }

            string sqlBase = string.Format(@"
                SELECT 
                  {0}
                FROM 
                  view_worksessionmovementresume
                WHERE 
                  (MovementTypeToken = 'FINANCE_DOCUMENT' AND {4})
                  {3}
                GROUP BY 
                  {1}
                ORDER BY 
                  {2}
                ;
                ",
                pFields,
                pGroupBy,
                pOrderBy,
                filter,
                queryModeWhere
            );

            return sqlBase;
        }
    }
}
