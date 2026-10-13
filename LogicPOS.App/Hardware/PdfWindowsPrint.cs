using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.Versioning;
using PDFtoImage;
using SkiaSharp;

namespace LogicPOS.App.Hardware;

/// <summary>
/// Prints a PDF through the Windows spooler (GDI). Shell verbs print/printto are unreliable
/// when Edge/Chrome owns the .pdf association and does not register those verbs.
/// </summary>
internal static class PdfWindowsPrint
{
    [SupportedOSPlatform("windows")]
    public static void Print(string pdfPath, string? printerName = null)
    {
        if (OperatingSystem.IsWindows() == false)
        {
            throw new PlatformNotSupportedException("A impressão de PDF só está disponível em Windows.");
        }

        if (string.IsNullOrWhiteSpace(pdfPath) || File.Exists(pdfPath) == false)
        {
            throw new FileNotFoundException("PDF não encontrado.", pdfPath);
        }

        var bytes = File.ReadAllBytes(pdfPath);
        var pages = Conversion.ToImages(bytes, password: null, new RenderOptions { Dpi = 220 }).ToList();
        if (pages.Count == 0)
        {
            throw new InvalidOperationException("O PDF não tem páginas.");
        }

        try
        {
            var index = 0;
            using var document = new PrintDocument();
            if (string.IsNullOrWhiteSpace(printerName) == false)
            {
                document.PrinterSettings.PrinterName = printerName.Trim();
                if (document.PrinterSettings.IsValid == false)
                {
                    throw new InvalidOperationException($"Impressora \"{printerName}\" inválida ou indisponível.");
                }
            }

            document.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
            document.PrintPage += (_, args) =>
            {
                var page = pages[index];
                using var encoded = page.Encode(SKEncodedImageFormat.Png, 95);
                using var stream = new MemoryStream(encoded.ToArray());
                using var image = Image.FromStream(stream);
                var area = args.MarginBounds;
                var scale = Math.Min(area.Width / (float)image.Width, area.Height / (float)image.Height);
                if (scale <= 0)
                {
                    scale = 1;
                }

                var width = image.Width * scale;
                var height = image.Height * scale;
                var left = area.Left + (area.Width - width) / 2f;
                args.Graphics!.DrawImage(image, left, area.Top, width, height);
                index++;
                args.HasMorePages = index < pages.Count;
            };

            document.Print();
        }
        finally
        {
            foreach (var page in pages)
            {
                page.Dispose();
            }
        }
    }
}
