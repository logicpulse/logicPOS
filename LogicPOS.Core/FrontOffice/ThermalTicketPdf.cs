using System.Globalization;
using LogicPOS.Application.Features.Finance.Documents.PdfGeneration;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QrCodes;
using QrCodes.Renderers;
using QrCodes.Renderers.Abstractions;
using SkiaSharp;
using ZXing;
using ZXing.QrCode;
using ZXing.SkiaSharp.Rendering;

namespace LogicPOS.Core.FrontOffice;

/// <summary>
/// PDF preview of a front-office thermal ticket (talão), for on-screen view / 2ª via print on any printer.
/// Layout mirrors ESC/POS content; page is continuous ~80 mm wide.
/// </summary>
public static class ThermalTicketPdf
{
    private static readonly HashSet<string> TicketTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "FS", "FR", "FT", "FC"
    };

    static ThermalTicketPdf()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public static bool IsTicketType(string? type)
        => string.IsNullOrWhiteSpace(type) == false && TicketTypes.Contains(type.Trim());

    public static byte[] Render(DocumentPdfData data, bool isSecondCopy)
    {
        var model = new ThermalTicketModel(data, isSecondCopy);
        return model.GeneratePdf();
    }

    private sealed class ThermalTicketModel : IDocument
    {
        private readonly DocumentPdfData _data;
        private readonly bool _isSecondCopy;

        public ThermalTicketModel(DocumentPdfData data, bool isSecondCopy)
        {
            _data = data;
            _isSecondCopy = isSecondCopy;
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.ContinuousSize(80, Unit.Millimetre);
                page.MarginHorizontal(3, Unit.Millimetre);
                page.MarginVertical(4, Unit.Millimetre);
                page.DefaultTextStyle(TextStyle.Default.FontSize(8).FontFamily("Arial"));
                page.Content().Column(column =>
                {
                    column.Spacing(2);
                    ComposeHeader(column);
                    ComposeDocumentTitle(column);
                    ComposeCustomer(column);
                    ComposeLines(column);
                    ComposeTotals(column);
                    ComposeTaxResume(column);
                    ComposePayments(column);
                    ComposeFiscal(column);
                    ComposeFooter(column);
                });
            });
        }

        private void ComposeHeader(ColumnDescriptor column)
        {
            var company = _data.Company;
            var business = string.IsNullOrWhiteSpace(company.BusinessName) ? company.Name : company.BusinessName;
            column.Item().AlignCenter().Text(business).Bold().FontSize(11);
            if (string.Equals(business, company.Name, StringComparison.OrdinalIgnoreCase) == false
                && string.IsNullOrWhiteSpace(company.Name) == false)
            {
                column.Item().AlignCenter().Text(company.Name).FontSize(9);
            }

            column.Item().AlignCenter().Text(company.Address ?? string.Empty).FontSize(7);
            column.Item().AlignCenter()
                .Text($"{company.PostalCode} {company.City} - {company.CountryCode2}".Trim(' ', '-'))
                .FontSize(7);
            if (string.IsNullOrWhiteSpace(company.Phone) == false)
            {
                column.Item().AlignCenter().Text($"Telefone: {company.Phone}").FontSize(7);
            }

            if (string.IsNullOrWhiteSpace(company.FiscalNumber) == false)
            {
                column.Item().AlignCenter().Text($"NIF: {company.FiscalNumber}").FontSize(7);
            }

            column.Item().PaddingVertical(2).LineHorizontal(0.5f).LineColor(Colors.Grey.Medium);
        }

        private void ComposeDocumentTitle(ColumnDescriptor column)
        {
            var document = _data.Document!;
            column.Item().AlignCenter().Text(document.TypeName).Bold().FontSize(11);
            column.Item().AlignCenter().Text(document.Number).Bold().FontSize(10);
            column.Item().AlignCenter().Text(_isSecondCopy ? "2ª Via" : "Original").FontSize(9);
            column.Item().AlignCenter()
                .Text(document.CreatedAt.ToString("d", CultureInfo.CurrentCulture))
                .FontSize(8);
            if (document.Status == "A")
            {
                column.Item().AlignCenter().Text("ANULADO").Bold().FontSize(12).FontColor(Colors.Red.Medium);
            }

            column.Item().PaddingVertical(2).LineHorizontal(0.5f).LineColor(Colors.Grey.Medium);
        }

        private void ComposeCustomer(ColumnDescriptor column)
        {
            var customer = _data.Document!.Customer;
            column.Item().Text($"Cliente: {customer.Name}").FontSize(8);
            if (string.IsNullOrWhiteSpace(customer.Address) == false)
            {
                column.Item().Text($"Morada: {customer.Address}").FontSize(7);
            }

            var place = string.Join(" ", new[] { customer.ZipCode, customer.City }.Where(item => string.IsNullOrWhiteSpace(item) == false));
            if (string.IsNullOrWhiteSpace(place) == false || string.IsNullOrWhiteSpace(customer.Country) == false)
            {
                column.Item().Text(string.IsNullOrWhiteSpace(place) ? customer.Country : $"{place} - {customer.Country}").FontSize(7);
            }

            if (string.IsNullOrWhiteSpace(customer.FiscalNumber) == false)
            {
                column.Item().Text($"NIF: {customer.FiscalNumber}").FontSize(8);
            }

            column.Item().PaddingVertical(2).LineHorizontal(0.5f).LineColor(Colors.Grey.Medium);
        }

        private void ComposeLines(ColumnDescriptor column)
        {
            foreach (var detail in _data.Document!.Details)
            {
                column.Item().Text(detail.Designation).Bold().FontSize(8);
                var left =
                    $"{detail.Quantity.ToString("0.##", CultureInfo.CurrentCulture)}"
                    + (string.IsNullOrWhiteSpace(detail.Unit) ? string.Empty : $" {detail.Unit}")
                    + $" x {detail.Price.ToString("0.00", CultureInfo.CurrentCulture)}"
                    + $" IVA {detail.Tax.Percentage.ToString("0.##", CultureInfo.CurrentCulture)}%";
                if (detail.Discount != 0m)
                {
                    left += $" Desc {detail.Discount.ToString("0.##", CultureInfo.CurrentCulture)}%";
                }

                var total = detail.TotalFinal.ToString("0.00", CultureInfo.CurrentCulture);
                column.Item().Row(row =>
                {
                    row.RelativeItem().Text(left).FontSize(7);
                    row.AutoItem().Text(total).FontSize(7);
                });
                if (string.IsNullOrWhiteSpace(detail.VatExemptionReason) == false)
                {
                    column.Item().Text(detail.VatExemptionReason).FontSize(6).Italic();
                }
            }

            column.Item().PaddingVertical(2).LineHorizontal(0.5f).LineColor(Colors.Grey.Medium);
        }

        private void ComposeTotals(ColumnDescriptor column)
        {
            var document = _data.Document!;
            AddTotalRow(column, "Total Ilíquido", document.TotalNet);
            AddTotalRow(column, "Total Imposto", document.TotalTax);
            AddTotalRow(column, "Total Final", document.TotalFinal, bold: true);
            column.Item().PaddingVertical(2).LineHorizontal(0.5f).LineColor(Colors.Grey.Medium);
        }

        private static void AddTotalRow(ColumnDescriptor column, string label, decimal value, bool bold = false)
        {
            column.Item().Row(row =>
            {
                var labelItem = row.RelativeItem().Text(label).FontSize(8);
                var valueItem = row.AutoItem().Text(value.ToString("F2", CultureInfo.CurrentCulture)).FontSize(8);
                if (bold)
                {
                    labelItem.Bold();
                    valueItem.Bold();
                }
            });
        }

        private void ComposeTaxResume(ColumnDescriptor column)
        {
            column.Item().Text("Resumo IVA").Bold().FontSize(8);
            foreach (var resume in _data.Document!.GetTaxResumes())
            {
                column.Item().Row(row =>
                {
                    row.RelativeItem().Text($"{resume.Designation} {resume.Rate:0.##}%").FontSize(7);
                    row.ConstantItem(45).AlignRight().Text(resume.Base.ToString("F2", CultureInfo.CurrentCulture)).FontSize(7);
                    row.ConstantItem(40).AlignRight().Text(resume.Total.ToString("F2", CultureInfo.CurrentCulture)).FontSize(7);
                });
            }

            column.Item().PaddingVertical(2).LineHorizontal(0.5f).LineColor(Colors.Grey.Medium);
        }

        private void ComposePayments(ColumnDescriptor column)
        {
            var document = _data.Document!;
            if (string.IsNullOrWhiteSpace(document.PaymentCondition) == false)
            {
                column.Item().Text($"Condições: {document.PaymentCondition}").FontSize(7);
            }

            if (document.PaymentMethods is { Count: > 0 })
            {
                foreach (var method in document.PaymentMethods)
                {
                    column.Item()
                        .Text($"{method.Designation}: {method.Amount.ToString("F2", CultureInfo.CurrentCulture)}")
                        .FontSize(7);
                }
            }

            column.Item().Text($"Moeda: {document.Currency}").FontSize(7);
            column.Item().PaddingVertical(2).LineHorizontal(0.5f).LineColor(Colors.Grey.Medium);
        }

        private void ComposeFiscal(ColumnDescriptor column)
        {
            var document = _data.Document!;
            if (string.IsNullOrWhiteSpace(document.Atcud) == false)
            {
                column.Item().AlignCenter().Text($"ATCUD: {document.Atcud}").FontSize(7);
            }

            var hashPreview = HashPreview(document.Hash);
            if (string.IsNullOrWhiteSpace(hashPreview) == false)
            {
                column.Item().AlignCenter().Text($"{hashPreview}-Processado por programa certificado").FontSize(6);
            }

            if (string.IsNullOrWhiteSpace(document.FiscalCodeLine) == false)
            {
                column.Item().AlignCenter().Text(document.FiscalCodeLine).FontSize(7);
            }

            var qrPayload = string.IsNullOrWhiteSpace(document.AtQRCode) ? document.Number : document.AtQRCode;
            if (string.IsNullOrWhiteSpace(qrPayload) == false)
            {
                var qr = BuildQrPng(qrPayload);
                if (qr.Length > 0)
                {
                    column.Item().PaddingTop(4).AlignCenter().Width(42, Unit.Millimetre).Image(qr);
                }
            }
        }

        private void ComposeFooter(ColumnDescriptor column)
        {
            var company = _data.Company;
            if (string.IsNullOrWhiteSpace(company.TicketFinalLine1) == false)
            {
                column.Item().AlignCenter().Text(company.TicketFinalLine1).FontSize(7);
            }

            if (string.IsNullOrWhiteSpace(company.TicketFinalLine2) == false)
            {
                column.Item().AlignCenter().Text(company.TicketFinalLine2).FontSize(7);
            }

            column.Item().PaddingTop(4).AlignCenter()
                .Text($"Impresso em: {DateTime.Now.ToString("g", CultureInfo.CurrentCulture)}")
                .FontSize(6);
            column.Item().AlignCenter()
                .Text($"LogicPulse: LogicPOS v{_data.SoftwareVersion}")
                .FontSize(6);
            if (_isSecondCopy)
            {
                column.Item().PaddingTop(2).AlignCenter().Text("2ª Via — conteúdo idêntico ao original").Bold().FontSize(7);
            }
        }

        private static string HashPreview(string? hash)
        {
            if (string.IsNullOrWhiteSpace(hash) || hash.Length < 31)
            {
                return string.Empty;
            }

            return $"{hash[0]}{hash[10]}{hash[20]}{hash[30]}";
        }

        private static byte[] BuildQrPng(string text)
        {
            try
            {
                var writer = new BarcodeWriter<SKBitmap>
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new QrCodeEncodingOptions
                    {
                        Width = 280,
                        Height = 280,
                        Margin = 1,
                        CharacterSet = "UTF-8",
                        ErrorCorrection = ZXing.QrCode.Internal.ErrorCorrectionLevel.M
                    },
                    Renderer = new SKBitmapRenderer
                    {
                        Background = SKColors.White,
                        Foreground = SKColors.Black
                    }
                };
                using var bitmap = writer.Write(text);
                using var image = SKImage.FromBitmap(bitmap);
                using var data = image.Encode(SKEncodedImageFormat.Png, 90);
                return data.ToArray();
            }
            catch
            {
                try
                {
                    var qrCode = QrCodeGenerator.Generate(text, ErrorCorrectionLevel.Medium, forceUtf8: true);
                    return new SkiaSharpRenderer().RenderToBytes(qrCode, new RendererSettings
                    {
                        PixelsPerModule = 6,
                        DrawQuietZones = true,
                        FileFormat = FileFormat.Png
                    });
                }
                catch
                {
                    return Array.Empty<byte>();
                }
            }
        }
    }
}
