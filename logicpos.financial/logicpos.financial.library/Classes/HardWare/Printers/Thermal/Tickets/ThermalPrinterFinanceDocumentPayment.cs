using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.library.App;
using logicpos.financial.library.Classes.Finance;
using logicpos.financial.library.Classes.Hardware.Printers.Thermal.Enums;
using logicpos.financial.library.Classes.Reports.BOs;
using logicpos.financial.library.Classes.Reports.BOs.Documents;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections.Generic;
using System.Data;

namespace logicpos.financial.library.Classes.Hardware.Printers.Thermal.Tickets
{
    public class ThermalPrinterFinanceDocumentPayment : ThermalPrinterBaseFinanceTemplate
    {
        //Parameters Properties
        private fin_documentfinancepayment _documentFinancePayment = null;
        //Business Objects
        private List<FRBODocumentFinancePaymentView> _documentFinancePaymentList;
        private List<FRBODocumentFinancePaymentDocumentView> _documentFinancePaymentDocumentList;

        public ThermalPrinterFinanceDocumentPayment(sys_configurationprinters pPrinter, fin_documentfinancepayment pDocumentFinancePayment, List<int> pCopyNames, bool pSecondCopy)
            : base(pPrinter, pDocumentFinancePayment.DocumentType, pCopyNames, pSecondCopy)
        {
            try
            {
                //Parameters
                _documentFinancePayment = pDocumentFinancePayment;

                //Init Fast Reports Business Objects (From FRBOHelper)
                ResultFRBODocumentFinancePayment fRBOHelperResponseProcessReportFinancePayment = FRBOHelper.GetFRBOFinancePayment(pDocumentFinancePayment.Oid);
                //Get FRBOs Lists 
                _documentFinancePaymentList = fRBOHelperResponseProcessReportFinancePayment.DocumentFinancePayment.List;
                _documentFinancePaymentDocumentList = fRBOHelperResponseProcessReportFinancePayment.DocumentFinancePayment.List[0].DocumentFinancePaymentDocument;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Override Parent Template
        public override void PrintContent()
        {
            try
            {
                //Call base PrintDocumentMaster();
                base.PrintDocumentMaster(_documentFinancePaymentList[0].DocumentTypeResourceString, _documentFinancePaymentList[0].PaymentRefNo, _documentFinancePaymentList[0].DocumentDate);

                //Call base PrintCustomer();
                base.PrintCustomer(
                    _documentFinancePaymentList[0].EntityName,
                    _documentFinancePaymentList[0].EntityAddress,
                    _documentFinancePaymentList[0].EntityZipCode,
                    _documentFinancePaymentList[0].EntityCity,
                    _documentFinancePaymentList[0].EntityCountry,
                    _documentFinancePaymentList[0].EntityFiscalNumber
                );

                PrintDocumentDetails();
                PrintMasterTotals();
                PrintExtendedValue();

                //Call base PrintDocumentPaymentDetails();
                base.PrintDocumentPaymentDetails(_documentFinancePaymentList[0].PaymentMethodDesignation, _documentFinancePaymentList[0].CurrencyAcronym);

                //Call base PrintNotes();
                if (!string.IsNullOrEmpty(_documentFinancePaymentList[0].Notes)) base.PrintNotes(_documentFinancePaymentList[0].Notes.ToString());

                //Call base PrintDocumentTypeFooterString();
                PrintDocumentTypeFooterString(_documentFinancePaymentList[0].DocumentTypeResourceStringReport);

                //Call Base CertificationText Without Hash
                base.PrintCertificationText();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void PrintDocumentDetails()
        {
            try
            {
                List<TicketColumn> columns = new List<TicketColumn>();
                columns.Add(new TicketColumn("DocumentDate", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_date"), 11, TicketColumnsAlign.Left));
                columns.Add(new TicketColumn("DocumentNumber", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_document_number_acronym"), 0, TicketColumnsAlign.Left));
                columns.Add(new TicketColumn("DocumentTotal", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_document_total"), 10, TicketColumnsAlign.Right, typeof(decimal), "{0:00.00}"));
                columns.Add(new TicketColumn("TotalPayed", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_total_payed_acronym"), 10, TicketColumnsAlign.Right, typeof(decimal), "{0:00.00}"));
                columns.Add(new TicketColumn("Payed", "L", 1, TicketColumnsAlign.Right, typeof(bool)));
                //Prepare Table with Padding
                DataTable dataTable = TicketTable.InitDataTableFromTicketColumns(columns);
                TicketTable ticketTable = new TicketTable(dataTable, columns, _maxCharsPerLineNormal - _ticketTablePaddingLeftLength);
                string paddingLeftFormat = "  {0,-" + ticketTable.TableWidth + "}";//"  {0,-TableWidth}"
                //Print Table Headers
                ticketTable.Print(_thermalPrinterGeneric, paddingLeftFormat);

                foreach (FRBODocumentFinancePaymentDocumentView item in _documentFinancePaymentDocumentList)
                {
                    //Recreate/Reset Table for Item Details Loop
                    ticketTable = new TicketTable(dataTable, columns, _maxCharsPerLineNormal - _ticketTablePaddingLeftLength);
                    PrintDocumentDetail(ticketTable, item, paddingLeftFormat);
                }

                //Line Feed
                _thermalPrinterGeneric.LineFeed();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Detail Row Block
        public void PrintDocumentDetail(TicketTable pTicketTable, FRBODocumentFinancePaymentDocumentView pFinancePaymentDocument, string pPaddingLeftFormat)
        {
            try
            {
                //Trim Data
                string documentNumber = (pFinancePaymentDocument.DocumentNumber.Length <= _maxCharsPerLineNormalBold)
                    ? pFinancePaymentDocument.DocumentNumber
                    : pFinancePaymentDocument.DocumentNumber.Substring(0, _maxCharsPerLineNormalBold)
                ;
                //Print Document Number : Bold
                _thermalPrinterGeneric.WriteLine(documentNumber, WriteLineTextMode.Bold);

                //Document Details
                DataRow dataRow;
                dataRow = pTicketTable.NewRow();
                dataRow[0] = pFinancePaymentDocument.DocumentDate;
                dataRow[1] = pFinancePaymentDocument.DocumentNumber;
                //dataRow[2] = (item.CreditAmount > 0 && item.DocumentTotal > item.CreditAmount) 
                //    ? FrameworkUtils.DecimalToString((item.DocumentTotal - item.CreditAmount) * _documentFinancePayment.ExchangeRate)
                //    : string.Empty;
                dataRow[2] = pFinancePaymentDocument.DocumentTotal * _documentFinancePayment.ExchangeRate;
                dataRow[3] = pFinancePaymentDocument.CreditAmount * _documentFinancePayment.ExchangeRate;
                dataRow[4] = (pFinancePaymentDocument.Payed) ? "*" : string.Empty;

                //Add DataRow to Table, Ready for Print
                pTicketTable.Rows.Add(dataRow);
                //Print Table Rows
                pTicketTable.Print(_thermalPrinterGeneric, WriteLineTextMode.Normal, true, pPaddingLeftFormat);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Totals, with TotalDelivery and TotalChange
        private void PrintMasterTotals()
        {
            try
            {
                //Init DataTable
                DataRow dataRow = null;
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add(new DataColumn("Label", typeof(string)));
                dataTable.Columns.Add(new DataColumn("Value", typeof(string)));

                //Add Row : TotalFinal
                dataRow = dataTable.NewRow();
                dataRow[0] = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_total");
                dataRow[1] = FrameworkUtils.DecimalToString(_documentFinancePaymentList[0].PaymentAmount * _documentFinancePaymentList[0].ExchangeRate);
                dataTable.Rows.Add(dataRow);

                //Configure Ticket Column Properties
                List<TicketColumn> columns = new List<TicketColumn>();
                columns.Add(new TicketColumn("Label", "", Convert.ToInt16(_maxCharsPerLineNormal / 2) - 2, TicketColumnsAlign.Left));
                columns.Add(new TicketColumn("Value", "", Convert.ToInt16(_maxCharsPerLineNormal / 2) - 2, TicketColumnsAlign.Right));

                //TicketTable(DataTable pDataTable, List<TicketColumn> pColumnsProperties, int pTableWidth)
                TicketTable ticketTable = new TicketTable(dataTable, columns, _thermalPrinterGeneric.MaxCharsPerLineNormalBold);
                ticketTable.Print(_thermalPrinterGeneric, WriteLineTextMode.DoubleHeightBold);

                //Line Feed
                _thermalPrinterGeneric.LineFeed();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void PrintExtendedValue()
        {
            try
            {
                string extended = _documentFinancePayment.ExtendedValue;

                //Require to generated/override default Exchanged with document ExchangeRate Extended Value (Foreign Curency)
                if (_documentFinancePaymentList[0].CurrencyAcronym != SettingsApp.ConfigurationSystemCurrency.Acronym)
                {
                    //Get ExtendedValue
                    ExtendValue extendValue = new ExtendValue();
                    extended = extendValue.GetExtendedValue(_documentFinancePaymentList[0].PaymentAmount * _documentFinancePaymentList[0].ExchangeRate, _documentFinancePaymentList[0].CurrencyDesignation);
                }

                //ExtendedValue
                _thermalPrinterGeneric.WriteLine(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_total_extended_label"), WriteLineTextMode.Bold);
                _thermalPrinterGeneric.WriteLine(extended);

                //Line Feed
                _thermalPrinterGeneric.LineFeed();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}