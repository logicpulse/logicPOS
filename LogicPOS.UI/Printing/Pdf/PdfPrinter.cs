using Spire.Pdf;
using Spire.Pdf.Print;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace LogicPOS.Printing.Services
{
    public static class PdfPrinter
    {
        public static DialogResult PrintWithNativeDialog(string fileLocation)
        {
            PrintDialog printDialog = new PrintDialog();
            var dialogResult = printDialog.ShowDialog();

            if (dialogResult != DialogResult.OK)
            {
                return dialogResult;
            }

            var printerName = printDialog.PrinterSettings.PrinterName;
            printDialog.Dispose();

            using (var pdf = new PdfDocument())
            {
                pdf.LoadFromFile(fileLocation);
                ApplyPrintSettings(pdf, printerName);
                pdf.Print();
            }

            return dialogResult;
        }

        public static void Print(string fileLocation, string printerName)
        {
            using (var pdf = new PdfDocument())
            {
                pdf.LoadFromFile(fileLocation);
                ApplyPrintSettings(pdf, printerName);
                // Avoid Windows "Sending to printer" status dialog blocking the UI thread.
                pdf.PrintSettings.PrintController = new StandardPrintController();
                pdf.Print();
            }
        }

        private static void ApplyPrintSettings(PdfDocument pdf, string printerName)
        {
            pdf.PrintSettings.PrinterName = printerName;

            if (pdf.Pages.Count == 0)
                return;

            var pageSize = pdf.Pages[0].Size;
            var widthPt = (float)pageSize.Width;
            var heightPt = (float)pageSize.Height;

            if (IsLabelPage(widthPt))
            {
                pdf.PrintSettings.Landscape = false;
                int exactWidth = (int)(widthPt / 72.0 * 100.0);
                int exactHeight = (int)(heightPt / 72.0 * 100.0);
                pdf.PrintSettings.PaperSize = new PaperSize("Custom Label", exactWidth, exactHeight);
            }
            else
            {
                pdf.PrintSettings.Landscape = widthPt > heightPt;
                var printerSettings = new PrinterSettings { PrinterName = printerName };
                // A4 PDF + A5 printer default: select A5 and shrink-to-fit (keep A4 path unchanged otherwise).
                if (TryGetA5PaperSize(printerSettings, out var a5Paper))
                {
                    pdf.PrintSettings.PaperSize = a5Paper;
                    pdf.PrintSettings.SelectSinglePageLayout(PdfSinglePageScalingMode.FitSize);
                }
                else
                {
                    pdf.PrintSettings.PaperSize = CreateA4PaperSize(printerSettings);
                }
            }
        }

        /// <summary>
        /// True when the printer's Windows default paper is A5 (or A5-like custom size).
        /// </summary>
        private static bool TryGetA5PaperSize(PrinterSettings printerSettings, out PaperSize a5Paper)
        {
            a5Paper = null;
            if (printerSettings == null)
                return false;

            var defaultPaper = printerSettings.DefaultPageSettings?.PaperSize;
            if (!IsA5Paper(defaultPaper))
                return false;

            if (printerSettings.PaperSizes != null)
            {
                foreach (PaperSize size in printerSettings.PaperSizes)
                {
                    if (size.Kind == PaperKind.A5)
                    {
                        a5Paper = size;
                        return true;
                    }
                }
            }

            a5Paper = defaultPaper;
            return a5Paper != null;
        }

        private static bool IsA5Paper(PaperSize paper)
        {
            if (paper == null)
                return false;

            if (paper.Kind == PaperKind.A5)
                return true;

            // Some drivers expose A5 as Custom; compare portrait dimensions (hundredths of an inch).
            const int a5Short = 583; // 148 mm
            const int a5Long = 827;  // 210 mm
            const int tolerance = 40;
            int shortSide = System.Math.Min(paper.Width, paper.Height);
            int longSide = System.Math.Max(paper.Width, paper.Height);
            return System.Math.Abs(shortSide - a5Short) <= tolerance
                && System.Math.Abs(longSide - a5Long) <= tolerance;
        }

        private static PaperSize CreateA4PaperSize(PrinterSettings printerSettings)
        {
            if (printerSettings?.PaperSizes != null)
            {
                foreach (PaperSize size in printerSettings.PaperSizes)
                {
                    if (size.Kind == PaperKind.A4)
                        return size;
                }
            }

            return new PaperSize("A4", 827, 1169);
        }

        private static bool IsLabelPage(float widthPt)
        {
            const double labelMaxWidthMm = 110;
            return widthPt / 72.0 * 25.4 <= labelMaxWidthMm;
        }
    }
}
