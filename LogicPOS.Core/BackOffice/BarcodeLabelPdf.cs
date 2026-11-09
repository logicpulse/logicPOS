using LogicPOS.Shared.Features.Software;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;
using ZXing.SkiaSharp.Rendering;

namespace LogicPOS.Core.BackOffice;

public static class BarcodeLabelPdf
{
    public const string Model100x50 = "100x50";
    public const string Model40x30 = "40x30";

    public static readonly (string Model, string Label)[] Models =
    [
        (Model100x50, "Modelo para impressão de Código de barras 100*50"),
        (Model40x30, "Modelo para impressão de Código de barras 40*30")
    ];

    public static string NormalizeModel(string? model)
    {
        if (string.IsNullOrWhiteSpace(model))
        {
            return Model100x50;
        }

        var match = Models.FirstOrDefault(item =>
            string.Equals(item.Model, model, StringComparison.OrdinalIgnoreCase)
            || string.Equals(item.Label, model, StringComparison.OrdinalIgnoreCase));
        return string.IsNullOrWhiteSpace(match.Model) ? Model100x50 : match.Model;
    }

    public static string LabelOf(string? model)
    {
        var key = NormalizeModel(model);
        return Models.First(item => item.Model == key).Label;
    }

    public static string Write(IReadOnlyList<BarcodeLabelLine> labels, string? forceModel = null)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        var website = (SoftwareInfo.Portugal.CompanyWeb ?? "http://www.logicpulse.com")
            .Replace("http://", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("https://", string.Empty, StringComparison.OrdinalIgnoreCase)
            .TrimEnd('/');

        var forced = string.IsNullOrWhiteSpace(forceModel) ? null : NormalizeModel(forceModel);
        var path = Path.Combine(Path.GetTempPath(), "logicpos-labels-" + Guid.NewGuid().ToString("N") + ".pdf");
        Document.Create(container =>
        {
            foreach (var source in labels)
            {
                var model = forced ?? NormalizeModel(source.PrintModel);
                var label = source with { PrintModel = model };
                var big = string.Equals(model, Model100x50, StringComparison.OrdinalIgnoreCase);
                var bytes = RenderBarcode(label.Serial, big);
                container.Page(page =>
                {
                    page.Size(big ? new PageSize(100, 50, Unit.Millimetre) : new PageSize(40, 30, Unit.Millimetre));
                    page.Margin(big ? 1.5f : 1f, Unit.Millimetre);
                    page.DefaultTextStyle(style => style.LineHeight(0.95f));
                    page.Content().Column(column =>
                    {
                        column.Spacing(big ? 0.8f : 0.4f, Unit.Millimetre);
                        // Stretch barcode across the full printable width of the label.
                        column.Item()
                            .Height(big ? 22 : 12, Unit.Millimetre)
                            .Image(bytes)
                            .FitUnproportionally();
                        column.Item().AlignCenter().Text(Clip("SN: " + label.Serial, big ? 48 : 32))
                            .FontSize(big ? 9 : 6).Bold();
                        if (string.IsNullOrWhiteSpace(label.Designation) == false)
                        {
                            column.Item().AlignCenter().Text(Clip(label.Designation, big ? 54 : 34))
                                .FontSize(big ? 8 : 5.5f);
                        }

                        if (string.IsNullOrWhiteSpace(label.Code) == false)
                        {
                            column.Item().AlignCenter().Text(Clip("ref. " + label.Code, big ? 48 : 32))
                                .FontSize(big ? 7.5f : 5.5f);
                        }

                        column.Item().AlignCenter().Text(Clip(website, big ? 48 : 32))
                            .FontSize(big ? 6.5f : 5);
                    });
                });
            }
        }).GeneratePdf(path);
        return path;
    }

    private static byte[] RenderBarcode(string serial, bool big)
    {
        var writer = new ZXing.BarcodeWriter<SKBitmap>
        {
            Format = ZXing.BarcodeFormat.CODE_128,
            Options = new ZXing.Common.EncodingOptions
            {
                Width = big ? 900 : 480,
                Height = big ? 180 : 120,
                Margin = 0,
                PureBarcode = true
            },
            Renderer = new SKBitmapRenderer
            {
                Background = SKColors.White,
                Foreground = SKColors.Black
            }
        };
        using var bitmap = writer.Write(serial);
        using var data = bitmap.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    private static string Clip(string? value, int maxChars)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var text = value.Trim();
        return text.Length <= maxChars ? text : text[..Math.Max(1, maxChars - 1)].TrimEnd() + "…";
    }
}
