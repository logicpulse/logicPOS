using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.DTOs.Common;
using LogicPOS.DTOs.Printing;
using LogicPOS.DTOs.Reporting;
using LogicPOS.Globalization;
using LogicPOS.Printing.Enums;
using LogicPOS.Printing.Templates;
using LogicPOS.Printing.Tickets;
using LogicPOS.Reporting.Common;
using LogicPOS.Settings;
using LogicPOS.Utility;
using SkiaSharp;
using SkiaSharp.QrCode.Image;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace LogicPOS.Printing.Documents
{
    public class FinanceDocumentMaster : BaseFinanceTemplate
    {

        private readonly PrintDocumentMasterDto _documentMaster;

        private readonly List<FinanceMasterViewReportDto> _financeMasters;
        private readonly List<FinanceDetailReportDto> _financeDetailsList;
        private readonly List<FinanceMasterTotalViewReportDto> _financeMasterTotalList;

        public FinanceDocumentMaster(
            PrintingPrinterDto printer,
            PrintDocumentMasterDto documentMaster,
            List<int> copyNames,
            bool isSecondCopy,
            string motive)
            : base(
                  printer,
                  documentMaster.DocumentType,
                  copyNames,
                  isSecondCopy)
        {

            _documentMaster = documentMaster;


            var financeMasters = ReportHelper.GetFinanceMasterViewReports(
                _documentMaster.Id)
                    .List.ConvertAll(
                            view => ReportMapping.GetFinanceMasterViewReportDto(view));

            _financeMasters = financeMasters;
            _financeDetailsList = financeMasters[0].DocumentFinanceDetail;
            _financeMasterTotalList = financeMasters[0].DocumentFinanceMasterTotal; ;
        }

        //Override Parent Template
        public override void PrintContent()
        {
            try
            {
                //Call base PrintDocumentMaster();
                PrintDocumentMaster(_financeMasters[0].DocumentTypeResourceString, _financeMasters[0].DocumentNumber, _financeMasters[0].DocumentDate);

                //Call 
                PrintDocumentMasterDocumentType();

                //Call base PrintCustomer();
                PrintCustomer(
                    _financeMasters[0].EntityName,
                    _financeMasters[0].EntityAddress,
                    _financeMasters[0].EntityZipCode,
                    _financeMasters[0].EntityCity,
                    _financeMasters[0].EntityCountry,
                    _financeMasters[0].EntityFiscalNumber
                );

                PrintDocumentDetails();
                PrintMasterTotals();
                PrintMasterTotalTax();
                PrintDocumenWayBillDetails();

                //Call base PrintDocumentPaymentDetails();
                PrintDocumentPaymentDetails(string.Empty, _financeMasters[0].PaymentMethodDesignation, _financeMasters[0].CurrencyAcronym); /* IN009055 */

                //Call base PrintNotes();
                //IN009279 Wont print notes on ticket mode 
                if (_financeMasters[0].Notes != null && !GeneralSettings.AppUseParkingTicketModule) PrintNotes(_financeMasters[0].Notes.ToString());

                //Only Print if is in Portugal ex "Os artigos faturados...."
                //Call base PrintDocumentTypeFooterString();
                try
                {
                    if (CultureSettings.CountryIdIsPortugal(XPOSettings.ConfigurationSystemCountry.Oid) || CultureSettings.CountryIdIsAngola(XPOSettings.ConfigurationSystemCountry.Oid))
                    {
                        PrintDocumentTypeFooterString(_financeMasters[0].DocumentTypeResourceStringReport);
                        //ATCUD Documentos - Criação do QRCode e ATCUD IN016508
                        //Print QRCode
                        if (Convert.ToBoolean(GeneralSettings.PreferenceParameters["PRINT_QRCODE"]) && !string.IsNullOrEmpty(_documentMaster.ATDocQRCode))
                        {
                            //PrintQRCode with buffer
                            //base.PrintQRCode(_documentMaster.ATDocQRCode);

                            //PrintQRCode with image
                            var qrCode = new QrCode(_documentMaster.ATDocQRCode, new Vector2Slim(256, 256), SKEncodedImageFormat.Png);
                            using (var output = new FileStream(@"temp/qrcode.Png", FileMode.OpenOrCreate))
                            {
                                qrCode.GenerateImage(output);
                            }
                            using (var bitmap = new System.Drawing.Bitmap(@"temp/qrcode.Png"))
                            {
                                bitmap.Save(@"temp/qrcode.Bmp");
                            }

                            PrintQRCodeImage(new System.Drawing.Bitmap(@"temp/qrcode.Bmp"));


                        }
                    }
                }
                catch (Exception ex) { _logger.Error("QRCode print error: " + ex.Message); }

                //Get Hash4Chars from Hash
                string hash4Chars = CryptographyUtils.GetDocumentHash4Chars(_documentMaster.Hash);
                //Call Base CertificationText 
                PrintCertificationText(hash4Chars);
            }
            catch (Exception ex)
            {
                _logger.Debug("override void PrintContent() :: Thermal Printer: " + ex.Message, ex);
                throw ex;
            }
        }

        //Print Specific DocumentType Details
        public void PrintDocumentMasterDocumentType()
        {
            //Set Align Center
            _genericThermalPrinter.SetAlignCenter();

            /* IN008024 */
            //string appOperationModeToken = LogicPOS.Settings.GeneralSettings.Settings["appOperationModeToken"].ToLower();

            //ConferenceDocument : Show Table if in ConferenceDocument and in default AppMode
            if (_documentType.Id == DocumentSettings.XpoOidDocumentFinanceTypeConferenceDocument && AppOperationModeSettings.IsDefaultTheme)
            {
                //Table|Order #2|Name/Zone
                string tableZone = string.Format("{0} : #{1}/{2}"
                    , CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, string.Format("global_table_appmode_{0}", AppOperationModeSettings.CustomAppOperationMode.AppOperationTheme.ToLower())) /* IN008024 */
                    , _documentMaster.TableDesignation
                    , _documentMaster.PlaceDesignation
                );
                _genericThermalPrinter.WriteLine(tableZone);
                _genericThermalPrinter.LineFeed();
            }

            //Reset Align 
            _genericThermalPrinter.SetAlignLeft();
        }

        //Loop Details
        public void PrintDocumentDetails()
        {
            try
            {
                List<TicketColumn> columns = new List<TicketColumn>();

                columns.Add(new TicketColumn("VatRate", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_vat_rate") + "%", 6, TicketColumnsAlignment.Right, typeof(decimal), "{0:00.00}"));

                columns.Add(new TicketColumn("Quantity", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_quantity_acronym"), 8, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"));
                columns.Add(new TicketColumn("UnitMeasure", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_unit_measure_acronym"), 3, TicketColumnsAlignment.Right));
                if (CultureSettings.CountryIdIsPortugal(XPOSettings.ConfigurationSystemCountry.Oid))
                {
                    columns.Add(new TicketColumn("UnitPrice", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_short_price"), 11, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"));
                }
                else
                {
                    columns.Add(new TicketColumn("Price", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_price"), 11, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"));
                }

                columns.Add(new TicketColumn("Discount", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_discount_acronym") + "%", 6, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"));
                //columns.Add(new TicketColumn("TotalNet", CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_totalnet_acronym, 9, TicketColumnsAlign.Right, typeof(decimal), "{0:0.00}"));
                columns.Add(new TicketColumn("TotalFinal", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_total_per_item"), 0, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"));//Dynamic
                /* IN009211 - end */

                //Prepare Table with Padding
                DataTable dataTable = TicketTable.InitDataTableFromTicketColumns(columns);
                TicketTable ticketTable = new TicketTable(dataTable, columns, _maxCharsPerLineNormal - _ticketTablePaddingLeftLength);
                string paddingLeftFormat = "  {0,-" + ticketTable.TableWidth + "}";//"  {0,-TableWidth}"
                //Print Table Headers
                ticketTable.Print(_genericThermalPrinter, paddingLeftFormat);

                //Print Items
                foreach (FinanceDetailReportDto item in _financeDetailsList)
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
                _logger.Debug("void PrintDocumentDetails() :: Thermal Printer: " + ex.Message, ex);
                throw ex;
            }
        }

        //Detail Row Block
        public void PrintDocumentDetail(
            TicketTable pTicketTable,
            FinanceDetailReportDto pFinanceDetail,
            string pPaddingLeftFormat)
        {
            try
            {
                //Trim Data
                string designation = (pFinanceDetail.Designation.Length <= _maxCharsPerLineNormalBold)
                    ? pFinanceDetail.Designation
                    : pFinanceDetail.Designation.Substring(0, _maxCharsPerLineNormalBold)
                ;
                //Print Item Designation : Bold
                _genericThermalPrinter.WriteLine(designation, WriteLineTextMode.Bold);

                //Prepare ExemptionReason
                string exemptionReason = string.Empty;
                if (!string.IsNullOrEmpty(pFinanceDetail.VatExemptionReasonDesignation))
                {
                    //Replaced Code : Remove Cut Exception Reason......its better not to cut VatExemptionReason.....
                    //exemptionReason = (pFinanceDetail.VatExemptionReasonDesignation.Length <= pTicketTable.TableWidth)
                    //    ? pFinanceDetail.VatExemptionReasonDesignation
                    //    : pFinanceDetail.VatExemptionReasonDesignation.Substring(0, pTicketTable.TableWidth)
                    //;
                    //Replace Code
                    exemptionReason = pFinanceDetail.VatExemptionReasonDesignation;
                    //Always Format Data, its appens in first line only
                    exemptionReason = string.Format(pPaddingLeftFormat, exemptionReason);
                }

                //Item Details
                DataRow dataRow = pTicketTable.NewRow();
                /* IN009211 block - begin 
                 * Method "LogicPOS.Utility.DataConversionUtils.DecimalToString(pFinanceDetail.Vat)" is safe for "%" format
                 */
                dataRow[0] = pFinanceDetail.Vat;
                dataRow[1] = pFinanceDetail.Quantity;
                dataRow[2] = pFinanceDetail.UnitMeasure;
                //Layout talões PT - Preço Unitário em vez de Preço sem IVA [IN:016509]
                dataRow[3] = pFinanceDetail.UnitPrice;
                dataRow[4] = pFinanceDetail.Discount;
                //dataRow[5] = pFinanceDetail.TotalNet * _documentFinanceMasterList[0].ExchangeRate;
                /* fix for item total before CustomerDiscount: (TotalGross - ItemDiscount) + ItemVAT */
                decimal amountItemDiscount = pFinanceDetail.TotalGross * pFinanceDetail.Discount / 100;
                decimal amountDueBeforeTax = (pFinanceDetail.TotalGross - amountItemDiscount);
                decimal totalItemTax = amountDueBeforeTax * pFinanceDetail.Vat / 100;
                decimal amountDueAfterTax = amountDueBeforeTax + totalItemTax;
                dataRow[5] = _financeMasters[0].ExchangeRate * amountDueAfterTax;
                /* IN009211 block - end */

                //Add DataRow to Table, Ready for Print
                pTicketTable.Rows.Add(dataRow);
                //Print Table Rows
                pTicketTable.Print(_genericThermalPrinter, WriteLineTextMode.Normal, true, pPaddingLeftFormat);

                //VatExemptionReason
                _genericThermalPrinter.WriteLine(exemptionReason, WriteLineTextMode.Small); /* IN009211 - WriteLine() already checks for empties and nulls */
            }
            catch (Exception ex)
            {
                _logger.Debug("void PrintDocumentDetail(TicketTable pTicketTable, FRBODocumentFinanceDetail pFinanceDetail, string pPaddingLeftFormat) :: Thermal Printer: " + ex.Message, ex);
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

                //Add Row : Discount
                if (_financeMasters[0].Discount > 0.0m)
                {
                    dataRow = dataTable.NewRow();
                    dataRow[0] = string.Format("{0} (%)", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_documentfinance_discount_customer")); /* IN009211 */
                    dataRow[1] = DataConversionUtils.DecimalToString(_financeMasters[0].Discount);
                    dataTable.Rows.Add(dataRow);
                }
                //Add Row : TotalDiscount
                if (_financeMasters[0].TotalDiscount > 0.0m)
                {
                    dataRow = dataTable.NewRow();
                    dataRow[0] = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_documentfinance_total_discount"); /* IN009211 */
                    dataRow[1] = DataConversionUtils.DecimalToString(_financeMasters[0].TotalDiscount * _financeMasters[0].ExchangeRate);
                    dataTable.Rows.Add(dataRow);
                }

                /* #TODO IN009214 */
                //Add Row : Discount PaymentCondition
                //WIP Always 0

                //Add Row : TotalNet
                dataRow = dataTable.NewRow();
                dataRow[0] = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_totalnet"); /* IN009211 */
                //dataRow[1] = LogicPOS.Utility.DataConversionUtils.DecimalToString(_documentFinanceMasterList[0].TotalGross * _documentFinanceMasterList[0].ExchangeRate);
                dataRow[1] = DataConversionUtils.DecimalToString(_financeMasters[0].TotalNet * _financeMasters[0].ExchangeRate);
                dataTable.Rows.Add(dataRow);
                //Add Row : TotalTax
                dataRow = dataTable.NewRow();
                dataRow[0] = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_documentfinance_totaltax"); /* IN009211 */
                dataRow[1] = DataConversionUtils.DecimalToString(_financeMasters[0].TotalTax * _financeMasters[0].ExchangeRate);
                dataTable.Rows.Add(dataRow);
                //Add Row : TotalFinal
                dataRow = dataTable.NewRow();
                dataRow[0] = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_documentfinance_totalfinal");
                dataRow[1] = DataConversionUtils.DecimalToString(_financeMasters[0].TotalFinal * _financeMasters[0].ExchangeRate);
                dataTable.Rows.Add(dataRow);

                //If Simplified Invoice, Payment Method MONEY and has Total Change, add it
                if (new Guid(_financeMasters[0].DocumentType) == DocumentSettings.XpoOidDocumentFinanceTypeSimplifiedInvoice
                    && _financeMasters[0].PaymentMethodToken == "MONEY"
                    && _financeMasters[0].TotalChange > 0
                    )
                {
                    //Blank Row, to separate from Main Totals
                    dataRow = dataTable.NewRow();
                    dataRow[0] = string.Empty;
                    dataRow[1] = string.Empty;
                    dataTable.Rows.Add(dataRow);
                    //TotalDelivery
                    dataRow = dataTable.NewRow();
                    dataRow[0] = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_total_deliver");
                    dataRow[1] = DataConversionUtils.DecimalToString(_financeMasters[0].TotalDelivery * _financeMasters[0].ExchangeRate);
                    dataTable.Rows.Add(dataRow);
                    //TotalChange
                    dataRow = dataTable.NewRow();
                    dataRow[0] = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_total_change");
                    dataRow[1] = DataConversionUtils.DecimalToString(_financeMasters[0].TotalChange * _financeMasters[0].ExchangeRate);
                    dataTable.Rows.Add(dataRow);
                }

                /* IN009055 - related to IN005976 for Mozambique deployment */
                if (CultureSettings.MozambiqueCountryId.Equals(XPOSettings.ConfigurationSystemCountry.Oid))
                {
                    CurrenyDto defaultCurrencyForExchangeRate = XPOUtility.GetUsdCurrencyDto();

                    dataRow = dataTable.NewRow();
                    dataRow[0] = string.Format(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_printer_thermal_total_default_currency"), defaultCurrencyForExchangeRate.Acronym);
                    dataRow[1] = DataConversionUtils.DecimalToString(_financeMasters[0].TotalFinal * defaultCurrencyForExchangeRate.ExchangeRate);/* TO DO : IN009055 - this causes total equals 0,00 when low product price */
                    dataTable.Rows.Add(dataRow);
                }

                //Configure Ticket Column Properties
                List<TicketColumn> columns = new List<TicketColumn>
                {
                    new TicketColumn("Label", "", Convert.ToInt16(_maxCharsPerLineNormal / 2) - 2, TicketColumnsAlignment.Left),
                    new TicketColumn("Value", "", Convert.ToInt16(_maxCharsPerLineNormal / 2) - 2, TicketColumnsAlignment.Right)
                };

                //TicketTable(DataTable pDataTable, List<TicketColumn> pColumnsProperties, int pTableWidth)
                TicketTable ticketTable = new TicketTable(dataTable, columns, _genericThermalPrinter.MaxCharsPerLineNormalBold);

                //Custom Print Loop, to Print all Table Rows, and Detect Rows to Print in DoubleHeight (TotalChange(4) and Total(7))
                List<string> table = ticketTable.GetTable();
                WriteLineTextMode rowTextMode;
                //Dynamic Print All, some Rows ommited/bold(Table Header, TotalDocument,TotalChange)
                for (int i = 1; i < table.Count; i++)
                {
                    //Prepare TextMode Based on Row
                    rowTextMode = (i == 5 || i == 8) ? WriteLineTextMode.DoubleHeightBold : WriteLineTextMode.Bold;
                    //Print Row
                    _genericThermalPrinter.WriteLine(table[i], rowTextMode);
                }

                //Line Feed
                _genericThermalPrinter.LineFeed();
            }
            catch (Exception ex)
            {
                _logger.Debug("void PrintMasterTotals() :: Thermal Printer: " + ex.Message, ex);
                throw ex;
            }
        }

        private void PrintMasterTotalTax()
        {
            try
            {
                //Init DataTable
                DataRow dataRow = null;
                DataTable dataTable = new DataTable();
                DataColumn dcDesignation = new DataColumn("Designation", typeof(string));
                DataColumn dcTax = new DataColumn("Tax", typeof(string));
                DataColumn dcTaxBase = new DataColumn("TaxBase", typeof(string));
                DataColumn dcTotal = new DataColumn("Total", typeof(string));
                dataTable.Columns.Add(dcDesignation);
                dataTable.Columns.Add(dcTax);
                dataTable.Columns.Add(dcTaxBase);
                dataTable.Columns.Add(dcTotal);

                foreach (FinanceMasterTotalViewReportDto item in _financeMasterTotalList)
                {
                    dataRow = dataTable.NewRow();
                    dataRow[0] = item.Designation;
                    dataRow[1] = string.Format("{0}%", DataConversionUtils.DecimalToString(item.Value));
                    dataRow[2] = DataConversionUtils.DecimalToString(_financeMasters[0].ExchangeRate * item.TotalBase);
                    dataRow[3] = DataConversionUtils.DecimalToString(_financeMasters[0].ExchangeRate * item.Total);
                    dataTable.Rows.Add(dataRow);
                }

                //Configure Ticket Column Properties
                List<TicketColumn> columns = new List<TicketColumn>
                {
                    new TicketColumn("Designation", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_designation"), 0, TicketColumnsAlignment.Left),
                    new TicketColumn("Tax", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_tax"), 8, TicketColumnsAlignment.Right),
                    new TicketColumn("TotalBase", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_total_tax_base"), 12, TicketColumnsAlignment.Right),
                    new TicketColumn("Total", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_documentfinance_totaltax_acronym"), 10, TicketColumnsAlignment.Right)
                };

                //TicketTable(DataTable pDataTable, List<TicketColumn> pColumnsProperties, int pTableWidth)
                TicketTable ticketTable = new TicketTable(dataTable, columns, _genericThermalPrinter.MaxCharsPerLineNormal);
                //Print Table Buffer
                ticketTable.Print(_genericThermalPrinter);

                //Line Feed
                _genericThermalPrinter.LineFeed();
            }
            catch (Exception ex)
            {
                _logger.Debug("void PrintMasterTotalTax() :: Thermal Printer: " + ex.Message, ex);
                throw ex;
            }
        }

        public void PrintDocumenWayBillDetails()
        {
            try
            {
                //If Simplified Invoice, Payment Method MONEY and has Total Change, add it
                if (_financeMasters[0].DocumentTypeWayBill)
                {
                    //WayBill Local Load
                    _genericThermalPrinter.WriteLine(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_documentfinance_waybill_local_load"), WriteLineTextMode.Bold);
                    _genericThermalPrinter.WriteLine(string.Format("{0} {1}", _financeMasters[0].ShipFromAddressDetail, _financeMasters[0].ShipFromCity));
                    _genericThermalPrinter.WriteLine(string.Format("{0} {1} [{2}]", _financeMasters[0].ShipFromPostalCode, _financeMasters[0].ShipFromRegion, _financeMasters[0].ShipFromCountry));
                    _genericThermalPrinter.WriteLine(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_ship_from_delivery_date_report"), XPOUtility.DateTimeToString(_financeMasters[0].ShipFromDeliveryDate));
                    _genericThermalPrinter.WriteLine(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_ship_from_delivery_id_report"), _financeMasters[0].ShipFromDeliveryID);
                    _genericThermalPrinter.WriteLine(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_ship_from_warehouse_id_report"), _financeMasters[0].ShipFromWarehouseID);
                    _genericThermalPrinter.WriteLine(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_ship_from_location_id_report"), _financeMasters[0].ShipFromLocationID);

                    //Separate with Blank Line
                    _genericThermalPrinter.LineFeed();

                    //WayBill Local Download
                    _genericThermalPrinter.WriteLine(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_documentfinance_waybill_local_download"), WriteLineTextMode.Bold);
                    _genericThermalPrinter.WriteLine(string.Format("{0} {1}", _financeMasters[0].ShipToAddressDetail, _financeMasters[0].ShipToCity));
                    _genericThermalPrinter.WriteLine(string.Format("{0} {1} [{2}]", _financeMasters[0].ShipToPostalCode, _financeMasters[0].ShipToRegion, _financeMasters[0].ShipToCountry));
                    _genericThermalPrinter.WriteLine(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_ship_to_delivery_date_report"), XPOUtility.DateTimeToString(_financeMasters[0].ShipToDeliveryDate));
                    _genericThermalPrinter.WriteLine(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_ship_to_delivery_id_report"), _financeMasters[0].ShipToDeliveryID);
                    _genericThermalPrinter.WriteLine(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_ship_to_warehouse_id_report"), _financeMasters[0].ShipToWarehouseID);
                    _genericThermalPrinter.WriteLine(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_ship_to_location_id_report"), _financeMasters[0].ShipToLocationID);

                    //Line Feed
                    _genericThermalPrinter.LineFeed();

                    //ATDocCodeID
                    if (!string.IsNullOrEmpty(_financeMasters[0].ATDocCodeID))
                    {
                        //Set Align Center
                        _genericThermalPrinter.SetAlignCenter();

                        //WayBill Local Load
                        _genericThermalPrinter.WriteLine(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_at_atdoccodeid"), WriteLineTextMode.Bold);
                        _genericThermalPrinter.WriteLine(_financeMasters[0].ATDocCodeID, WriteLineTextMode.DoubleHeight);

                        //Reset Align 
                        _genericThermalPrinter.SetAlignLeft();

                        //Line Feed
                        _genericThermalPrinter.LineFeed();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Debug("void PrintDocumenWayBillDetails() :: Thermal Printer: " + ex.Message, ex);
                throw ex;
            }
        }
    }
}
