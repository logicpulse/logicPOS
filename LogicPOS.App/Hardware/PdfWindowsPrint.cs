using System.Diagnostics;

namespace LogicPOS.App.Hardware;

/// <summary>
/// Opens a PDF with the OS default app so the user can print from there.
/// Avoids WinForms / GDI+ private assemblies that break self-contained and non-Windows builds.
/// </summary>
internal static class PdfWindowsPrint
{
    /// <summary>
    /// Opens the PDF in the default viewer (Windows, Linux, macOS).
    /// <paramref name="printerName"/> is ignored — targeting a spooler needs OS-specific APIs we do not ship.
    /// </summary>
    public static bool PrintOrOpen(string pdfPath, string? printerName = null)
    {
        _ = printerName;
        return OpenWithDefaultApp(pdfPath);
    }

    /// <summary>Same as <see cref="PrintOrOpen"/>; kept for existing call sites.</summary>
    public static void Print(string pdfPath, string? printerName = null)
    {
        if (PrintOrOpen(pdfPath, printerName) == false)
        {
            throw new InvalidOperationException("Não foi possível abrir o PDF.");
        }
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
}
