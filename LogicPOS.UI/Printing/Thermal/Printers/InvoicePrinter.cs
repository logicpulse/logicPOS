using ESC_POS_USB_NET.Enums;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Company;
using LogicPOS.Api.Features.Documents;
using LogicPOS.Api.Features.Documents.GetDocumentById;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Components.Users;
using LogicPOS.UI.Printing.Enums;
using LogicPOS.UI.Printing.Tickets;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using Document = LogicPOS.Api.Entities.Document;
using Printer = ESC_POS_USB_NET.Printer.Printer;

namespace LogicPOS.UI.Printing
{
    public class InvoicePrinter : ThermalPrinter
    {
        private readonly Document _document;
        protected readonly CompanyInformations _companyInformations;
        public InvoicePrinter(Printer printer, Guid documentId) : base(printer)
        {
            _companyInformations = GetCompanyInformations();
            var result = _mediator.Send(new GetDocumentByIdQuery(documentId)).Result;
            
            if (result.IsError)
            {
                CustomAlerts.Error()
                            .WithMessage(result.FirstError.Description)
                            .ShowAlert();
            }
            _document = result.Value;
        }
        public void PrintDocumentDetails()
        {
            List<TicketColumn> columns = new List<TicketColumn>();

            columns.Add(new TicketColumn("VatRate", GeneralUtils.GetResourceByName("global_vat_rate") + "%", 6, TicketColumnsAlignment.Right, typeof(decimal), "{0:00.00}"));
            columns.Add(new TicketColumn("Quantity", GeneralUtils.GetResourceByName("global_quantity_acronym"), 8, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"));
            columns.Add(new TicketColumn("UnitMeasure", GeneralUtils.GetResourceByName("global_unit_measure_acronym"), 3, TicketColumnsAlignment.Right));
            if (_document.Customer.Country.ToUpper() == "PORTUGAL")
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

            TicketTable ticketTable = new TicketTable(columns, 48);
            string paddingLeftFormat = "  {0,-" + ticketTable.TableWidth + "}";//"  {0,-TableWidth}"

            //Print Table Headers
            ticketTable.Print(_printer);

            foreach (var item in _document.Details)
            {
                ticketTable = new TicketTable(columns, 48 - 2);
                PrintDocumentDetail(ticketTable, item, paddingLeftFormat);
            }
            _printer.NewLine();
        }
       
        public void PrintDocumentDetail(TicketTable pTicketTable, DocumentDetail documentDetail, string pPaddingLeftFormat)
        {
            string designation = (documentDetail.Designation.Length <= 48) ? documentDetail.Designation : documentDetail.Designation.Substring(0, 48);

            _printer.Append(designation);
            string exemptionReason = string.Empty;
            if (!string.IsNullOrEmpty(documentDetail.VatExemptionReason))
            {
                exemptionReason = string.Format(pPaddingLeftFormat, documentDetail.VatExemptionReason);
            }
            DataRow dataRow = pTicketTable.NewRow();
            dataRow[0] = documentDetail.Tax.Percentage;
            dataRow[1] = documentDetail.Quantity;
            dataRow[2] = documentDetail.Unit;
            dataRow[3] = documentDetail.Price;
            dataRow[4] = documentDetail.Discount;
            dataRow[5] = documentDetail.TotalFinal;
            pTicketTable.Rows.Add(dataRow);

            pTicketTable.Print(_printer, true, pPaddingLeftFormat);
            _printer.Append(exemptionReason);
        }
       
        private void PrintTotalTax()
        {
            var TaxResume = _document.GetTaxResumes();
            List<TicketColumn> columns = new List<TicketColumn>();

            columns.Add(new TicketColumn("Designation", GeneralUtils.GetResourceByName("global_designation"), 0, TicketColumnsAlignment.Left));
            columns.Add(new TicketColumn("Tax", GeneralUtils.GetResourceByName("global_tax"), 8, TicketColumnsAlignment.Right));
            columns.Add(new TicketColumn("TotalBase", GeneralUtils.GetResourceByName("global_total_tax_base"), 12, TicketColumnsAlignment.Right));
            columns.Add(new TicketColumn("Total", GeneralUtils.GetResourceByName("global_documentfinance_totaltax_acronym"), 10, TicketColumnsAlignment.Right));

            TicketTable ticketTable = new TicketTable(columns, 48);

            foreach (var item in TaxResume)
            {
                var dataRow = ticketTable.NewRow();
                dataRow[0] = item.Designation;
                dataRow[1] = $"{item.Rate.ToString("F2")}%";
                dataRow[2] = $"{DecimalToMoneyString(item.Base)}";
                dataRow[3] = $"{DecimalToMoneyString(item.Total)}";
                ticketTable.Rows.Add(dataRow);
            }
            ticketTable.Print(_printer);
            _printer.NewLine();
        }
       
        protected void PrintDocumentPaymentDetails()
        {
            _printer.AlignCenter();
            if (!string.IsNullOrEmpty(_document.PaymentCondition?.Designation))
            {
                _printer.Append(GeneralUtils.GetResourceByName("global_payment_conditions") + ": " + _document.PaymentCondition.Designation);
            }
            if (_document.PaymentMethods != null)
            {
                foreach (var payment in _document.PaymentMethods)
                {
                    _printer.Append(GeneralUtils.GetResourceByName("global_payment_method_field") + ": " + payment.PaymentMethod.Designation);
                }
            }
            _printer.Append(GeneralUtils.GetResourceByName("global_currency_field") + ": " + _document.Currency.Acronym); /* IN009055 */
            _printer.NewLine();
        }
       
        protected void PrintCustomer(DocumentCustomer customer)
        {
            /* IN009055 block - begin */
            _printer.Append(string.Format("{0}: {1}", GeneralUtils.GetResourceByName("global_customer"), customer.Name));
            _printer.Append(string.Format("{0}: {1}", GeneralUtils.GetResourceByName("global_address"), customer.Address));

            string addressDetails = customer.Country;

            if (!string.IsNullOrEmpty(customer.ZipCode) && !string.IsNullOrEmpty(customer.City))
            {
                addressDetails = string.Format("{0} {1} - {2}", customer.ZipCode, customer.City, customer.Country);
            }
            else if (!string.IsNullOrEmpty(customer.ZipCode))
            {
                addressDetails = string.Format("{0} - {1}", customer.ZipCode, customer.Country);
            }
            else if (!string.IsNullOrEmpty(customer.City))
            {
                addressDetails = string.Format("{0} - {1}", customer.City, customer.Country);
            }
            _printer.Append(addressDetails); /* When FS, no details */
            _printer.Append(string.Format("{0}: {1}", GeneralUtils.GetResourceByName("global_fiscal_number"), customer.FiscalNumber)); /* Do not print Fiscal Number when empty */
            /* IN009055  block - end */
            _printer.AlignLeft();
            _printer.Separator(' ');
        }
       
        public void PrintFooter()
        {
            if (_companyInformations.TicketFinalLine1 != string.Empty || _companyInformations.TicketFinalLine1 != string.Empty)
            {
                _printer.AlignCenter();
                if (_companyInformations.TicketFinalLine1 != string.Empty) _printer.Append(_companyInformations.TicketFinalLine1);
                if (_companyInformations.TicketFinalLine2 != string.Empty) _printer.Append(_companyInformations.TicketFinalLine2);
                _printer.Separator(' ');
                _printer.Separator(' ');
                _printer.NewLine();
                _printer.Append(string.Format("{0} - {1}", AuthenticationService.User.Name, TerminalService.Terminal.Designation));
                _printer.NewLine();
                _printer.Append(string.Format("{1}: {2}{0}{3}: {4} {5}"
                                    , Environment.NewLine
                                    , GeneralUtils.GetResourceByName("global_printed_on_date")
                                    , DateTime.Now.ToLocalTime()
                                    , "LogicPulse"//"APP_COMPANY"
                                    , "LogicPOS"//"APP_NAME"
                                    , "vs1.000.0"//"APP_VERSION"
                                    ));
                _printer.AlignLeft();
            }
        }
        
        public override void Print()
        {
            var typeAnalyzer = _document.TypeAnalyzer;
            var documentType = "global_documentfinance_type_title_fr";
            documentType = documentType.Substring(0, documentType.Length - 2) + _document.Number.Substring(0, 2).ToLower();
            _printer.AlignCenter();
            PrintHeader();
            _printer.SetLineHeight(80);
            _printer.Separator(' ');
            _printer.AlignCenter();
            _printer.DoubleWidth2();
            _printer.ExpandedMode(PrinterModeState.On);
            _printer.BoldMode(GeneralUtils.GetResourceByName(documentType));
            _printer.Separator(' ');
            _printer.Append(_document.Number);
            _printer.Append($"Original");
            _printer.Append(_document.Date);
            _printer.ExpandedMode(PrinterModeState.Off);
            _printer.Separator(' ');
            _printer.NormalWidth();
            _printer.SetLineHeight(20);
            _printer.AlignLeft();

            PrintCustomer(_document.Customer);
            _printer.Separator(' ');
            PrintDocumentDetails();

            _printer.AlignLeft();
            _printer.BoldMode($"{GeneralUtils.GetResourceByName("global_totalnet")}: {DecimalToMoneyString(_document.TotalNet)}");
            _printer.NewLine(); 
            _printer.BoldMode($"{GeneralUtils.GetResourceByName("global_documentfinance_totaltax")}: {DecimalToMoneyString(_document.TotalTax)}");
            _printer.NewLine();
            _printer.BoldMode($"{GeneralUtils.GetResourceByName("global_documentfinance_totalfinal")}: {DecimalToMoneyString(_document.TotalFinal)}");
            _printer.Separator(' ');
            _printer.NewLine();
            PrintTotalTax();

            _printer.AlignCenter();
            _printer.Separator(' ');
            PrintDocumentPaymentDetails();
            _printer.Separator(' ');

            if (typeAnalyzer.IsInvoice() || typeAnalyzer.IsSimplifiedInvoice() || typeAnalyzer.IsInvoiceReceipt() || typeAnalyzer.IsConsignmentInvoice())
            {
                _printer.Append(GeneralUtils.GetResourceByName("global_documentfinance_type_report_invoice_footer_at"));
            }
            else
            {
                _printer.Append(GeneralUtils.GetResourceByName("global_documentfinance_type_report_non_invoice_footer_at"));
            }
            _printer.Separator(' ');
            _printer.SetLineHeight(100);

            if(_companyInformations.CountryCode2.ToUpper()=="PT" && !string.IsNullOrEmpty(_document.ATQRCode))
            {
                _printer.QrCode(_document.ATQRCode, QrCodeSize.Size2);
                _printer.NormalLineHeight();
                _printer.NewLine();
            }
            else
            {
                _printer.QrCode(_document.Number, QrCodeSize.Size2);
                _printer.NormalLineHeight();
                _printer.NewLine();
            }

            PrintFooter();
            _printer.FullPaperCut();
            _printer.PrintDocument();
            _printer.Clear();
        }

    }
}