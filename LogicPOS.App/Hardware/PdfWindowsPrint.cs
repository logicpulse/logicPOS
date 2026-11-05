using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.Versioning;
using PDFtoImage;
using SkiaSharp;

namespace LogicPOS.App.Hardware;

/// <summary>
/// Cross-platform PDF print helper. Windows uses the GDI spooler (no WinForms dialog);
/// Linux/macOS open the PDF in the default viewer so the user can print there.
/// </summary>
internal static class PdfWindowsPrint
{
    /// <summary>
    /// Prints on Windows (default or named printer) or opens the PDF on other OSes.
    /// Returns false only when the user-facing open/print could not be started.
    /// </summary>
    public static bool PrintOrOpen(string pdfPath, string? printerName = null)
    {
        if (OperatingSystem.IsWindows())
        {
            Print(pdfPath, printerName);
            return true;
        }

        return OpenWithDefaultApp(pdfPath);
    }

    [SupportedOSPlatform("windows")]
    public static void Print(string pdfPath, string? printerName = null)
    {
        if (OperatingSystem.IsWindows() == false)
        {
            throw new PlatformNotSupportedException("A impressão GDI só está disponível em Windows.");
        }

        using var session = CreateSession(pdfPath, printerName);
        session.Document.Print();
    }

    public static bool OpenWithDefaultApp(string pdfPath)
    {
        if (string.IsNullOrWhiteSpace(pdfPath) || File.Exists(pdfPath) == false)
        {
            throw new FileNotFoundException("PDF não encontrado.", pdfPath);
        }

        var start = new ProcessStartInfo
        {
            FileName = pdfPath,
            UseShellExecute = true
        };
        return Process.Start(start) is not null;
    }

    [SupportedOSPlatform("windows")]
    private static PrintSession CreateSession(string pdfPath, string? printerName)
    {
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

        var index = 0;
        var document = new PrintDocument();
        if (string.IsNullOrWhiteSpace(printerName) == false)
        {
            document.PrinterSettings.PrinterName = printerName.Trim();
            if (document.PrinterSettings.IsValid == false)
            {
                foreach (var page in pages)
                {
                    page.Dispose();
                }

                document.Dispose();
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

        return new PrintSession(document, pages);
    }

    private sealed class PrintSession : IDisposable
    {
        public PrintSession(PrintDocument document, List<SKBitmap> pages)
        {
            Document = document;
            _pages = pages;
        }

        public PrintDocument Document { get; }

        private readonly List<SKBitmap> _pages;

        public void Dispose()
        {
            Document.Dispose();
            foreach (var page in _pages)
            {
                page.Dispose();
            }
        }
    }
}
