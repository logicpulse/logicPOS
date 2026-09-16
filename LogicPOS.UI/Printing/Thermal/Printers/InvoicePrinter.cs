using ESC_POS_USB_NET.Enums;
using LogicPOS.Api.Features.Company;
using LogicPOS.Api.Features.Finance.Documents.Documents.Prints.GetPrintingModel;
using LogicPOS.Globalization;
using LogicPOS.UI.Application.Services;
using LogicPOS.UI.Components.Users;
using LogicPOS.UI.Printing.Enums;
using LogicPOS.UI.Printing.Tickets;
using LogicPOS.UI.Services;
using QrCodes;
using QrCodes.Renderers;
using QrCodes.Renderers.Abstractions;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using Printer = ESC_POS_USB_NET.Printer.Printer;

namespace LogicPOS.UI.Printing
{
    /// <summary>
    /// Thermal finance document template (legacy ThermalPrinterFinanceDocumentMaster / BaseFinanceTemplate).
    /// </summary>
    public class InvoicePrinter : ThermalPrinter
    {
        private readonly InvoicePrintingData _data;
        private readonly int _ticketTablePaddingLeftLength = 2;
        private string _copyName = string.Empty;

        public InvoicePrinter(Printer printer, InvoicePrintingData data) : base(printer)
        {
            _data = data;
        }

        public override void Print()
        {
            if (_data.OpenDrawer)
            {
                AuthenticationService.HardwareOpenDrawer();
            }

            ResetPrintModes();
            PrintCompanyHeader(isOrder: false);
            PrintExtendedHeader();
            ResolveCopyName();
            PrintContent();
            PrintFooterExtended();
            PrintStandardFooter();
            _printer.FullPaperCut();
            ThermalPrinterTarget.Commit(_printer);
            _printer.Clear();
        }

        private void ResolveCopyName()
        {
            if (_data.IsSecondCopy)
            {
                var original = LocalizedString.Instance["global_print_copy_title1"];
                _copyName = $"{original}/{LocalizedString.Instance["global_print_second_print"]}";
                return;
            }

            var copyNumber = _data.CopyNumber > 0 ? _data.CopyNumber : 1;
            _copyName = LocalizedString.Instance[$"global_print_copy_title{copyNumber}"];
            if (!string.IsNullOrWhiteSpace(_data.Reason))
            {
                _copyName = $"{_copyName} {_data.Reason}";
            }
        }

        public void PrintContent()
        {
            var documentTypeKey = ResolveDocumentTypeResourceKey();
            PrintDocumentMaster(
                ToThermalText(LocalizedString.Instance[documentTypeKey]),
                _data.Document.Number,
                _data.Document.Date.ToShortDateString());

            PrintDocumentMasterDocumentType();
            PrintCustomer(_data.Document.Customer);
            PrintDocumentDetails();
            PrintMasterTotals();
            PrintMasterTotalTax();
            PrintDocumentPaymentDetails();
            PrintDocumentTypeFooterString();
            PrintAtcudAndQrCode();
        }

        private string ResolveDocumentTypeResourceKey()
        {
            var documentType = "global_documentfinance_type_title_fr";
            var suffix = _data.Document.Number.Substring(0, 2).ToLower() == "cm"
                ? "dc"
                : _data.Document.Number.Substring(0, 2).ToLower();
            return documentType.Substring(0, documentType.Length - 2) + suffix;
        }

        private void PrintExtendedHeader()
        {
            var company = _data.CompanyInformations;
            ResetPrintModes();
            SetFontSmall();

            if (!string.IsNullOrEmpty(company.Address))
            {
                WriteLineSmall(company.Address);
            }

            var cityLine = string.IsNullOrEmpty(company.PostalCode)
                ? $"0000-000 {company.City} - {company.CountryCode2}"
                : $"{company.PostalCode} {company.City} - {company.CountryCode2}";
            WriteLineSmall(cityLine);

            if (!string.IsNullOrEmpty(company.Phone))
            {
                WriteLineSmall($"{LocalizedString.Instance["prefparam_company_telephone"]}: {company.Phone} ({LocalizedString.Instance["report_phonenumber_label"]})");
            }
            if (!string.IsNullOrEmpty(company.MobilePhone))
            {
                WriteLineSmall($"{LocalizedString.Instance["global_mobile_phone"]}: {company.MobilePhone} ({LocalizedString.Instance["report_mobilephonenumber_label"]})");
            }
            if (!string.IsNullOrEmpty(company.Email))
            {
                WriteLineSmall($"{LocalizedString.Instance["global_email"]}: {company.Email}");
            }
            if (!string.IsNullOrEmpty(company.Website))
            {
                WriteLineSmall(company.Website);
            }

            WriteLineSmall($"{LocalizedString.Instance["prefparam_company_fiscalnumber"]}: {company.FiscalNumber}");
            SetFontNormal();
        }

        private void PrintDocumentMaster(string documentTypeTitle, string documentNumber, string documentDate)
        {
            PrintTitles(documentTypeTitle, documentNumber);
            _printer.AlignCenter();
            WriteLine(_copyName);
            WriteLine(documentDate);
            LineFeed();
            ResetPrintModes();
        }

        private void PrintDocumentMasterDocumentType()
        {
            if (string.IsNullOrEmpty(_data.Table))
            {
                return;
            }

            _printer.AlignCenter();
            WriteLine($"Mesa: {_data.Table} / {_data.Place}");
            LineFeed();
            ResetPrintModes();
        }

        private void PrintCustomer(Customer customer)
        {
            WriteLabeledLine(LocalizedString.Instance["global_customer"], customer.Name, skipWhenEmpty: false);
            WriteLabeledLine(LocalizedString.Instance["global_address"], customer.Address, skipWhenEmpty: false);

            string addressDetails = customer.Country;
            if (!string.IsNullOrEmpty(customer.ZipCode) && !string.IsNullOrEmpty(customer.City))
            {
                addressDetails = $"{customer.ZipCode} {customer.City} - {customer.Country}";
            }
            else if (!string.IsNullOrEmpty(customer.ZipCode))
            {
                addressDetails = $"{customer.ZipCode} - {customer.Country}";
            }
            else if (!string.IsNullOrEmpty(customer.City))
            {
                addressDetails = $"{customer.City} - {customer.Country}";
            }

            WriteLine(addressDetails);
            WriteLabeledLine(LocalizedString.Instance["global_fiscal_number"], customer.FiscalNumber);
            LineFeed();
        }

        private void PrintDocumentDetails()
        {
            var columns = CreateDetailColumns();
            var tableWidth = Math.Max(16, MaxCharsPerLineNormal - _ticketTablePaddingLeftLength);
            var paddingLeftFormat = new string(' ', _ticketTablePaddingLeftLength) + "{0,-" + tableWidth + "}";

            var headerTable = new TicketTable(columns, tableWidth);
            headerTable.Print(_printer, false, paddingLeftFormat);

            foreach (var item in _data.Document.Details)
            {
                var rowTable = new TicketTable(columns, tableWidth);
                PrintDocumentDetail(rowTable, item, paddingLeftFormat);
            }

            LineFeed();
        }

        private List<TicketColumn> CreateDetailColumns()
        {
            GetDetailColumnWidths(out var vat, out var qty, out var unit, out var price, out var discount);

            return new List<TicketColumn>
            {
                new TicketColumn("VatRate", ToThermalText(LocalizedString.Instance["global_vat_rate"] + "%"), vat, TicketColumnsAlignment.Right, typeof(decimal), "{0:00.00}"),
                new TicketColumn("Quantity", ToThermalText(LocalizedString.Instance["global_quantity_acronym"]), qty, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"),
                new TicketColumn("UnitMeasure", ToThermalText(LocalizedString.Instance["global_unit_measure_acronym"]), unit, TicketColumnsAlignment.Right),
                new TicketColumn("Price", ToThermalText(LocalizedString.Instance["global_price"]), price, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"),
                new TicketColumn("Discount", ToThermalText(LocalizedString.Instance["global_discount_acronym"] + "%"), discount, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}"),
                new TicketColumn("TotalFinal", ToThermalText(LocalizedString.Instance["global_total_per_item"]), 0, TicketColumnsAlignment.Right, typeof(decimal), "{0:0.00}")
            };
        }

        private void GetDetailColumnWidths(out int vat, out int qty, out int unit, out int price, out int discount)
        {
            var width = Math.Max(16, MaxCharsPerLineNormal - _ticketTablePaddingLeftLength);
            // Leave room for TotalFinal (dynamic column).
            var budget = Math.Max(10, width - Math.Max(4, width / 5));

            vat = 6; qty = 8; unit = 3; price = 11; discount = 6;
            if (vat + qty + unit + price + discount <= budget)
            {
                return;
            }

            vat = 5; qty = 6; unit = 2; price = 8; discount = 5;
            if (vat + qty + unit + price + discount <= budget)
            {
                return;
            }

            vat = 4; qty = 5; unit = 2; price = 7; discount = 4;
            if (vat + qty + unit + price + discount <= budget)
            {
                return;
            }

            vat = 3; qty = 4; unit = 2; price = 5; discount = 3;
        }

        private void PrintDocumentDetail(TicketTable ticketTable, Detail documentDetail, string paddingLeftFormat)
        {
            var maxLen = MaxCharsPerLineNormalBold;
            var designation = documentDetail.Designation.Length <= maxLen
                ? documentDetail.Designation
                : documentDetail.Designation.Substring(0, maxLen);

            WriteLineBold(designation);

            var dataRow = ticketTable.NewRow();
            dataRow[0] = documentDetail.Tax;
            dataRow[1] = documentDetail.Quantity;
            dataRow[2] = documentDetail.Unit;
            dataRow[3] = documentDetail.UnitPrice;
            dataRow[4] = documentDetail.Discount;
            dataRow[5] = documentDetail.TotalFinal;
            ticketTable.Rows.Add(dataRow);
            ticketTable.Print(_printer, true, paddingLeftFormat);

            if (!string.IsNullOrEmpty(documentDetail.VatExemptionReason))
            {
                WriteLineSmall(string.Format(paddingLeftFormat, documentDetail.VatExemptionReason));
            }
        }

        private void PrintMasterTotals()
        {
            // Same width as the rest of the document so amounts share the right margin.
            var width = MaxCharsPerLineNormal;

            var rows = new[]
            {
                (Label: ToThermalText(LocalizedString.Instance["global_totalnet"]),
                    Value: _data.Document.TotalNet.ToString("F2")),
                (Label: ToThermalText(LocalizedString.Instance["global_documentfinance_totaltax"]),
                    Value: _data.Document.TotalTax.ToString("F2")),
                (Label: ToThermalText(LocalizedString.Instance["global_documentfinance_totalfinal"]),
                    Value: _data.Document.TotalFinal.ToString("F2"))
            };

            foreach (var item in rows)
            {
                var value = item.Value ?? string.Empty;
                var label = item.Label ?? string.Empty;
                var valueWidth = value.Length;
                var labelWidth = Math.Max(1, width - valueWidth);

                if (label.Length > labelWidth)
                {
                    label = label.Substring(0, labelWidth);
                }

                // Label left, value flush to the right edge — no inter-column gap.
                WriteLineBold(label.PadRight(labelWidth) + value);
            }

            LineFeed();
        }

        private void PrintMasterTotalTax()
        {
            var narrow = MaxCharsPerLineNormal <= 32;
            var tax = narrow ? 5 : 8;
            var totalBase = narrow ? 8 : 12;
            var total = narrow ? 7 : 10;
            if (tax + totalBase + total >= MaxCharsPerLineNormal)
            {
                tax = 4; totalBase = 6; total = 5;
            }

            var columns = new List<TicketColumn>
            {
                new TicketColumn("Designation", LocalizedString.Instance["global_designation"], 0, TicketColumnsAlignment.Left),
                new TicketColumn("Tax", LocalizedString.Instance["global_tax"], tax, TicketColumnsAlignment.Right),
                new TicketColumn("TotalBase", LocalizedString.Instance["global_total_tax_base"], totalBase, TicketColumnsAlignment.Right),
                new TicketColumn("Total", LocalizedString.Instance["global_documentfinance_totaltax_acronym"], total, TicketColumnsAlignment.Right)
            };

            var ticketTable = new TicketTable(columns, MaxCharsPerLineNormal);
            foreach (var item in _data.Document.GetTaxResumes())
            {
                var dataRow = ticketTable.NewRow();
                dataRow[0] = item.Designation;
                dataRow[1] = $"{item.Rate:F2}%";
                dataRow[2] = $"{item.Base:F2}";
                dataRow[3] = $"{item.Total:F2}";
                ticketTable.Rows.Add(dataRow);
            }

            ticketTable.Print(_printer);
            LineFeed();
        }

        private void PrintDocumentPaymentDetails()
        {
            _printer.AlignCenter();

            void WritePaymentLine(string label, string value)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return;
                }

                var cleanLabel = ToThermalText(label ?? string.Empty).Trim().TrimEnd(':').Trim();
                WriteLine($"{cleanLabel}: {ToThermalText(value)}");
            }

            WritePaymentLine(LocalizedString.Instance["global_payment_conditions"], _data.Document.PaymentCondition);
            if (_data.Document.PaymentMethods != null)
            {
                foreach (var paymentMethod in _data.Document.PaymentMethods)
                {
                    WritePaymentLine(LocalizedString.Instance["global_payment_method_field"], paymentMethod);
                }
            }
            WritePaymentLine(LocalizedString.Instance["global_currency_field"], _data.Document.Currency);
            LineFeed();
        }

        private void PrintDocumentTypeFooterString()
        {
            var typeAnalyzer = _data.Document.TypeAnalyzer;
            var resource = typeAnalyzer.IsInvoice()
                || typeAnalyzer.IsSimplifiedInvoice()
                || typeAnalyzer.IsInvoiceReceipt()
                || typeAnalyzer.IsConsignmentInvoice()
                ? "global_documentfinance_type_report_invoice_footer_at"
                : "global_documentfinance_type_report_non_invoice_footer_at";

            _printer.AlignCenter();
            WriteLine(LocalizedString.Instance[resource]);
            LineFeed();
        }

        private void PrintAtcudAndQrCode()
        {
            if (!PreferenceParametersService.PrintQrCode)
            {
                return;
            }

            if (_data.CompanyInformations.IsPortugal)
            {
                LineFeed();
                _printer.AlignCenter();
                WriteLineSmall($"ATCUD: {_data.Document.ATCUD}");
                SetFontNormal();
            }

            var qrContent = !string.IsNullOrEmpty(_data.Document.ATQRCode)
                ? _data.Document.ATQRCode
                : _data.Document.Number;

            PrintThermalQr(qrContent);
        }

        private void PrintFooterExtended()
        {
            var line1 = _data.CompanyInformations.TicketFinalLine1;
            var line2 = _data.CompanyInformations.TicketFinalLine2;
            if (string.IsNullOrEmpty(line1) && string.IsNullOrEmpty(line2))
            {
                return;
            }

            _printer.AlignCenter();
            if (!string.IsNullOrEmpty(line1))
            {
                WriteLine(line1);
            }
            if (!string.IsNullOrEmpty(line2))
            {
                WriteLine(line2);
            }
            LineFeed();
            ResetPrintModes();
        }

        private void PrintThermalQr(string qrContent)
        {
            // GS v 0 ignores AlignCenter. Emulate the same axis as "Obrigado...":
            // a paper-wide canvas (58mm=384 / 80mm=576) with the QR in the geometric centre,
            // printed from the left margin.
            ResetPrintModes();
            _printer.AlignLeft();
            _printer.Append(new byte[] { 0x1B, 0x24, 0x00, 0x00 }); // ESC $ x=0
            _printer.Append(new byte[] { 0x1D, 0x4C, 0x00, 0x00 }); // GS L left margin = 0

            using (var qrImage = CreateThermalQrBitmap(qrContent))
            {
                if (qrImage == null)
                {
                    return;
                }

                ThermalRasterImage.Print(_printer, qrImage, Layout.ImageDots);
            }

            ResetPrintModes();
            _printer.NewLine();
        }

        private Bitmap CreateThermalQrBitmap(string text)
        {
            var paperDots = Layout.ImageDots;
            var qrSize = Math.Min(paperDots * 40 / 100, paperDots - 32);
            qrSize = Math.Max(160, qrSize);
            qrSize = Math.Max(8, (qrSize + 7) / 8 * 8);

            var qrCode = QrCodeGenerator.Generate(
                plainText: text,
                eccLevel: ErrorCorrectionLevel.Quartile,
                forceUtf8: true,
                utf8Bom: false,
                eciMode: ExtendedChannelInterpolationMode.Utf8);

            var settings = new RendererSettings
            {
                PixelsPerModule = 4,
                DrawQuietZones = true,
                FileFormat = FileFormat.Png,
                PixelSizeFactor = 0
            };

            var qrBytes = new SkiaSharpRenderer().RenderToBytes(qrCode, settings);
            using (var skBitmap = SKBitmap.Decode(qrBytes))
            {
                if (skBitmap == null)
                {
                    return null;
                }

                using (var skImage = SKImage.FromBitmap(skBitmap))
                using (var png = skImage.Encode(SKEncodedImageFormat.Png, 100))
                using (var ms = new MemoryStream(png.ToArray()))
                using (var original = new Bitmap(ms))
                using (var square = ToThermalMonoBitmap(original, qrSize))
                {
                    return CenterQrOnPaperCanvas(square, paperDots);
                }
            }
        }

        /// <summary>
        /// Paper-wide white canvas; QR centred the same way AlignCenter centres footer text.
        /// </summary>
        private static Bitmap CenterQrOnPaperCanvas(Bitmap qr, int paperDots)
        {
            var width = Math.Max(8, (paperDots + 7) / 8 * 8);
            const int quietZone = 16;
            var height = qr.Height + (quietZone * 2);
            var qrWidth = Math.Min(qr.Width, width);
            var x = (width - qrWidth) / 2;

            var canvas = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(canvas))
            {
                g.Clear(Color.White);
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.DrawImage(qr, x, quietZone, qrWidth, qr.Height);
            }

            return canvas;
        }

        private static Bitmap ToThermalMonoBitmap(Bitmap source, int size)
        {
            var mono = new Bitmap(size, size, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(mono))
            {
                g.Clear(Color.White);
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.DrawImage(source, 0, 0, size, size);
            }

            for (var y = 0; y < mono.Height; y++)
            {
                for (var x = 0; x < mono.Width; x++)
                {
                    var c = mono.GetPixel(x, y);
                    var luminance = (c.R * 299 + c.G * 587 + c.B * 114) / 1000;
                    mono.SetPixel(x, y, luminance < 140 ? Color.Black : Color.White);
                }
            }

            return mono;
        }

        public struct InvoicePrintingData
        {
            public Guid DocumentId { get; set; }
            public string Table { get; set; }
            public string Place { get; set; }
            public DocumentPrintingModel Document { get; set; }
            public CompanyInformation CompanyInformations { get; set; }
            public bool IsSecondCopy { get; set; }
            public int CopyNumber { get; set; }
            public string Reason { get; set; }
            public bool OpenDrawer { get; set; }
        }
    }
}
