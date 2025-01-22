using ESC_POS_USB_NET.Enums;
using LogicPOS.Api.Features.Company;
using LogicPOS.Api.Features.Documents.GetDocumentById;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Components.Users;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Document = LogicPOS.Api.Entities.Document;
using Printer = ESC_POS_USB_NET.Printer.Printer;

namespace LogicPOS.UI.Printing
{
    public class PosInvoicePrinter : ThermalPrinter
    {
        private readonly Document _document;
        private static readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();
        protected readonly CompanyInformations _companyInformations;
        public PosInvoicePrinter(Printer printer, Guid documentId) : base(printer)
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
        static string FormatDetails(string vat, string quantity, string measurementUnit, string price, string discount, string total)
        {
            // Formata a string com colunas alinhadas
            return String.Format("{0,-5} {2,-5} {2,-5} {2,-5} {2,5} {2,5}", vat, quantity, measurementUnit, price, discount, total);
        }
        static string FormatTax(string designation, string tax, string Base, string total)
        {
            // Formata a string com colunas alinhadas
            return String.Format("{0,-5} {1,-15} {1,-15} {2,-20}", designation, tax, Base, total);
        }
        protected void PrintCustomer(string name, string address, string zipCode, string city, string country, string fiscalNumber)
        {
            /* IN009055 block - begin */
            _printer.Append(string.Format("{0}: {1}", GeneralUtils.GetResourceByName("global_customer"), name));
            _printer.Append(string.Format("{0}: {1}", GeneralUtils.GetResourceByName("global_address"), address));

            string addressDetails = country;

            if (!string.IsNullOrEmpty(zipCode) && !string.IsNullOrEmpty(city))
            {
                addressDetails = string.Format("{0} {1} - {2}", zipCode, city, country);
            }
            else if (!string.IsNullOrEmpty(zipCode))
            {
                addressDetails = string.Format("{0} - {1}", zipCode, country);
            }
            else if (!string.IsNullOrEmpty(city))
            {
                addressDetails = string.Format("{0} - {1}", city, country);
            }
            _printer.Append(addressDetails); /* When FS, no details */
            _printer.Append(string.Format("{0}: {1}", GeneralUtils.GetResourceByName("global_fiscal_number"), fiscalNumber)); /* Do not print Fiscal Number when empty */
            /* IN009055  block - end */
            _printer.Separator(' ');
        }
        public void PrintFooter()
        {
            if (_companyInformations.TicketFinalLine1 != string.Empty || _companyInformations.TicketFinalLine1 != string.Empty)
            {
                //Align Center
                _printer.AlignCenter();

                if (_companyInformations.TicketFinalLine1 != string.Empty) _printer.Append(_companyInformations.TicketFinalLine1);
                if (_companyInformations.TicketFinalLine2 != string.Empty) _printer.Append(_companyInformations.TicketFinalLine2);

                //Line Feed
                _printer.Separator(' ');

                _printer.Separator(' ');
                _printer.NewLine();
                _printer.Append(string.Format("{0} - {1}", AuthenticationService.User.Name, TerminalService.Terminal.Designation));
                _printer.NewLine();

                //Printed On | Company|App|Version
                _printer.Append(string.Format("{1}: {2}{0}{3}: {4} {5}"
                    , Environment.NewLine
                    , GeneralUtils.GetResourceByName("global_printed_on_date")
                    , DateTime.Now.ToLocalTime()
                    , "LogicPulse"//_customVars["APP_COMPANY"]
                    , "LogicPOS"//_customVars["APP_NAME"]
                    , "vs1.010.1"//_customVars["APP_VERSION"]
                    ));

                //Reset to Left
                _printer.AlignLeft();

            }
        }
        public override void Print()
        {
            PrintHeader();
            _printer.AlignCenter();

            _printer.CondensedMode(PrinterModeState.On);
            _printer.DoubleWidth3();
            _printer.SetLineHeight(100);
            _printer.BoldMode("Technologies, Ltda");
            _printer.SetLineHeight(100);
            _printer.NewLine();
            _printer.Append(_document.Number);

            _printer.CondensedMode(PrinterModeState.Off);
            _printer.Append($"Original");
            _printer.Append(_document.Date);
            _printer.NewLine();
            _printer.SetLineHeight(20);
            _printer.AlignLeft();

            PrintCustomer(
                          _document.Customer.Name,
                          _document.Customer.Address,
                          _document.Customer.ZipCode,
                          _document.Customer.City,
                          _document.Customer.Country,
                          _document.Customer.FiscalNumber

                          );

            _printer.NewLine();
            _printer.AlignLeft();
            _printer.BoldMode(FormatDetails("IVA%", "Qnt", "Un", "Pr.Unit", "Desc%", "Total"));

            foreach (var item in _document.Details)
            {
                _printer.AlignLeft();
                _printer.Separator(' ');
                _printer.BoldMode($"{item.Article.Designation}");

                _printer.Append(FormatDetails(item.Tax.Percentage.ToString("F2"),
                                                item.Quantity.ToString("F2"),
                                                item.Article.MeasurementUnit.Acronym,
                                                item.Article.Price1.Price.ToString("F2"),
                                                item.Discount.ToString("F2"),
                                                item.TotalFinal.ToString("F2"))
                               );

                _printer.NewLine();
            }
            _printer.AlignLeft();
            _printer.NewLine();
            _printer.NewLine();
            _printer.BoldMode($"Total Liquido: {_document.TotalNet}");
            _printer.BoldMode($"Total Imposto: {_document.TotalTax}");
            _printer.BoldMode($"Total do Documento: {_document.TotalFinal}");
            _printer.NewLine();
            _printer.NewLine();

            _printer.Append(FormatTax("Designação", "Imposto", "Base Imp.", "Total Imp."));
            foreach (var item in _document.GetTaxResumes())
            {
                _printer.Append(FormatTax(item.Designation, item.Rate.ToString(), item.Base.ToString(), item.Total.ToString()));
                _printer.NewLine();
            }

            _printer.AlignCenter();
            _printer.Separator(' ');
            _printer.Append($"Método de Pagamento: {_document.PaymentMethods.FirstOrDefault().PaymentMethod.Designation}");
            _printer.Append($"Valores em: {_document.Currency.Acronym}");
            _printer.Separator(' ');

            _printer.Append(GeneralUtils.GetResourceByName("global_documentfinance_type_report_invoice_footer_at"));
            _printer.Separator(' ');
            _printer.SetLineHeight(100);
            _printer.QrCode(_document.Number, QrCodeSize.Size2);
            _printer.NormalLineHeight();
            _printer.Append($"Obrigado pela visita");
            _printer.Append($"Volte Sempre");
            _printer.NewLine();

            PrintFooter();
            _printer.FullPaperCut();
            _printer.PrintDocument();
            _printer.Clear();
        }

    }


}