using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using PDFtoImage;
using SkiaSharp;

namespace LogicPOS.App.Hardware;

/// <summary>
/// Prints a PDF directly (no external viewer, no WinForms, no System.Drawing).
/// Windows: SkiaSharp pages → Win32 spooler. Linux/macOS: <c>lpr</c>.
/// </summary>
internal static class PdfWindowsPrint
{
    /// <param name="paperWidthMm">
    /// When set with <paramref name="paperHeightMm"/> (barcode labels), asks the printer for that
    /// media size so each PDF page lands on one sticker instead of being stretched to A4/default.
    /// </param>
    public static bool PrintOrOpen(
        string pdfPath,
        string? printerName = null,
        float? paperWidthMm = null,
        float? paperHeightMm = null)
    {
        if (OperatingSystem.IsWindows())
        {
            Print(pdfPath, printerName, paperWidthMm, paperHeightMm);
            return true;
        }

        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
        {
            return TryUnixLpr(pdfPath, printerName);
        }

        return OpenWithDefaultApp(pdfPath);
    }

    [SupportedOSPlatform("windows")]
    public static void Print(
        string pdfPath,
        string? printerName = null,
        float? paperWidthMm = null,
        float? paperHeightMm = null)
    {
        if (OperatingSystem.IsWindows() == false)
        {
            throw new PlatformNotSupportedException("A impressão directa só está disponível em Windows.");
        }

        EnsurePdf(pdfPath);
        var bytes = File.ReadAllBytes(pdfPath);
        var dpi = paperWidthMm is > 0 && paperHeightMm is > 0 ? 300 : 200;
        var pages = Conversion.ToImages(bytes, password: null, new RenderOptions { Dpi = dpi }).ToList();
        if (pages.Count == 0)
        {
            throw new InvalidOperationException("O PDF não tem páginas.");
        }

        try
        {
            WinSpoolBitmapPrint.Print(pages, printerName, paperWidthMm, paperHeightMm, dpi);
        }
        finally
        {
            foreach (var page in pages)
            {
                page.Dispose();
            }
        }
    }

    public static bool OpenWithDefaultApp(string pdfPath)
    {
        EnsurePdf(pdfPath);
        var start = new ProcessStartInfo
        {
            FileName = pdfPath,
            UseShellExecute = true
        };
        return Process.Start(start) is not null;
    }

    private static bool TryUnixLpr(string pdfPath, string? printerName)
    {
        EnsurePdf(pdfPath);
        var args = string.IsNullOrWhiteSpace(printerName)
            ? $"\"{pdfPath}\""
            : $"-P \"{printerName.Trim()}\" \"{pdfPath}\"";
        var start = new ProcessStartInfo
        {
            FileName = "lpr",
            Arguments = args,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardError = true
        };
        using var process = Process.Start(start);
        if (process is null)
        {
            return false;
        }

        process.WaitForExit(30_000);
        return process.ExitCode == 0;
    }

    private static void EnsurePdf(string pdfPath)
    {
        if (string.IsNullOrWhiteSpace(pdfPath) || File.Exists(pdfPath) == false)
        {
            throw new FileNotFoundException("PDF não encontrado.", pdfPath);
        }
    }
}

/// <summary>Minimal Win32 bitmap printing via gdi32/winspool — no System.Drawing.</summary>
file static class WinSpoolBitmapPrint
{
    private const int DmOutBuffer = 2;
    private const int DmInBuffer = 8;
    private const int DmInOut = DmInBuffer | DmOutBuffer;
    private const int DmOrientation = 0x00000001;
    private const int DmPaperSize = 0x00000002;
    private const int DmPaperLength = 0x00000004;
    private const int DmPaperWidth = 0x00000008;
    private const short DmPaperUser = 256;
    private const short DmOrientPortrait = 1;

    // DEVMODEW fixed-header offsets (Unicode).
    private const int OffDmFields = 72;
    private const int OffDmOrientation = 76;
    private const int OffDmPaperSize = 78;
    private const int OffDmPaperLength = 80;
    private const int OffDmPaperWidth = 82;

    public static void Print(
        IReadOnlyList<SKBitmap> pages,
        string? printerName,
        float? paperWidthMm,
        float? paperHeightMm,
        int renderDpi)
    {
        var device = string.IsNullOrWhiteSpace(printerName) ? PreferDefaultPrinter() : printerName.Trim();
        var labelMode = paperWidthMm is > 0 && paperHeightMm is > 0;
        var hdc = labelMode
            ? CreatePrinterDc(device, paperWidthMm!.Value, paperHeightMm!.Value)
            : CreateDC(null, device, null, IntPtr.Zero);
        if (hdc == IntPtr.Zero)
        {
            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(printerName)
                    ? "Não foi possível aceder à impressora por defeito."
                    : $"Impressora \"{device}\" inválida ou indisponível.");
        }

        try
        {
            var gdiDoc = new DocInfoW
            {
                SizeOfStruct = Marshal.SizeOf<DocInfoW>(),
                DocName = labelMode ? "logicpos-labels" : "logicpos",
                Output = null,
                DataType = null
            };
            if (StartDoc(hdc, ref gdiDoc) <= 0)
            {
                throw new InvalidOperationException("Falha ao iniciar o trabalho de impressão.");
            }

            try
            {
                foreach (var page in pages)
                {
                    if (StartPage(hdc) <= 0)
                    {
                        throw new InvalidOperationException("Falha ao iniciar a página de impressão.");
                    }

                    try
                    {
                        DrawPage(hdc, page, labelMode, renderDpi);
                    }
                    finally
                    {
                        EndPage(hdc);
                    }
                }
            }
            finally
            {
                EndDoc(hdc);
            }
        }
        finally
        {
            DeleteDC(hdc);
        }
    }

    private static IntPtr CreatePrinterDc(string device, float widthMm, float heightMm)
    {
        if (OpenPrinter(device, out var hPrinter, IntPtr.Zero) == false || hPrinter == IntPtr.Zero)
        {
            return CreateDC(null, device, null, IntPtr.Zero);
        }

        IntPtr pDevMode = IntPtr.Zero;
        try
        {
            var bytesNeeded = DocumentProperties(IntPtr.Zero, hPrinter, device, IntPtr.Zero, IntPtr.Zero, 0);
            if (bytesNeeded <= 0)
            {
                return CreateDC(null, device, null, IntPtr.Zero);
            }

            pDevMode = Marshal.AllocHGlobal(bytesNeeded);
            if (DocumentProperties(IntPtr.Zero, hPrinter, device, pDevMode, IntPtr.Zero, DmOutBuffer) < 0)
            {
                return CreateDC(null, device, null, IntPtr.Zero);
            }

            var widthTenths = (short)Math.Clamp(Math.Round(widthMm * 10), 1, short.MaxValue);
            var heightTenths = (short)Math.Clamp(Math.Round(heightMm * 10), 1, short.MaxValue);
            var fields = Marshal.ReadInt32(pDevMode, OffDmFields);
            fields |= DmOrientation | DmPaperSize | DmPaperLength | DmPaperWidth;
            Marshal.WriteInt32(pDevMode, OffDmFields, fields);
            // Portrait + custom size matches the PDF page (already WxH mm); landscape would rotate twice.
            Marshal.WriteInt16(pDevMode, OffDmOrientation, DmOrientPortrait);
            Marshal.WriteInt16(pDevMode, OffDmPaperSize, DmPaperUser);
            Marshal.WriteInt16(pDevMode, OffDmPaperWidth, widthTenths);
            Marshal.WriteInt16(pDevMode, OffDmPaperLength, heightTenths);

            _ = DocumentProperties(IntPtr.Zero, hPrinter, device, pDevMode, pDevMode, DmInOut);
            var hdc = CreateDC(null, device, null, pDevMode);
            return hdc != IntPtr.Zero ? hdc : CreateDC(null, device, null, IntPtr.Zero);
        }
        finally
        {
            if (pDevMode != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(pDevMode);
            }

            ClosePrinter(hPrinter);
        }
    }

    private static void DrawPage(IntPtr hdc, SKBitmap page, bool labelMode, int renderDpi)
    {
        var printableWidth = GetDeviceCaps(hdc, HORZRES);
        var printableHeight = GetDeviceCaps(hdc, VERTRES);
        if (printableWidth <= 0 || printableHeight <= 0)
        {
            throw new InvalidOperationException("A impressora não reportou área útil.");
        }

        using var flipped = new SKBitmap(page.Width, page.Height, SKColorType.Bgra8888, SKAlphaType.Premul);
        using (var canvas = new SKCanvas(flipped))
        {
            canvas.Clear(SKColors.White);
            canvas.DrawBitmap(page, 0, 0);
        }

        int destW;
        int destH;
        int destX;
        int destY;
        if (labelMode)
        {
            // 1:1 physical size from the PDF render DPI; do not stretch to a larger default page.
            var dpiX = Math.Max(1, GetDeviceCaps(hdc, LOGPIXELSX));
            var dpiY = Math.Max(1, GetDeviceCaps(hdc, LOGPIXELSY));
            destW = Math.Max(1, (int)Math.Round(flipped.Width * (dpiX / (double)renderDpi)));
            destH = Math.Max(1, (int)Math.Round(flipped.Height * (dpiY / (double)renderDpi)));
            if (destW > printableWidth || destH > printableHeight)
            {
                var fit = Math.Min(printableWidth / (double)destW, printableHeight / (double)destH);
                destW = Math.Max(1, (int)Math.Round(destW * fit));
                destH = Math.Max(1, (int)Math.Round(destH * fit));
            }

            destX = Math.Max(0, (printableWidth - destW) / 2);
            destY = Math.Max(0, (printableHeight - destH) / 2);
        }
        else
        {
            var scale = Math.Min(
                printableWidth / (double)flipped.Width,
                printableHeight / (double)flipped.Height);
            destW = Math.Max(1, (int)Math.Round(flipped.Width * scale));
            destH = Math.Max(1, (int)Math.Round(flipped.Height * scale));
            destX = Math.Max(0, (printableWidth - destW) / 2);
            destY = Math.Max(0, (printableHeight - destH) / 2);
        }

        var info = flipped.Info;
        var pixels = flipped.GetPixels();
        var bmi = new BitmapInfo
        {
            Size = 40,
            Width = info.Width,
            Height = -info.Height, // top-down
            Planes = 1,
            BitCount = 32,
            Compression = BI_RGB
        };

        var copied = StretchDIBits(
            hdc,
            destX,
            destY,
            destW,
            destH,
            0,
            0,
            info.Width,
            info.Height,
            pixels,
            ref bmi,
            DIB_RGB_COLORS,
            SRCCOPY);
        if (copied == 0 || copied == GDI_ERROR)
        {
            throw new InvalidOperationException("Falha ao enviar a página para a impressora.");
        }
    }

    private static string PreferDefaultPrinter()
    {
        var buffer = new char[256];
        var size = buffer.Length;
        if (GetDefaultPrinter(buffer, ref size) && size > 1)
        {
            return new string(buffer, 0, size - 1);
        }

        throw new InvalidOperationException("Não há impressora por defeito configurada.");
    }

    private const int HORZRES = 8;
    private const int VERTRES = 10;
    private const int LOGPIXELSX = 88;
    private const int LOGPIXELSY = 90;
    private const int BI_RGB = 0;
    private const int DIB_RGB_COLORS = 0;
    private const uint SRCCOPY = 0x00CC0020;
    private const int GDI_ERROR = unchecked((int)0xFFFFFFFF);

    [DllImport("winspool.drv", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool GetDefaultPrinter(char[] buffer, ref int bufferSize);

    [DllImport("winspool.drv", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool OpenPrinter(string printerName, out IntPtr printerHandle, IntPtr defaults);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool ClosePrinter(IntPtr printerHandle);

    [DllImport("winspool.drv", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int DocumentProperties(
        IntPtr hwnd,
        IntPtr printerHandle,
        string deviceName,
        IntPtr devModeOutput,
        IntPtr devModeInput,
        int mode);

    [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr CreateDC(string? driver, string device, string? output, IntPtr initData);

    [DllImport("gdi32.dll")]
    private static extern bool DeleteDC(IntPtr hdc);

    [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
    private static extern int StartDoc(IntPtr hdc, ref DocInfoW docInfo);

    [DllImport("gdi32.dll")]
    private static extern int EndDoc(IntPtr hdc);

    [DllImport("gdi32.dll")]
    private static extern int StartPage(IntPtr hdc);

    [DllImport("gdi32.dll")]
    private static extern int EndPage(IntPtr hdc);

    [DllImport("gdi32.dll")]
    private static extern int GetDeviceCaps(IntPtr hdc, int index);

    [DllImport("gdi32.dll")]
    private static extern int StretchDIBits(
        IntPtr hdc,
        int xDest,
        int yDest,
        int destWidth,
        int destHeight,
        int xSrc,
        int ySrc,
        int srcWidth,
        int srcHeight,
        IntPtr bits,
        ref BitmapInfo info,
        int usage,
        uint rasterOp);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct DocInfoW
    {
        public int SizeOfStruct;
        public string? DocName;
        public string? Output;
        public string? DataType;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct BitmapInfo
    {
        public int Size;
        public int Width;
        public int Height;
        public short Planes;
        public short BitCount;
        public int Compression;
        public int SizeImage;
        public int XPelsPerMeter;
        public int YPelsPerMeter;
        public int ClrUsed;
        public int ClrImportant;
    }
}
