using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.library.Classes.Finance;
using logicpos.financial.library.Classes.Hardware.Printers.Thermal.Enums;
using logicpos.financial.library.Classes.Reports.BOs;
using logicpos.financial.library.Classes.Reports.BOs.Documents;
using System;
using System.Collections.Generic;
using System.Data;
using SkiaSharp;
using SkiaSharp.QrCode.Image;
using System.IO;
using logicpos.datalayer.App;
using logicpos.shared.App;
using logicpos.datalayer.Xpo;
using LogicPOS.Settings.Extensions;
using LogicPOS.Globalization;

namespace logicpos.financial.library.Classes.Hardware.Printers.Thermal.Tickets
{
    public class ThermalPrinterFinanceDocumentMaster : ThermalPrinterBaseFinanceTemplate
    {
        //Parameters Properties
        private readonly fin_documentfinancemaster _documentMaster;
        //Business Objects
        private readonly List<FRBODocumentFinanceMasterView> _documentFinanceMasterList;
        private readonly List<FRBODocumentFinanceDetail> _documentFinanceDetailList;
        private readonly List<FRBODocumentFinanceMasterTotalView> _documentFinanceMasterTotalList;

        public ThermalPrinterFinanceDocumentMaster(sys_configurationprinters pPrinter, fin_documentfinancemaster pDocumentMaster, List<int> pCopyNames, bool pSecondCopy, string pMotive)
            : base(pPrinter, pDocumentMaster.DocumentType, pCopyNames, pSecondCopy)
        {
            try
            {
                //Parameters
                _documentMaster = pDocumentMaster;

                //Init Fast Reports Business Objects (From FRBOHelper)
                ResultFRBODocumentFinanceMaster fRBOHelperResponseProcessReportFinanceDocument = FRBOHelper.GetFRBOFinanceDocument(_documentMaster.Oid);
                //Get FRBOs Lists 
                _documentFinanceMasterList = fRBOHelperResponseProcessReportFinanceDocument.DocumentFinanceMaster.List;
                _documentFinanceDetailList = fRBOHelperResponseProcessReportFinanceDocument.DocumentFinanceMaster.List[0].DocumentFinanceDetail;
                _documentFinanceMasterTotalList = fRBOHelperResponseProcessReportFinanceDocument.DocumentFinanceMaster.List[0].DocumentFinanceMasterTotal; ;
            }
            catch (Exception ex)
            {
                _logger.Debug("ThermalPrinterFinanceDocumentMaster(sys_configurationprinters pPrinter, fin_documentfinancemaster pDocumentMaster, List<int> pCopyNames, bool pSecondCopy, string pMotive) :: " + ex.Message, ex);
                throw ex;
            }
        }

        //Override Parent Template
        public override void PrintContent()
        {
            try
            {
                //Call base PrintDocumentMaster();
                PrintDocumentMaster(_documentFinanceMasterList[0].DocumentTypeResourceString, _documentFinanceMasterList[0].DocumentNumber, _documentFinanceMasterList[0].DocumentDate);

                //Call 
                PrintDocumentMasterDocumentType();

                //Call base PrintCustomer();
                PrintCustomer(
                    _documentFinanceMasterList[0].EntityName,
                    _documentFinanceMasterList[0].EntityAddress,
                    _documentFinanceMasterList[0].EntityZipCode,
                    _documentFinanceMasterList[0].EntityCity,
                    _documentFinanceMasterList[0].EntityCountry,
                    _documentFinanceMasterList[0].EntityFiscalNumber
                );

                PrintDocumentDetails();
                PrintMasterTotals();
                PrintMasterTotalTax();
                PrintDocumenWayBillDetails();

                //Call base PrintDocumentPaymentDetails();
                PrintDocumentPaymentDetails(string.Empty, _documentFinanceMasterList[0].PaymentMethodDesignation, _documentFinanceMasterList[0].CurrencyAcronym); /* IN009055 */

                //Call base PrintNotes();
                //IN009279 Wont print notes on ticket mode 
                if (_documentFinanceMasterList[0].Notes != null && !SharedFramework.AppUseParkingTicketModule) PrintNotes(_documentFinanceMasterList[0].Notes.ToString());

                //Only Print if is in Portugal ex "Os artigos faturados...."
                //Call base PrintDocumentTypeFooterString();
                try
                {
                    if (DataLayerSettings.ConfigurationSystemCountry.Oid == SharedSettings.XpoOidConfigurationCountryPortugal || DataLayerSettings.ConfigurationSystemCountry.Oid == SharedSettings.XpoOidConfigurationCountryAngola)
                    {
                        PrintDocumentTypeFooterString(_documentFinanceMasterList[0].DocumentTypeResourceStringReport);
                        //ATCUD Documentos - Criação do QRCode e ATCUD IN016508
                        //Print QRCode
                        if (Convert.ToBoolean(LogicPOS.Settings.GeneralSettings.PreferenceParameters["PRINT_QRCODE"]) && !string.IsNullOrEmpty(_documentMaster.ATDocQRCode))
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
                }catch(Exception ex) { _logger.Error("QRCode print error: " + ex.Message); }

                //Get Hash4Chars from Hash
                string hash4Chars = ProcessFinanceDocument.GenDocumentHash4Chars(_documentMaster.Hash);
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
            _thermalPrinterGeneric.SetAlignCenter();

            /* IN008024 */
            //string appOperationModeToken = LogicPOS.Settings.GeneralSettings.Settings["appOperationModeToken"].ToLower();

            //ConferenceDocument : Show Table if in ConferenceDocument and in default AppMode
            if (_documentType.Oid == SharedSettings.XpoOidDocumentFinanceTypeConferenceDocument && DataLayerSettings.IsDefaultTheme)
            {
                //Table|Order #2|Name/Zone
                string tableZone = string.Format("{0} : #{1}/{2}"
                    , CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), string.Format("global_table_appmode_{0}", DataLayerSettings.CustomAppOperationMode.AppOperationTheme.ToLower())) /* IN008024 */
                    , _documentMaster.SourceOrderMain.PlaceTable.Designation
                    , _documentMaster.SourceOrderMain.PlaceTable.Place.Designation
                );
                _thermalPrinterGeneric.WriteLine(tableZone);
                _thermalPrinterGeneric.LineFeed();
            }

            //Reset Align 
            _thermalPrinterGeneric.SetAlignLeft();
        }

        //Loop Details
        public void PrintDocumentDetails()
        {
            try
            {
                List<TicketColumn> columns = new List<TicketColumn>();
                //columns.Add(new TicketColumn("Article", CultureResources.GetCustomResources(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_article_acronym, 0, TicketColumnsAlign.Right, typeof(string), "{0:0.00}"));
                /* IN009211 block - begin 
                 * 
                 * We opt for "(%)" symbol to be added to value itself because of column title lenght limit, and for this, we changed from Decimal to String here.
                 * The conversion will be done when printing the DataRow for VatRate and Discount fields (LogicPOS.Utility.DataConversionUtils.DecimalToString(pFinanceDetail.Vat)).
                 */
                //Colum
                if (DataLayerSettings.ConfigurationSystemCountry.Oid == SharedSettings.XpoOidConfigurationCountryPortugal){
                    columns.Add(new TicketColumn("VatRate", CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "IVA") + "%", 6, TicketColumnsAlign.Right, typeof(decimal), "{0:00.00}"));
                }
                else
                {
                    columns.Add(new TicketColumn("VatRate", CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_vat_rate") + "%", 6, TicketColumnsAlign.Right, typeof(decimal), "{0:00.00}"));
                }                
                columns.Add(new TicketColumn("Quantity", CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_quantity_acronym"), 8, TicketColumnsAlign.Right, typeof(decimal), "{0:0.00}"));
                columns.Add(new TicketColumn("UnitMeasure", CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_unit_measure_acronym"), 3, TicketColumnsAlign.Right));
                if (DataLayerSettings.ConfigurationSystemCountry.Oid == SharedSettings.XpoOidConfigurationCountryPortugal)
                {
                    columns.Add(new TicketColumn("UnitPrice", CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_short_price"), 11, TicketColumnsAlign.Right, typeof(decimal), "{0:0.00}"));
                }
                else
                {
                    columns.Add(new TicketColumn("Price", CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_price"), 11, TicketColumnsAlign.Right, typeof(decimal), "{0:0.00}"));
                }

                columns.Add(new TicketColumn("Discount", CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_discount_acronym") + "%", 6, TicketColumnsAlign.Right, typeof(decimal), "{0:0.00}"));
                //columns.Add(new TicketColumn("TotalNet", CultureResources.GetCustomResources(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_totalnet_acronym, 9, TicketColumnsAlign.Right, typeof(decimal), "{0:0.00}"));
                columns.Add(new TicketColumn("TotalFinal", CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_total_per_item"), 0, TicketColumnsAlign.Right, typeof(decimal), "{0:0.00}"));//Dynamic
                /* IN009211 - end */

                //Prepare Table with Padding
                DataTable dataTable = TicketTable.InitDataTableFromTicketColumns(columns);
                TicketTable ticketTable = new TicketTable(dataTable, columns, _maxCharsPerLineNormal - _ticketTablePaddingLeftLength);
                string paddingLeftFormat = "  {0,-" + ticketTable.TableWidth + "}";//"  {0,-TableWidth}"
                //Print Table Headers
                ticketTable.Print(_thermalPrinterGeneric, paddingLeftFormat);

                //Print Items
                foreach (FRBODocumentFinanceDetail item in _documentFinanceDetailList)
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
                _logger.Debug("void PrintDocumentDetails() :: Thermal Printer: " + ex.Message, ex);
                throw ex;
            }
        }

        //Detail Row Block
        public void PrintDocumentDetail(TicketTable pTicketTable, FRBODocumentFinanceDetail pFinanceDetail, string pPaddingLeftFormat)
        {
            try
            {
                //Trim Data
                string designation = (pFinanceDetail.Designation.Length <= _maxCharsPerLineNormalBold)
                    ? pFinanceDetail.Designation
                    : pFinanceDetail.Designation.Substring(0, _maxCharsPerLineNormalBold)
                ;
                //Print Item Designation : Bold
                _thermalPrinterGeneric.WriteLine(designation, WriteLineTextMode.Bold);

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
                dataRow[5] = _documentFinanceMasterList[0].ExchangeRate * amountDueAfterTax;
                /* IN009211 block - end */

                //Add DataRow to Table, Ready for Print
                pTicketTable.Rows.Add(dataRow);
                //Print Table Rows
                pTicketTable.Print(_thermalPrinterGeneric, WriteLineTextMode.Normal, true, pPaddingLeftFormat);

                //VatExemptionReason
                _thermalPrinterGeneric.WriteLine(exemptionReason, WriteLineTextMode.Small); /* IN009211 - WriteLine() already checks for empties and nulls */
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
                if (_documentFinanceMasterList[0].Discount > 0.0m)
                {
                    dataRow = dataTable.NewRow();
                    dataRow[0] = string.Format("{0} (%)", CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_documentfinance_discount_customer")); /* IN009211 */
                    dataRow[1] = LogicPOS.Utility.DataConversionUtils.DecimalToString(_documentFinanceMasterList[0].Discount);
                    dataTable.Rows.Add(dataRow);
                }
                //Add Row : TotalDiscount
                if (_documentFinanceMasterList[0].TotalDiscount > 0.0m)
                {
                    dataRow = dataTable.NewRow();
                    dataRow[0] = CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_documentfinance_total_discount"); /* IN009211 */
                    dataRow[1] = LogicPOS.Utility.DataConversionUtils.DecimalToString(_documentFinanceMasterList[0].TotalDiscount * _documentFinanceMasterList[0].ExchangeRate);
                    dataTable.Rows.Add(dataRow);
                }
                
                /* #TODO IN009214 */
                //Add Row : Discount PaymentCondition
                //WIP Always 0

                //Add Row : TotalNet
                dataRow = dataTable.NewRow();
                dataRow[0] = CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_totalnet"); /* IN009211 */
                //dataRow[1] = LogicPOS.Utility.DataConversionUtils.DecimalToString(_documentFinanceMasterList[0].TotalGross * _documentFinanceMasterList[0].ExchangeRate);
                dataRow[1] = LogicPOS.Utility.DataConversionUtils.DecimalToString(_documentFinanceMasterList[0].TotalNet * _documentFinanceMasterList[0].ExchangeRate);
                dataTable.Rows.Add(dataRow);
                //Add Row : TotalTax
                dataRow = dataTable.NewRow();
                dataRow[0] = CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_documentfinance_totaltax"); /* IN009211 */
                dataRow[1] = LogicPOS.Utility.DataConversionUtils.DecimalToString(_documentFinanceMasterList[0].TotalTax * _documentFinanceMasterList[0].ExchangeRate);
                dataTable.Rows.Add(dataRow);
                //Add Row : TotalFinal
                dataRow = dataTable.NewRow();
                dataRow[0] = CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_documentfinance_totalfinal");
                dataRow[1] = LogicPOS.Utility.DataConversionUtils.DecimalToString(_documentFinanceMasterList[0].TotalFinal * _documentFinanceMasterList[0].ExchangeRate);
                dataTable.Rows.Add(dataRow);

                //If Simplified Invoice, Payment Method MONEY and has Total Change, add it
                if (new Guid(_documentFinanceMasterList[0].DocumentType) == SharedSettings.XpoOidDocumentFinanceTypeSimplifiedInvoice
                    && _documentFinanceMasterList[0].PaymentMethodToken == "MONEY"
                    && _documentFinanceMasterList[0].TotalChange > 0
                    )
                {
                    //Blank Row, to separate from Main Totals
                    dataRow = dataTable.NewRow();
                    dataRow[0] = string.Empty;
                    dataRow[1] = string.Empty;
                    dataTable.Rows.Add(dataRow);
                    //TotalDelivery
                    dataRow = dataTable.NewRow();
                    dataRow[0] = CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_total_deliver");
                    dataRow[1] = LogicPOS.Utility.DataConversionUtils.DecimalToString(_documentFinanceMasterList[0].TotalDelivery * _documentFinanceMasterList[0].ExchangeRate);
                    dataTable.Rows.Add(dataRow);
                    //TotalChange
                    dataRow = dataTable.NewRow();
                    dataRow[0] = CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_total_change");
                    dataRow[1] = LogicPOS.Utility.DataConversionUtils.DecimalToString(_documentFinanceMasterList[0].TotalChange * _documentFinanceMasterList[0].ExchangeRate);
                    dataTable.Rows.Add(dataRow);
                }

                /* IN009055 - related to IN005976 for Mozambique deployment */
                if (SharedSettings.XpoOidConfigurationCountryMozambique.Equals(DataLayerSettings.ConfigurationSystemCountry.Oid))
                {
                    cfg_configurationcurrency defaultCurrencyForExchangeRate =
                        (cfg_configurationcurrency)DataLayerUtils.GetXPGuidObject(
                            XPOSettings.Session,
                            typeof(cfg_configurationcurrency),
                            SharedSettings.XpoOidConfigurationCurrencyUSDollar);

                    dataRow = dataTable.NewRow();
                    dataRow[0] = string.Format(CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_printer_thermal_total_default_currency"), defaultCurrencyForExchangeRate.Acronym);
                    dataRow[1] = LogicPOS.Utility.DataConversionUtils.DecimalToString(_documentFinanceMasterList[0].TotalFinal * defaultCurrencyForExchangeRate.ExchangeRate);/* TO DO : IN009055 - this causes total equals 0,00 when low product price */
                    dataTable.Rows.Add(dataRow);
                }

                //Configure Ticket Column Properties
                List<TicketColumn> columns = new List<TicketColumn>
                {
                    new TicketColumn("Label", "", Convert.ToInt16(_maxCharsPerLineNormal / 2) - 2, TicketColumnsAlign.Left),
                    new TicketColumn("Value", "", Convert.ToInt16(_maxCharsPerLineNormal / 2) - 2, TicketColumnsAlign.Right)
                };

                //TicketTable(DataTable pDataTable, List<TicketColumn> pColumnsProperties, int pTableWidth)
                TicketTable ticketTable = new TicketTable(dataTable, columns, _thermalPrinterGeneric.MaxCharsPerLineNormalBold);

                //Custom Print Loop, to Print all Table Rows, and Detect Rows to Print in DoubleHeight (TotalChange(4) and Total(7))
                List<string> table = ticketTable.GetTable();
                WriteLineTextMode rowTextMode;
                //Dynamic Print All, some Rows ommited/bold(Table Header, TotalDocument,TotalChange)
                for (int i = 1; i < table.Count; i++)
                {
                    //Prepare TextMode Based on Row
                    rowTextMode = (i == 5 || i == 8) ? WriteLineTextMode.DoubleHeightBold : WriteLineTextMode.Bold;
                    //Print Row
                    _thermalPrinterGeneric.WriteLine(table[i], rowTextMode);
                }

                //Line Feed
                _thermalPrinterGeneric.LineFeed();
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

                foreach (FRBODocumentFinanceMasterTotalView item in _documentFinanceMasterTotalList)
                {
                    dataRow = dataTable.NewRow();
                    dataRow[0] = item.Designation;
                    dataRow[1] = string.Format("{0}%", LogicPOS.Utility.DataConversionUtils.DecimalToString(item.Value));
                    dataRow[2] = LogicPOS.Utility.DataConversionUtils.DecimalToString(_documentFinanceMasterList[0].ExchangeRate * item.TotalBase);
                    dataRow[3] = LogicPOS.Utility.DataConversionUtils.DecimalToString(_documentFinanceMasterList[0].ExchangeRate * item.Total);
                    dataTable.Rows.Add(dataRow);
                }

                //Configure Ticket Column Properties
                List<TicketColumn> columns = new List<TicketColumn>
                {
                    new TicketColumn("Designation", CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_designation"), 0, TicketColumnsAlign.Left),
                    new TicketColumn("Tax", CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_tax"), 8, TicketColumnsAlign.Right),
                    new TicketColumn("TotalBase", CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_total_tax_base"), 12, TicketColumnsAlign.Right),
                    new TicketColumn("Total", CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_documentfinance_totaltax_acronym"), 10, TicketColumnsAlign.Right)
                };

                //TicketTable(DataTable pDataTable, List<TicketColumn> pColumnsProperties, int pTableWidth)
                TicketTable ticketTable = new TicketTable(dataTable, columns, _thermalPrinterGeneric.MaxCharsPerLineNormal);
                //Print Table Buffer
                ticketTable.Print(_thermalPrinterGeneric);

                //Line Feed
                _thermalPrinterGeneric.LineFeed();
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
                if (_documentFinanceMasterList[0].DocumentTypeWayBill)
                {
                    //WayBill Local Load
                    _thermalPrinterGeneric.WriteLine(CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_documentfinance_waybill_local_load"), WriteLineTextMode.Bold);
                    _thermalPrinterGeneric.WriteLine(string.Format("{0} {1}", _documentFinanceMasterList[0].ShipFromAddressDetail, _documentFinanceMasterList[0].ShipFromCity));
                    _thermalPrinterGeneric.WriteLine(string.Format("{0} {1} [{2}]", _documentFinanceMasterList[0].ShipFromPostalCode, _documentFinanceMasterList[0].ShipFromRegion, _documentFinanceMasterList[0].ShipFromCountry));
                    _thermalPrinterGeneric.WriteLine(CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_ship_from_delivery_date_report"), SharedUtils.DateTimeToString(_documentFinanceMasterList[0].ShipFromDeliveryDate));
                    _thermalPrinterGeneric.WriteLine(CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_ship_from_delivery_id_report"), _documentFinanceMasterList[0].ShipFromDeliveryID);
                    _thermalPrinterGeneric.WriteLine(CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_ship_from_warehouse_id_report"), _documentFinanceMasterList[0].ShipFromWarehouseID);
                    _thermalPrinterGeneric.WriteLine(CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_ship_from_location_id_report"), _documentFinanceMasterList[0].ShipFromLocationID);

                    //Separate with Blank Line
                    _thermalPrinterGeneric.LineFeed();

                    //WayBill Local Download
                    _thermalPrinterGeneric.WriteLine(CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_documentfinance_waybill_local_download"), WriteLineTextMode.Bold);
                    _thermalPrinterGeneric.WriteLine(string.Format("{0} {1}", _documentFinanceMasterList[0].ShipToAddressDetail, _documentFinanceMasterList[0].ShipToCity));
                    _thermalPrinterGeneric.WriteLine(string.Format("{0} {1} [{2}]", _documentFinanceMasterList[0].ShipToPostalCode, _documentFinanceMasterList[0].ShipToRegion, _documentFinanceMasterList[0].ShipToCountry));
                    _thermalPrinterGeneric.WriteLine(CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_ship_to_delivery_date_report"), SharedUtils.DateTimeToString(_documentFinanceMasterList[0].ShipToDeliveryDate));
                    _thermalPrinterGeneric.WriteLine(CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_ship_to_delivery_id_report"), _documentFinanceMasterList[0].ShipToDeliveryID);
                    _thermalPrinterGeneric.WriteLine(CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_ship_to_warehouse_id_report"), _documentFinanceMasterList[0].ShipToWarehouseID);
                    _thermalPrinterGeneric.WriteLine(CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_ship_to_location_id_report"), _documentFinanceMasterList[0].ShipToLocationID);

                    //Line Feed
                    _thermalPrinterGeneric.LineFeed();

                    //ATDocCodeID
                    if (!string.IsNullOrEmpty(_documentFinanceMasterList[0].ATDocCodeID))
                    {
                        //Set Align Center
                        _thermalPrinterGeneric.SetAlignCenter();

                        //WayBill Local Load
                        _thermalPrinterGeneric.WriteLine(CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_at_atdoccodeid"), WriteLineTextMode.Bold);
                        _thermalPrinterGeneric.WriteLine(_documentFinanceMasterList[0].ATDocCodeID, WriteLineTextMode.DoubleHeight);

                        //Reset Align 
                        _thermalPrinterGeneric.SetAlignLeft();

                        //Line Feed
                        _thermalPrinterGeneric.LineFeed();
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
