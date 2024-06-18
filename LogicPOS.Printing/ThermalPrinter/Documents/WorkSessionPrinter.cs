using DevExpress.Xpo.DB;
using LogicPOS.Data.XPO;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.DTOs.Printing;
using LogicPOS.Printing.Enums;
using LogicPOS.Printing.Templates;
using LogicPOS.Printing.Tickets;
using LogicPOS.Settings;
using LogicPOS.Settings.Enums;
using LogicPOS.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace LogicPOS.Printing.Documents
{
    public partial class WorkSessionPrinter : BaseInternalTemplate
    {
        private readonly PrintWorkSessionDto _workSessionPeriod;
        private readonly SplitCurrentAccountMode _splitCurrentAccountMode;
        private readonly string _workSessionMovementPrintingFileTemplate;
        private readonly Hashtable _sessionPeriodSummaryDetails;

        public WorkSessionPrinter(
            PrinterDto printer,
            PrintWorkSessionDto workSession,
            SplitCurrentAccountMode pSplitCurrentAccountMode,
            string workSessionMovementPrintingFileTemplate,
            Hashtable sessionPeriodSummaryDetails)
            : base(printer)
        {
            _workSessionPeriod = workSession;
            _splitCurrentAccountMode = pSplitCurrentAccountMode;

            DefineTicketTitle();

            DefineTicketSubtitle();
            _workSessionMovementPrintingFileTemplate = workSessionMovementPrintingFileTemplate;
            _sessionPeriodSummaryDetails = sessionPeriodSummaryDetails;
        }

        private void DefineTicketSubtitle()
        {
            string ticketSubTitleExtra = string.Empty;
            switch (_splitCurrentAccountMode)
            {
                case SplitCurrentAccountMode.All:
                    break;
                case SplitCurrentAccountMode.NonCurrentAcount:
                    ticketSubTitleExtra = "";
                    break;
                case SplitCurrentAccountMode.CurrentAcount:
                    ticketSubTitleExtra = GeneralUtils.GetResourceByName("global_current_account");
                    break;
            }

            if (_ticketSubTitle != string.Empty && ticketSubTitleExtra != string.Empty)
            {
                _ticketSubTitle = string.Format("{0} : ({1})", _ticketSubTitle, ticketSubTitleExtra);
            }
            else if (_ticketSubTitle == string.Empty && ticketSubTitleExtra != string.Empty)
            {
                _ticketSubTitle = string.Format("({0})", ticketSubTitleExtra);
            }
        }

        private void DefineTicketTitle()
        {
            if (_workSessionPeriod.PeriodTypeIsDay)
            {
                if (_workSessionPeriod.SessionStatusIsOpen)
                {
                    _ticketTitle = GeneralUtils.GetResourceByName("ticket_title_worksession_day_resume");
                }
                else
                {
                    _ticketTitle = GeneralUtils.GetResourceByName("ticket_title_worksession_day_close");
                }

                return;
            }

            if (_workSessionPeriod.SessionStatusIsOpen)
            {
                _ticketTitle = GeneralUtils.GetResourceByName("ticket_title_worksession_terminal_resume");
            }
            else
            {
                _ticketTitle = GeneralUtils.GetResourceByName("ticket_title_worksession_terminal_close");
            }

            _ticketSubTitle = _workSessionPeriod.PeriodTypeIsTerminal ? _workSessionPeriod.TerminalDesignation : string.Empty;

        }

        public override void PrintContent()
        {
            PrintTitles(_ticketTitle);
            _printer.SetAlignCenter();
            PrintDocumentDetails();
            _printer.SetAlignLeft();
            _printer.LineFeed();
        }

        private void PrintDocumentDetails()
        {
            PrintWorkSessionMovement(_workSessionPeriod,
                                     _splitCurrentAccountMode);
        }

        public bool PrintWorkSessionMovement(
            PrintWorkSessionDto workSession,
            SplitCurrentAccountMode pSplitCurrentAccountMode
            )
        {
            string splitCurrentAccountFilter = string.Empty;
            string fileTicket = _workSessionMovementPrintingFileTemplate;

            switch (pSplitCurrentAccountMode)
            {
                case SplitCurrentAccountMode.All:
                    break;
                case SplitCurrentAccountMode.NonCurrentAcount:
                    splitCurrentAccountFilter = $"AND DocumentType <> '{DocumentSettings.CurrentAccountInputId}'";
                    break;
                case SplitCurrentAccountMode.CurrentAcount:
                    splitCurrentAccountFilter = $"AND DocumentType = '{DocumentSettings.CurrentAccountInputId}'";
                    break;
            }

            string sqlWhere = string.Empty;

            if (workSession.PeriodTypeIsDay)
            {
                sqlWhere = $"PeriodParent = '{workSession.Id}'{splitCurrentAccountFilter}";
            }
            else
            {
                sqlWhere = $"Period = '{workSession.Id}'{splitCurrentAccountFilter}";
            }

            if (sqlWhere != string.Empty)
            {
                sqlWhere = $" AND {sqlWhere}";
            }

            string dateCloseDisplay = (workSession.SessionStatusIsOpen) ?
                GeneralUtils.GetResourceByName("global_in_progress") :
                workSession.StartDate.ToString(CultureSettings.DateTimeFormat);


            //Print Header Summary
            DataRow dataRow = null;
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("Label", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Value", typeof(string)));
            //Open DateTime
            dataRow = dataTable.NewRow();
            dataRow[0] = string.Format("{0}:", GeneralUtils.GetResourceByName("global_worksession_open_datetime"));
            dataRow[1] = workSession.StartDate.ToString(CultureSettings.DateTimeFormat);
            dataTable.Rows.Add(dataRow);
            //Close DataTime
            dataRow = dataTable.NewRow();
            dataRow[0] = string.Format("{0}:", GeneralUtils.GetResourceByName("global_worksession_close_datetime"));
            dataRow[1] = dateCloseDisplay;
            dataTable.Rows.Add(dataRow);
            //Open Total CashDrawer
            dataRow = dataTable.NewRow();
            dataRow[0] = string.Format("{0}:", GeneralUtils.GetResourceByName("global_worksession_open_total_cashdrawer"));
            dataRow[1] = LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(
                (decimal)_sessionPeriodSummaryDetails["totalMoneyInCashDrawerOnOpen"],
                XPOSettings.ConfigurationSystemCurrency.Acronym);
            dataTable.Rows.Add(dataRow);
            //Close Total CashDrawer
            dataRow = dataTable.NewRow();
            dataRow[0] = string.Format("{0}:", GeneralUtils.GetResourceByName("global_worksession_close_total_cashdrawer"));
            dataRow[1] = LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(
                (decimal)_sessionPeriodSummaryDetails["totalMoneyInCashDrawer"],
                XPOSettings.ConfigurationSystemCurrency.Acronym);
            dataTable.Rows.Add(dataRow);
            //Total Money In
            dataRow = dataTable.NewRow();
            dataRow[0] = string.Format("{0}:", GeneralUtils.GetResourceByName("global_worksession_total_money_in"));
            dataRow[1] = LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(
                (decimal)_sessionPeriodSummaryDetails["totalMoneyIn"],
                XPOSettings.ConfigurationSystemCurrency.Acronym);
            dataTable.Rows.Add(dataRow);
            //Total Money Out
            dataRow = dataTable.NewRow();
            dataRow[0] = string.Format("{0}:", GeneralUtils.GetResourceByName("global_worksession_total_money_out"));
            dataRow[1] = LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(
                (decimal)_sessionPeriodSummaryDetails["totalMoneyOut"],
                XPOSettings.ConfigurationSystemCurrency.Acronym);
            dataTable.Rows.Add(dataRow);
            //Configure Ticket Column Properties
            List<TicketColumn> columns = new List<TicketColumn>
                    {
                        new TicketColumn("Label", "", Convert.ToInt16(_maxCharsPerLineNormal / 2) - 2, TicketColumnsAlignment.Right),
                        new TicketColumn("Value", "", Convert.ToInt16(_maxCharsPerLineNormal / 2) - 2, TicketColumnsAlignment.Left)
                    };
            TicketTable ticketTable = new TicketTable(dataTable, columns, _printer.MaxCharsPerLineNormalBold);
            //Print Ticket Table
            ticketTable.Print(_printer);
            //Line Feed
            _printer.LineFeed();

            //Get Final Rendered DataTable Groups
            Dictionary<DataTableGroupPropertiesType, DataTableGroupProperties> dictGroupProperties = GenDataTableWorkSessionMovementResume(workSession.PeriodType, pSplitCurrentAccountMode, sqlWhere);

            //Prepare Local vars for Group Loop
            SQLSelectResultData xPSelectData = null;
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
            int groupPositionTitlePayments = (workSession.PeriodTypeIsDay) ? 9 : 8;
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
                        _printer.WriteLine(GeneralUtils.GetResourceByName("global_worksession_resume_finance_documents"), WriteLineTextMode.Big);
                        _printer.LineFeed();
                    }
                    else if (groupPosition == groupPositionTitlePayments)
                    {
                        //When finish FinanceDocuemnts groups, print Last Row, the Summary Totals Row
                        _printer.WriteLine(tableCustomPrint[tableCustomPrint.Count - 1], WriteLineTextMode.DoubleHeight);
                        _printer.LineFeed();

                        _printer.WriteLine(GeneralUtils.GetResourceByName("global_worksession_resume_paymens_documents"), WriteLineTextMode.Big);
                        _printer.LineFeed();
                    }

                    //Reset Totals
                    summaryTotalQuantity = 0.0m;
                    summaryTotal = 0.0m;

                    //Get Group Data from group Query
                    xPSelectData = XPOUtility.GetSelectedDataFromQuery(item.Value.Sql);

                    //Generate Columns
                    columns = new List<TicketColumn>
                            {
                                new TicketColumn("GroupTitle", item.Value.Title, 0, TicketColumnsAlignment.Left),
                                new TicketColumn("Quantity", GeneralUtils.GetResourceByName("global_quantity_acronym"), 8, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"),
                                //columns.Add(new TicketColumn("UnitMeasure", string.Empty, 3));
                                new TicketColumn("Total", GeneralUtils.GetResourceByName("global_totalfinal_acronym"), 10, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}")
                            };

                    //Init DataTable
                    dataTable = new DataTable();
                    dataTable.Columns.Add(new DataColumn("GroupDesignation", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("Quantity", typeof(decimal)));
                    //dataTable.Columns.Add(new DataColumn("UnitMeasure", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("Total", typeof(decimal)));

                    //If Has data
                    if (xPSelectData.DataRows.Length > 0)
                    {
                        foreach (SelectStatementResultRow row in xPSelectData.DataRows)
                        {
                            designation = Convert.ToString(row.Values[xPSelectData.GetFieldIndexFromName("Designation")]);
                            quantity = Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndexFromName("Quantity")]);
                            unitMeasure = Convert.ToString(row.Values[xPSelectData.GetFieldIndexFromName("UnitMeasure")]);
                            total = Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndexFromName("Total")]);
                            // Override Encrypted values
                            if (PluginSettings.HasSoftwareVendorPlugin && item.Key.Equals(DataTableGroupPropertiesType.DocumentsUser) || item.Key.Equals(DataTableGroupPropertiesType.PaymentsUser))
                            {
                                designation = PluginSettings.SoftwareVendor.Decrypt(designation);
                            }
                            //Sum Summary Totals
                            summaryTotalQuantity += quantity;
                            summaryTotal += total;
                            //_logger.Debug(string.Format("Designation: [{0}], quantity: [{1}], unitMeasure: [{2}], total: [{3}]", designation, quantity, unitMeasure, total));
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
                        dataRow[0] = GeneralUtils.GetResourceByName("global_cashdrawer_without_movements");
                        dataRow[1] = 0.0m;
                        //dataRow[2] = string.Empty;//UnitMeasure
                        dataRow[2] = 0.0m;
                        dataTable.Rows.Add(dataRow);
                    }

                    //Add Final Summary Row
                    dataRow = dataTable.NewRow();
                    dataRow[0] = GeneralUtils.GetResourceByName("global_total");
                    dataRow[1] = summaryTotalQuantity;
                    //dataRow[2] = string.Empty;
                    dataRow[2] = summaryTotal;
                    dataTable.Rows.Add(dataRow);

                    //Prepare TicketTable
                    ticketTable = new TicketTable(dataTable, columns, _printer.MaxCharsPerLineNormal);

                    //Custom Print Loop, to Print all Table Rows, and Detect Rows to Print in DoubleHeight (Title and Total)
                    tableCustomPrint = ticketTable.GetTable();
                    WriteLineTextMode rowTextMode;

                    //Dynamic Print All except Last One (Totals), Double Height in Titles
                    for (int i = 0; i < tableCustomPrint.Count - 1; i++)
                    {
                        //Prepare TextMode Based on Row
                        rowTextMode = (i == 0) ? WriteLineTextMode.DoubleHeight : WriteLineTextMode.Normal;
                        //Print Row
                        _printer.WriteLine(tableCustomPrint[i], rowTextMode);
                    }

                    //Line Feed
                    _printer.LineFeed();
                }
            }

            //When finish all groups, print Last Row, the Summary Totals Row, Ommited in Custom Print Loop
            _printer.WriteLine(tableCustomPrint[tableCustomPrint.Count - 1], WriteLineTextMode.DoubleHeight);

            return true;
        }

        private static Dictionary<DataTableGroupPropertiesType, DataTableGroupProperties> GenDataTableWorkSessionMovementResume(
            string periodType,
            SplitCurrentAccountMode pSplitCurrentAccountMode,
            string pSqlWhere)
        {
            //Parameters
            string sqlWhere = pSqlWhere;
            bool enabledGroupTerminal = (periodType == "Day"); ;
            bool enabledGroupPaymentMethod = (pSplitCurrentAccountMode != SplitCurrentAccountMode.CurrentAcount); ; ;
            bool enabledGroupSubFamily = true;

            //Init DataTableGroupProperties Object
            Dictionary<DataTableGroupPropertiesType, DataTableGroupProperties> dictGroupProperties = new Dictionary<DataTableGroupPropertiesType, DataTableGroupProperties>
            {
                //WorkSessionMovementResumeQueryMode.FinanceDocuments Groups : Show FinanceDocuments

                //Family
                {
                    DataTableGroupPropertiesType.DocumentsFamily,
                    new DataTableGroupProperties(
              GeneralUtils.GetResourceByName("global_family"),
              GenWorkSessionMovementResumeQuery(
                "FamilyDesignation AS Designation, SUM(Quantity) AS Quantity, SUM(TotalFinal) AS Total, UnitMeasure",
                "UnitMeasure, FamilyDesignation",//Required UnitMeasure and used FieldName for SqlServer Group
                "MIN(FamilyCode)",
                sqlWhere
              )
            )
                },

                //SubFamily
                {
                    DataTableGroupPropertiesType.DocumentsSubFamily,
                    new DataTableGroupProperties(
              GeneralUtils.GetResourceByName("global_subfamily"),
              GenWorkSessionMovementResumeQuery(
                "SubFamilyDesignation AS Designation, SUM(Quantity) AS Quantity, SUM(TotalFinal) AS Total, UnitMeasure",
                "UnitMeasure, SubFamilyDesignation",//Required UnitMeasure and used FieldName for SqlServer Group
                "MIN(SubFamilyCode)",
                sqlWhere
              )
              , enabledGroupSubFamily
            )
                },

                //Article
                {
                    DataTableGroupPropertiesType.DocumentsArticle,
                    new DataTableGroupProperties(
              GeneralUtils.GetResourceByName("global_article"),
              GenWorkSessionMovementResumeQuery(
                "Designation AS Designation, SUM(Quantity) AS Quantity, SUM(TotalFinal) AS Total, UnitMeasure",
                "UnitMeasure, Designation",//Required UnitMeasure and used FieldName for SqlServer Group
                "MIN(Code)",
                sqlWhere
              )
            )
                },

                //Tax
                {
                    DataTableGroupPropertiesType.DocumentsTax,
                    new DataTableGroupProperties(
              GeneralUtils.GetResourceByName("global_tax"),
              GenWorkSessionMovementResumeQuery(
                "VatDesignation AS Designation, SUM(Quantity) AS Quantity, SUM(TotalFinal) AS Total, UnitMeasure",
                "UnitMeasure, VatDesignation",//Required UnitMeasure and used FieldName for SqlServer Group
                "MIN(VatCode)",
                sqlWhere
              )
            )
                },

                //PaymentMethod
                {
                    DataTableGroupPropertiesType.DocumentsPaymentMethod,
                    new DataTableGroupProperties(
              GeneralUtils.GetResourceByName("global_type_of_payment"),
              GenWorkSessionMovementResumeQuery(
                "PaymentMethodDesignation AS Designation, SUM(Quantity) AS Quantity, SUM(TotalFinal) AS Total, UnitMeasure",
                "UnitMeasure, PaymentMethodDesignation",//Required UnitMeasure and used FieldName for SqlServer Group
                "MIN(PaymentMethodCode)",
                sqlWhere
              )
              , enabledGroupPaymentMethod
            )
                },

                //DocumentType
                {
                    DataTableGroupPropertiesType.DocumentsDocumentType,
                    new DataTableGroupProperties(
              GeneralUtils.GetResourceByName("global_documentfinance_type"),
              GenWorkSessionMovementResumeQuery(
                "DocumentTypeDesignation AS Designation, SUM(Quantity) AS Quantity, SUM(TotalFinal) AS Total, UnitMeasure",
                "UnitMeasure, DocumentTypeDesignation",//Required UnitMeasure and used FieldName for SqlServer Group
                "MIN(DocumentTypeCode)",
                sqlWhere
              )
            )
                }
            };

            //Hour
            string hourField = string.Empty;
            switch (DatabaseSettings.DatabaseType)
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
              GeneralUtils.GetResourceByName("global_hour"),
              GenWorkSessionMovementResumeQuery(
                string.Format(@"{0} AS Designation, SUM(Quantity) AS Quantity, SUM(TotalFinal) AS Total, UnitMeasure", hourField),
                string.Format("UnitMeasure, {0}", hourField),//Required UnitMeasure and used FieldName for SqlServer Group
                string.Format("MIN({0})", hourField),
                sqlWhere
              )
            ));

            //Terminal
            dictGroupProperties.Add(DataTableGroupPropertiesType.DocumentsTerminal, new DataTableGroupProperties(
              GeneralUtils.GetResourceByName("global_terminal"),
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
              GeneralUtils.GetResourceByName("global_user"),
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
                GeneralUtils.GetResourceByName("global_type_of_payment"),
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
                GeneralUtils.GetResourceByName("global_hour"),
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
              GeneralUtils.GetResourceByName("global_terminal"),
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
                GeneralUtils.GetResourceByName("global_user"),
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

        private static string GenWorkSessionMovementResumeQuery(string pFields, string pGroupBy, string pOrderBy, string pFilter)
        {
            return GenWorkSessionMovementResumeQuery(pFields, pGroupBy, pOrderBy, pFilter, WorkSessionMovementResumeQueryMode.FinanceDocuments);
        }

        private static string GenWorkSessionMovementResumeQuery(string pFields, string pGroupBy, string pOrderBy, string pFilter, WorkSessionMovementResumeQueryMode pQueryModeWhere)
        {
            string filter = string.Empty;
            string queryModeWhere = string.Empty;

            if (pFilter != string.Empty) filter = pFilter;

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
