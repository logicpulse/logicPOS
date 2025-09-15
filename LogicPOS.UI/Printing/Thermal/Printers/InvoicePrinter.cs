using ESC_POS_USB_NET.Enums;
using LogicPOS.Api.Features.Company;
using LogicPOS.Api.Features.Finance.Documents.Documents.GetPrintingModel;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Components.Users;
using LogicPOS.UI.Printing.Enums;
using LogicPOS.UI.Printing.Tickets;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Printer = ESC_POS_USB_NET.Printer.Printer;

namespace LogicPOS.UI.Printing
{
    public class InvoicePrinter : ThermalPrinter
    {
        private readonly InvoicePrintingData _data;

        public InvoicePrinter(Printer printer, InvoicePrintingData data) : base(printer)
        {
            _data = data;
        }

        public void PrintDocumentDetails()
        {
            List<TicketColumn> columns = new List<TicketColumn>();

            columns.Add(new TicketColumn("VatRate", GeneralUtils.GetResourceByName("global_vat_rate") + "%", 6, TicketColumnsAlignment.Right, typeof(decimal), "{0:00.00}"));
            columns.Add(new TicketColumn("Quantity", GeneralUtils.GetResourceByName("global_quantity_acronym"), 8, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"));
            columns.Add(new TicketColumn("UnitMeasure", GeneralUtils.GetResourceByName("global_unit_measure_acronym"), 3, TicketColumnsAlignment.Right));
            if (_data.Document.Customer.Country.ToUpper() == "PT")
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

            foreach (var item in _data.Document.Details)
            {
                ticketTable = new TicketTable(columns, 48 - 2);
                PrintDocumentDetail(ticketTable, item, paddingLeftFormat);
            }
            _printer.NewLine();
        }

        public void PrintDocumentDetail(TicketTable pTicketTable, Detail documentDetail, string pPaddingLeftFormat)
        {
            string designation = (documentDetail.Designation.Length <= 48) ? documentDetail.Designation : documentDetail.Designation.Substring(0, 48);

            _printer.Append(designation);
            string exemptionReason = string.Empty;
            if (!string.IsNullOrEmpty(documentDetail.VatExemptionReason))
            {
                exemptionReason = string.Format(pPaddingLeftFormat, documentDetail.VatExemptionReason);
            }
            DataRow dataRow = pTicketTable.NewRow();
            dataRow[0] = documentDetail.Tax;
            dataRow[1] = documentDetail.Quantity;
            dataRow[2] = documentDetail.Unit;
            dataRow[3] = documentDetail.UnitPrice;
            dataRow[4] = documentDetail.Discount;
            dataRow[5] = documentDetail.TotalFinal;
            pTicketTable.Rows.Add(dataRow);

            pTicketTable.Print(_printer, true, pPaddingLeftFormat);
            _printer.Append(exemptionReason);
        }

        private void PrintTotalTax()
        {
            var TaxResume = _data.Document.GetTaxResumes();
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
                dataRow[1] = $"{item.Rate:F2}%";
                dataRow[2] = $"{item.Base:F2}";
                dataRow[3] = $"{item.Total:F2}";
                ticketTable.Rows.Add(dataRow);
            }
            ticketTable.Print(_printer);
            _printer.NewLine();
        }

        protected void PrintDocumentPaymentDetails()
        {
            _printer.AlignCenter();
            if (!string.IsNullOrEmpty(_data.Document.PaymentCondition))
            {
                _printer.Append(GeneralUtils.GetResourceByName("global_payment_conditions") + ": " + _data.Document.PaymentCondition);
            }
            if (_data.Document.PaymentMethods != null)
            {
                foreach (var paymentMethod in _data.Document.PaymentMethods)
                {
                    _printer.Append(GeneralUtils.GetResourceByName("global_payment_method_field") + ": " + paymentMethod);
                }
            }
            _printer.Append(GeneralUtils.GetResourceByName("global_currency_field") + ": " + _data.Document.Currency);
            _printer.NewLine();
        }

        protected void PrintCustomer(Customer customer)
        {
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
            _printer.Append(addressDetails);
            _printer.Append(string.Format("{0}: {1}", GeneralUtils.GetResourceByName("global_fiscal_number"), customer.FiscalNumber));

            _printer.AlignLeft();
            _printer.Separator(' ');
        }

        public void PrintFooter()
        {
            if (_data.CompanyInformations.TicketFinalLine1 != string.Empty || _data.CompanyInformations.TicketFinalLine1 != string.Empty)
            {
                _printer.AlignCenter();
                if (_data.CompanyInformations.TicketFinalLine1 != string.Empty) _printer.Append(_data.CompanyInformations.TicketFinalLine1);
                if (_data.CompanyInformations.TicketFinalLine2 != string.Empty) _printer.Append(_data.CompanyInformations.TicketFinalLine2);
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
            var typeAnalyzer = _data.Document.TypeAnalyzer;
            var documentType = "global_documentfinance_type_title_fr";
            var documentTypeSuffix = _data.Document.Number.Substring(0, 2).ToLower() == "cm" ? "dc" : _data.Document.Number.Substring(0, 2).ToLower();
            documentType = documentType.Substring(0, documentType.Length - 2) + documentTypeSuffix;
            _printer.AlignCenter();
            PrintHeader();
            //_printer.NewLines(2);
            _printer.AlignLeft();
            if (string.IsNullOrEmpty(_data.CompanyInformations.Address) == false) _printer.Append($"{_data.CompanyInformations.Address} ");
            if (string.IsNullOrEmpty(_data.CompanyInformations.PostalCode) == false) _printer.Append($"{_data.CompanyInformations.PostalCode} {_data.CompanyInformations.City} - {_data.CompanyInformations.CountryCode2} ");
            if (string.IsNullOrEmpty(_data.CompanyInformations.PostalCode)) _printer.Append($"0000-000 {_data.CompanyInformations.City} - {_data.CompanyInformations.CountryCode2} ");
            if (string.IsNullOrEmpty(_data.CompanyInformations.Phone) == false) _printer.Append($"{GeneralUtils.GetResourceByName("global_phone")}: {_data.CompanyInformations.Phone} ({GeneralUtils.GetResourceByName("report_phonenumber_label")})");
            if (string.IsNullOrEmpty(_data.CompanyInformations.MobilePhone) == false) _printer.Append($"{GeneralUtils.GetResourceByName("global_mobile_phone")}: {_data.CompanyInformations.MobilePhone} ({GeneralUtils.GetResourceByName("report_mobilephonenumber_label")})");
            if (string.IsNullOrEmpty(_data.CompanyInformations.Email) == false) _printer.Append($"{GeneralUtils.GetResourceByName("global_user_email")}: {_data.CompanyInformations.Email} ");
            _printer.Append($"{GeneralUtils.GetResourceByName("prefparam_company_fiscalnumber")}: {_data.CompanyInformations.FiscalNumber} ");
            _printer.AlignCenter();
            _printer.SetLineHeight(80);
            _printer.Separator(' ');
            _printer.AlignCenter();
            _printer.DoubleWidth2();
            _printer.ExpandedMode(PrinterModeState.On);
            _printer.BoldMode(GeneralUtils.GetResourceByName(documentType));
            _printer.Separator(' ');
            _printer.Append(_data.Document.Number);
            _printer.Append($"Original");
            _printer.Append(_data.Document.Date.ToShortDateString());
            _printer.ExpandedMode(PrinterModeState.Off);
            _printer.Separator(' ');
            _printer.NormalWidth();
            _printer.SetLineHeight(20);
            _printer.AlignLeft();

            PrintCustomer(_data.Document.Customer);
            _printer.Separator(' ');
            PrintDocumentDetails();

            _printer.AlignLeft();
            _printer.BoldMode($"{GeneralUtils.GetResourceByName("global_totalnet")}: {_data.Document.TotalNet:F2}");
            _printer.NewLine();
            _printer.BoldMode($"{GeneralUtils.GetResourceByName("global_documentfinance_totaltax")}: {_data.Document.TotalTax:F2}");
            _printer.NewLine();
            _printer.BoldMode($"{GeneralUtils.GetResourceByName("global_documentfinance_totalfinal")}: {_data.Document.TotalFinal:F2}");
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

            if (_data.CompanyInformations.CountryCode2.ToUpper() == "PT" && !string.IsNullOrEmpty(_data.Document.ATQRCode))
            {
                _printer.QrCode(_data.Document.ATQRCode, QrCodeSize.Size2);
                _printer.NormalLineHeight();
                _printer.NewLine();
            }
            else
            {
                _printer.QrCode(_data.Document.Number, QrCodeSize.Size2);
                _printer.NormalLineHeight();
                _printer.NewLine();
            }

            PrintFooter();
            if (_data.Document.Type.ToUpper()=="FR" || _data.Document.Type.ToUpper() == "FS" || _data.Document.Type.ToUpper() == "VD")
            {
                AuthenticationService.HardwareOpenDrawer();
            }
            _printer.FullPaperCut();
            _printer.PrintDocument();
            _printer.Clear();

        }

    }

    public struct InvoicePrintingData
    {
        public DocumentPrintingModel Document { get; set; }
        public CompanyInformations CompanyInformations { get; set; }
    }
}