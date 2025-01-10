using LogicPOS.Api.Entities;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.DTOs.Common;
using LogicPOS.DTOs.Printing;
using LogicPOS.DTOs.Reporting;
using LogicPOS.Printing.Enums;
using LogicPOS.Printing.Templates;
using LogicPOS.Printing.Tickets;
using LogicPOS.Settings;
//using LogicPOS.UI;
using LogicPOS.Utility;
using SkiaSharp;
using SkiaSharp.QrCode.Image;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace LogicPOS.Printing.Documents
{
    public class FinanceMaster : BaseFinanceTemplate
    {

        private readonly Document _documentMaster;//readonly PrintDocumentMasterDto _documentMaster;
        private readonly List<FinanceMasterViewReportDataDto> _financeMasterViewReportsDtos;
        private readonly List<DocumentDetail> _documentDetails;

        public FinanceMaster(
            PrinterDto printer,
            string terminalDesignation,
            string userName,
            Document documentMaster,
            CompanyPrintingInformationsDto companyInformationsDto,
            List<int> copyNames,
            bool isSecondCopy,
            string motive
            )
            : base(
                  printer,documentMaster,terminalDesignation,userName,companyInformationsDto, copyNames)
        {

            _documentMaster = documentMaster;
            _companyInformationsDto = companyInformationsDto;
            _documentDetails = documentMaster.Details.ToList();
            //_financeMasterTotalViewReportsDtos = financeMasterViewReportsDtos[0].DocumentFinanceMasterTotal; ;
        }

        //Override Parent Template
        public override void PrintContent()
        {
            var documentType="global_documentfinance_type_title_fr";
            documentType =documentType.Substring(0,documentType.Length-2)+_documentMaster.Number.Substring(0,2).ToLower();
            PrintDocumentMaster(documentType, _documentMaster.Number, _documentMaster.Date);
            

            //Call 
            PrintDocumentMasterDocumentType();

            //Call base PrintCustomer();
            PrintCustomer(
                _documentMaster.Customer.Name,
                _documentMaster.Customer.Address,
                _documentMaster.Customer.ZipCode,
                _documentMaster.Customer.City,
                _documentMaster.Customer.Country,
                _documentMaster.Customer.FiscalNumber
            );

            PrintDocumentDetails();
            PrintMasterTotals();
            PrintMasterTotalTax();
            PrintDocumenWayBillDetails();

            //Call base PrintDocumentPaymentDetails();
            PrintDocumentPaymentDetails(string.Empty, _documentMaster.PaymentMethods[0].PaymentMethod.Designation, _documentMaster.Currency.Acronym); /* IN009055 */

            //Call base PrintNotes();
            //IN009279 Wont print notes on ticket mode 
            if (_documentMaster.Notes != null && !GeneralSettings.AppUseParkingTicketModule) PrintNotes(_documentMaster.Notes.ToString());


            //VERIFICAÇÃO XPO COMENTADO - LUCIANO
            if (_documentMaster.Customer.Country.ToUpper() == "PT" || _documentMaster.Customer.Country.ToUpper() == "AO")
            {
                var typeAnalyzer = _documentMaster.TypeAnalyzer;

                if (typeAnalyzer.IsInvoice() ||
                   typeAnalyzer.IsSimplifiedInvoice() ||
                   typeAnalyzer.IsInvoiceReceipt() ||
                   typeAnalyzer.IsConsignmentInvoice()) 
                {
                    PrintDocumentTypeFooterString("global_documentfinance_type_report_invoice_footer_at"); 
                }
                else
                {
                    PrintDocumentTypeFooterString("global_documentfinance_type_report_non_invoice_footer_at");
                }
                //ATCUD Documentos - Criação do QRCode e ATCUD IN016508
                //Print QRCode
                if (!string.IsNullOrEmpty(_documentMaster.Number))
                {
                    //PrintQRCode with buffer
                    //base.PrintQRCode(_documentMaster.ATDocQRCode);

                    //PrintQRCode with image
                    var qrCode = new QrCode(_documentMaster.Number, new Vector2Slim(256, 256), SKEncodedImageFormat.Png);
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


            //TEMP---- CRIPTOGRAFICA COMENTADA POR CAUSA DO HASH NOS DOCUMENTS -- LUCIANO
            //Get Hash4Chars from Hash
            //string hash4Chars = CryptographyUtils.GetDocumentHash4Chars(_documentMaster.Hash);
            //Call Base CertificationText 
            //PrintCertificationText(hash4Chars);

        }

        //Print Specific DocumentType Details
        public void PrintDocumentMasterDocumentType()
        {
            //Set Align Center
            _printer.SetAlignCenter();

            /* IN008024 */
            // string appOperationModeToken = LogicPOS.Settings.GeneralSettings.Settings["appOperationModeToken"].ToLower();

            //Reset Align 
            _printer.SetAlignLeft();
        }

        //Loop Details
        public void PrintDocumentDetails()
        {
            List<TicketColumn> columns = new List<TicketColumn>();

            columns.Add(new TicketColumn("VatRate", GeneralUtils.GetResourceByName("global_vat_rate") + "%", 6, TicketColumnsAlignment.Right, typeof(decimal), "{0:00.00}"));

            columns.Add(new TicketColumn("Quantity", GeneralUtils.GetResourceByName("global_quantity_acronym"), 8, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"));
            columns.Add(new TicketColumn("UnitMeasure", GeneralUtils.GetResourceByName("global_unit_measure_acronym"), 3, TicketColumnsAlignment.Right));

            if (_documentMaster.Customer.Country.ToUpper() == "PORTUGAL")
            {
                columns.Add(new TicketColumn("UnitPrice", GeneralUtils.GetResourceByName("global_short_price"), 11, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"));
            }
            else
            {
                columns.Add(new TicketColumn("Price", GeneralUtils.GetResourceByName("global_price"), 11, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"));
            }

            columns.Add(new TicketColumn("Discount", GeneralUtils.GetResourceByName("global_discount_acronym") + "%", 6, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"));
            //columns.Add(new TicketColumn("TotalNet", CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_totalnet_acronym, 9, TicketColumnsAlign.Right, typeof(decimal), "{0:0.00}"));
            columns.Add(new TicketColumn("TotalFinal", GeneralUtils.GetResourceByName("global_total_per_item"), 0, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"));//Dynamic
            /* IN009211 - end */

            //Prepare Table with Padding
            DataTable dataTable = TicketTable.InitDataTableFromTicketColumns(columns);
            TicketTable ticketTable = new TicketTable(dataTable, columns, _maxCharsPerLineNormal - _ticketTablePaddingLeftLength);
            string paddingLeftFormat = "  {0,-" + ticketTable.TableWidth + "}";//"  {0,-TableWidth}"
                                                                               //Print Table Headers
            ticketTable.Print(_printer, paddingLeftFormat);

            //Print Items
            foreach (var item in _documentMaster.Details)
            {

                //Recreate/Reset Table for Item Details Loop
                ticketTable = new TicketTable(dataTable, columns, _maxCharsPerLineNormal - _ticketTablePaddingLeftLength);
                PrintDocumentDetail(ticketTable, item, paddingLeftFormat);
            }

            //Line Feed
            _printer.LineFeed();
        }

        //Detail Row Block
        public void PrintDocumentDetail(
            TicketTable pTicketTable,
            DocumentDetail pDocumentDetail,
            string pPaddingLeftFormat)
        {
            string designation = (pDocumentDetail.Designation.Length <= _maxCharsPerLineNormalBold)
                ? pDocumentDetail.Designation
                : pDocumentDetail.Designation.Substring(0, _maxCharsPerLineNormalBold)
            ;
            //Print Item Designation : Bold
            _printer.WriteLine(designation, WriteLineTextMode.Bold);

            //Prepare ExemptionReason
            string exemptionReason = string.Empty;
            if (!string.IsNullOrEmpty(pDocumentDetail.VatExemptionReason))
            {
                //Replaced Code : Remove Cut Exception Reason......its better not to cut VatExemptionReason.....
                //exemptionReason = (pDocumentDetail.VatExemptionReasonDesignation.Length <= pTicketTable.TableWidth)
                //    ? pDocumentDetail.VatExemptionReasonDesignation
                //    : pDocumentDetail.VatExemptionReasonDesignation.Substring(0, pTicketTable.TableWidth)
                //;
                //Replace Code
                exemptionReason = pDocumentDetail.VatExemptionReason;
                //Always Format Data, its appens in first line only
                exemptionReason = string.Format(pPaddingLeftFormat, exemptionReason);
            }

            //Item Details
            DataRow dataRow = pTicketTable.NewRow();
            /* IN009211 block - begin 
             * Method "LogicPOS.Utility.DataConversionUtils.DecimalToString(pDocumentDetail.Vat)" is safe for "%" format
             */
            dataRow[0] = pDocumentDetail.Tax.Percentage;
            dataRow[1] = pDocumentDetail.Quantity;
            dataRow[2] = pDocumentDetail.Unit;
            //Layout talões PT - Preço Unitário em vez de Preço sem IVA [IN:016509]
            dataRow[3] = pDocumentDetail.Price;
            dataRow[4] = pDocumentDetail.Discount;
            dataRow[5] = pDocumentDetail.TotalNet;
            /* fix for item total before CustomerDiscount: (TotalGross - ItemDiscount) + ItemVAT */
            decimal amountItemDiscount = pDocumentDetail.TotalGross * pDocumentDetail.Discount / 100;
            decimal amountDueBeforeTax = (pDocumentDetail.TotalGross - amountItemDiscount);
            decimal totalItemTax = amountDueBeforeTax * pDocumentDetail.Tax.Percentage / 100;
            decimal amountDueAfterTax = amountDueBeforeTax + totalItemTax;
            dataRow[5] = amountDueAfterTax;
            /* IN009211 block - end */

            //Add DataRow to Table, Ready for Print
            pTicketTable.Rows.Add(dataRow);
            //Print Table Rows
            pTicketTable.Print(_printer, WriteLineTextMode.Normal, true, pPaddingLeftFormat);

            //VatExemptionReason
            _printer.WriteLine(exemptionReason, WriteLineTextMode.Small); /* IN009211 - WriteLine() already checks for empties and nulls */
            _printer.LineFeed();
        }

        //Totals, with TotalDelivery and TotalChange
        private void PrintMasterTotals()
        {
            DataRow dataRow = null;
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("Label", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Value", typeof(string)));

            //Add Row : Discount
            if (_documentMaster.Discount > 0.0m)
            {
                dataRow = dataTable.NewRow();
                dataRow[0] = string.Format("{0} (%)", GeneralUtils.GetResourceByName("global_documentfinance_discount_customer")); /* IN009211 */
                dataRow[1] = DataConversionUtils.DecimalToString(_documentMaster.Discount);
                dataTable.Rows.Add(dataRow);
            }
            //Add Row : TotalDiscount
            if (_documentMaster.TotalDiscount > 0.0m)
            {
                dataRow = dataTable.NewRow();
                dataRow[0] = GeneralUtils.GetResourceByName("global_documentfinance_total_discount"); /* IN009211 */
                dataRow[1] = DataConversionUtils.DecimalToString(_documentMaster.TotalDiscount);
                dataTable.Rows.Add(dataRow);
            }

            /* #TODO IN009214 */
            //Add Row : Discount PaymentCondition
            //WIP Always 0

            //Add Row : TotalNet
            dataRow = dataTable.NewRow();
            dataRow[0] = GeneralUtils.GetResourceByName("global_totalnet"); /* IN009211 */
            dataRow[1] = LogicPOS.Utility.DataConversionUtils.DecimalToString(_documentMaster.TotalGross * _documentMaster.ExchangeRate);
            dataRow[1] = DataConversionUtils.DecimalToString(_documentMaster.TotalNet);
            dataTable.Rows.Add(dataRow);
            //Add Row : TotalTax
            dataRow = dataTable.NewRow();
            dataRow[0] = GeneralUtils.GetResourceByName("global_documentfinance_totaltax"); /* IN009211 */
            dataRow[1] = DataConversionUtils.DecimalToString(_documentMaster.TotalTax);
            dataTable.Rows.Add(dataRow);
            //Add Row : TotalFinal
            dataRow = dataTable.NewRow();
            dataRow[0] = GeneralUtils.GetResourceByName("global_documentfinance_totalfinal");
            dataRow[1] = DataConversionUtils.DecimalToString(_documentMaster.TotalFinal );
            dataTable.Rows.Add(dataRow);

            //If Simplified Invoice, Payment Method MONEY and has Total Change, add it
            if ((_documentMaster.Type == "FS" || _documentMaster.Type == "FR")
                && _documentMaster.PaymentMethods[0].PaymentMethod.Token == "MONEY"
                && _documentMaster.TotalChange > 0
                )
            {
                //Blank Row, to separate from Main Totals
                dataRow = dataTable.NewRow();
                dataRow[0] = string.Empty;
                dataRow[1] = string.Empty;
                dataTable.Rows.Add(dataRow);
                //TotalDelivery
                dataRow = dataTable.NewRow();
                dataRow[0] = GeneralUtils.GetResourceByName("global_total_deliver");
                dataRow[1] = DataConversionUtils.DecimalToString(_documentMaster.TotalDelivery );
                dataTable.Rows.Add(dataRow);
                //TotalChange
                dataRow = dataTable.NewRow();
                dataRow[0] = GeneralUtils.GetResourceByName("global_total_change");
                dataRow[1] = DataConversionUtils.DecimalToString(_documentMaster.TotalChange );
                dataTable.Rows.Add(dataRow);
            }

            /* IN009055 - related to IN005976 for Mozambique deployment */
            if (CultureSettings.MozambiqueCountryId == Guid.Empty)
            {
                CurrenyDto defaultCurrencyForExchangeRate = XPOUtility.GetUsdCurrencyDto();

                dataRow = dataTable.NewRow();
                dataRow[0] = string.Format(GeneralUtils.GetResourceByName("global_printer_thermal_total_default_currency"), defaultCurrencyForExchangeRate.Acronym);
                dataRow[1] = DataConversionUtils.DecimalToString(_documentMaster.TotalFinal * defaultCurrencyForExchangeRate.ExchangeRate);/* TO DO : IN009055 - this causes total equals 0,00 when low product price */
                dataTable.Rows.Add(dataRow);
            }

            //Configure Ticket Column Properties
            List<TicketColumn> columns = new List<TicketColumn>
                {
                    new TicketColumn("Label", "", Convert.ToInt16(_maxCharsPerLineNormal / 2) - 2, TicketColumnsAlignment.Left),
                    new TicketColumn("Value", "", Convert.ToInt16(_maxCharsPerLineNormal / 2) - 2, TicketColumnsAlignment.Right)
                };

            //TicketTable(DataTable pDataTable, List<TicketColumn> pColumnsProperties, int pTableWidth)
            TicketTable ticketTable = new TicketTable(dataTable, columns, _printer.MaxCharsPerLineNormalBold);

            //Custom Print Loop, to Print all Table Rows, and Detect Rows to Print in DoubleHeight (TotalChange(4) and Total(7))
            List<string> table = ticketTable.GetTable();
            WriteLineTextMode rowTextMode;
            //Dynamic Print All, some Rows ommited/bold(Table Header, TotalDocument,TotalChange)
            for (int i = 1; i < table.Count; i++)
            {
                //Prepare TextMode Based on Row
                rowTextMode = (i == 5 || i == 8) ? WriteLineTextMode.DoubleHeightBold : WriteLineTextMode.Bold;
                //Print Row
                _printer.WriteLine(table[i], rowTextMode);
            }

            //Line Feed
            _printer.LineFeed();
        }


        private void PrintMasterTotalTax()
        {
            var TaxResume = _documentMaster.GetTaxResumes();
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

            foreach (var item in TaxResume)
            {
                dataRow = dataTable.NewRow();
                dataRow[0] = item.Designation;
                dataRow[1] = string.Format("{0}%", DataConversionUtils.DecimalToString(item.Rate));
                dataRow[2] = DataConversionUtils.DecimalToString(item.Base);
                dataRow[3] = DataConversionUtils.DecimalToString(item.Total);
                dataTable.Rows.Add(dataRow);
            }

            //Configure Ticket Column Properties
            List<TicketColumn> columns = new List<TicketColumn>
                {
                    new TicketColumn("Designation", GeneralUtils.GetResourceByName("global_designation"), 0, TicketColumnsAlignment.Left),
                    new TicketColumn("Tax", GeneralUtils.GetResourceByName("global_tax"), 8, TicketColumnsAlignment.Right),
                    new TicketColumn("TotalBase", GeneralUtils.GetResourceByName("global_total_tax_base"), 12, TicketColumnsAlignment.Right),
                    new TicketColumn("Total", GeneralUtils.GetResourceByName("global_documentfinance_totaltax_acronym"), 10, TicketColumnsAlignment.Right)
                };

            //TicketTable(DataTable pDataTable, List<TicketColumn> pColumnsProperties, int pTableWidth)
            TicketTable ticketTable = new TicketTable(dataTable, columns, _printer.MaxCharsPerLineNormal);
            //Print Table Buffer
            ticketTable.Print(_printer);

            //Line Feed
            _printer.LineFeed();
        }

        public void PrintDocumenWayBillDetails()
        {
            var typeAnalyzer = _documentMaster.TypeAnalyzer;

            if (typeAnalyzer.IsGuide())
            {
                //WayBill Local Load
                _printer.WriteLine(GeneralUtils.GetResourceByName("global_documentfinance_waybill_local_load"), WriteLineTextMode.Bold);
                _printer.WriteLine(string.Format("{0} {1}", _documentMaster.ShipFromAdress.AddressDetail, _documentMaster.ShipFromAdress.City));
                _printer.WriteLine(string.Format("{0} {1} [{2}]", _documentMaster.ShipFromAdress.PostalCode, _documentMaster.ShipFromAdress.Region, _documentMaster.ShipFromAdress.Country));
                _printer.WriteLine(GeneralUtils.GetResourceByName("global_ship_from_delivery_date_report"), _documentMaster.ShipFromAdress.DeliveryDate.ToString());
                _printer.WriteLine(GeneralUtils.GetResourceByName("global_ship_from_delivery_id_report"), _documentMaster.ShipFromAdress.DeliveryID);
                _printer.WriteLine(GeneralUtils.GetResourceByName("global_ship_from_warehouse_id_report"), _documentMaster.ShipFromAdress.WarehouseID);
                _printer.WriteLine(GeneralUtils.GetResourceByName("global_ship_from_location_id_report"), _documentMaster.ShipFromAdress.LocationID);

                //Separate with Blank Line
                _printer.LineFeed();

                //WayBill Local Download
                _printer.WriteLine(GeneralUtils.GetResourceByName("global_documentfinance_waybill_local_download"), WriteLineTextMode.Bold);
                _printer.WriteLine(string.Format("{0} {1}", _documentMaster.ShipToAdress.AddressDetail, _documentMaster.ShipToAdress.City));
                _printer.WriteLine(string.Format("{0} {1} [{2}]", _documentMaster.ShipToAdress.PostalCode, _documentMaster.ShipToAdress.Region, _documentMaster.ShipToAdress.Country));
                _printer.WriteLine(GeneralUtils.GetResourceByName("global_ship_to_delivery_date_report"), _documentMaster.ShipToAdress.DeliveryDate.ToString());
                _printer.WriteLine(GeneralUtils.GetResourceByName("global_ship_to_delivery_id_report"), _documentMaster.ShipToAdress.DeliveryID);
                _printer.WriteLine(GeneralUtils.GetResourceByName("global_ship_to_warehouse_id_report"), _documentMaster.ShipToAdress.WarehouseID);
                _printer.WriteLine(GeneralUtils.GetResourceByName("global_ship_to_location_id_report"), _documentMaster.ShipToAdress.LocationID);

                //Line Feed
                _printer.LineFeed();

                //ATDocCodeID
                if (!string.IsNullOrEmpty(_documentMaster.ATDocCodeID))
                {
                    //Set Align Center
                    _printer.SetAlignCenter();

                    //WayBill Local Load
                    _printer.WriteLine(GeneralUtils.GetResourceByName("global_at_atdoccodeid"), WriteLineTextMode.Bold);
                    _printer.WriteLine(_documentMaster.ATDocCodeID, WriteLineTextMode.DoubleHeight);

                    //Reset Align 
                    _printer.SetAlignLeft();

                    //Line Feed
                    _printer.LineFeed();
                }
            }
        }
    }
}
