using LogicPOS.Data.XPO.Settings;
using LogicPOS.DTOs.Printing;
using LogicPOS.Globalization;
using LogicPOS.Printing.Enums;
using LogicPOS.Printing.Templates;
using LogicPOS.Printing.Tickets;
using LogicPOS.Reporting.BOs;
using LogicPOS.Reporting.BOs.Documents;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Data;

namespace LogicPOS.Printing.Documents
{
    public class ThermalPrinterFinanceDocumentPayment : ThermalPrinterBaseFinanceTemplate
    {
        //Parameters Properties
        private readonly PrintingFinancePaymentDto _documentFinancePayment = null;
        //Business Objects
        private readonly List<FRBODocumentFinancePaymentView> _documentFinancePaymentList;
        private readonly List<FRBODocumentFinancePaymentDocumentView> _documentFinancePaymentDocumentList;

        public ThermalPrinterFinanceDocumentPayment(
            PrintingPrinterDto printer,
            PrintingFinancePaymentDto financePayment,
            List<int> copyNames,
            bool secondCopy)
            : base(
                  printer,
                  financePayment.DocumentType,
                  copyNames,
                  secondCopy)
        {
            try
            {
                //Parameters
                _documentFinancePayment = financePayment;

                //Init Fast Reports Business Objects (From FRBOHelper)
                ResultFRBODocumentFinancePayment fRBOHelperResponseProcessReportFinancePayment = FRBOHelper.GetFRBOFinancePayment(financePayment.Id);
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
                PrintDocumentMaster(_documentFinancePaymentList[0].DocumentTypeResourceString, _documentFinancePaymentList[0].PaymentRefNo, _documentFinancePaymentList[0].DocumentDate);

                //Call base PrintCustomer();
                PrintCustomer(
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
                PrintDocumentPaymentDetails(_documentFinancePaymentList[0].PaymentMethodDesignation, _documentFinancePaymentList[0].CurrencyAcronym);

                //Call base PrintNotes();
                if (!string.IsNullOrEmpty(_documentFinancePaymentList[0].Notes)) PrintNotes(_documentFinancePaymentList[0].Notes.ToString());

                //Call base PrintDocumentTypeFooterString();
                PrintDocumentTypeFooterString(_documentFinancePaymentList[0].DocumentTypeResourceStringReport);

                //Call Base CertificationText Without Hash
                PrintCertificationText();
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
                List<TicketColumn> columns = new List<TicketColumn>
                {
                    new TicketColumn("DocumentDate", CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_date"), 11, TicketColumnsAlignment.Left),
                    new TicketColumn("DocumentNumber", CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_document_number_acronym"), 0, TicketColumnsAlignment.Left),
                    new TicketColumn("DocumentTotal", CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_document_total"), 10, TicketColumnsAlignment.Right, typeof(decimal), "{0:00.00}"),
                    new TicketColumn("TotalPayed", CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_total_payed_acronym"), 10, TicketColumnsAlignment.Right, typeof(decimal), "{0:00.00}"),
                    new TicketColumn("Payed", "L", 1, TicketColumnsAlignment.Right)
                };
                //Prepare Table with Padding
                DataTable dataTable = TicketTable.InitDataTableFromTicketColumns(columns);
                TicketTable ticketTable = new TicketTable(dataTable, columns, _maxCharsPerLineNormal - _ticketTablePaddingLeftLength);
                string paddingLeftFormat = "  {0,-" + ticketTable.TableWidth + "}";//"  {0,-TableWidth}"
                //Print Table Headers
                ticketTable.Print(_genericThermalPrinter, paddingLeftFormat);

                foreach (FRBODocumentFinancePaymentDocumentView item in _documentFinancePaymentDocumentList)
                {
                    //Recreate/Reset Table for Item Details Loop
                    ticketTable = new TicketTable(dataTable, columns, _maxCharsPerLineNormal - _ticketTablePaddingLeftLength);
                    PrintDocumentDetail(ticketTable, item, paddingLeftFormat);
                }

                //Line Feed
                _genericThermalPrinter.LineFeed();
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
                _genericThermalPrinter.WriteLine(documentNumber, WriteLineTextMode.Bold);

                //Document Details
                DataRow dataRow;
                dataRow = pTicketTable.NewRow();
                dataRow[0] = pFinancePaymentDocument.DocumentDate;
                dataRow[1] = pFinancePaymentDocument.DocumentNumber;
                //dataRow[2] = (item.CreditAmount > 0 && item.DocumentTotal > item.CreditAmount) 
                //    ? LogicPOS.Utility.DataConversionUtils.DecimalToString((item.DocumentTotal - item.CreditAmount) * _documentFinancePayment.ExchangeRate)
                //    : string.Empty;
                dataRow[2] = pFinancePaymentDocument.DocumentTotal * _documentFinancePayment.ExchangeRate;
                dataRow[3] = pFinancePaymentDocument.CreditAmount * _documentFinancePayment.ExchangeRate;
                dataRow[4] = (pFinancePaymentDocument.Payed) ? "*" : string.Empty;

                //Add DataRow to Table, Ready for Print
                pTicketTable.Rows.Add(dataRow);
                //Print Table Rows
                pTicketTable.Print(_genericThermalPrinter, WriteLineTextMode.Normal, true, pPaddingLeftFormat);
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
                dataRow[0] = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_total");
                dataRow[1] = LogicPOS.Utility.DataConversionUtils.DecimalToString(_documentFinancePaymentList[0].PaymentAmount * _documentFinancePaymentList[0].ExchangeRate);
                dataTable.Rows.Add(dataRow);

                //Configure Ticket Column Properties
                List<TicketColumn> columns = new List<TicketColumn>
                {
                    new TicketColumn("Label", "", Convert.ToInt16(_maxCharsPerLineNormal / 2) - 2, TicketColumnsAlignment.Left),
                    new TicketColumn("Value", "", Convert.ToInt16(_maxCharsPerLineNormal / 2) - 2, TicketColumnsAlignment.Right)
                };

                //TicketTable(DataTable pDataTable, List<TicketColumn> pColumnsProperties, int pTableWidth)
                TicketTable ticketTable = new TicketTable(dataTable, columns, _genericThermalPrinter.MaxCharsPerLineNormalBold);
                ticketTable.Print(_genericThermalPrinter, WriteLineTextMode.DoubleHeightBold);

                //Line Feed
                _genericThermalPrinter.LineFeed();
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
                if (_documentFinancePaymentList[0].CurrencyAcronym != XPOSettings.ConfigurationSystemCurrency.Acronym)
                {
                    //Get ExtendedValue
                    NumberToWordsUtility extendValue = new NumberToWordsUtility();
                    extended = extendValue.GetExtendedValue(_documentFinancePaymentList[0].PaymentAmount * _documentFinancePaymentList[0].ExchangeRate, _documentFinancePaymentList[0].CurrencyDesignation);
                }

                //ExtendedValue
                _genericThermalPrinter.WriteLine(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_total_extended_label"), WriteLineTextMode.Bold);
                _genericThermalPrinter.WriteLine(extended);

                //Line Feed
                _genericThermalPrinter.LineFeed();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}