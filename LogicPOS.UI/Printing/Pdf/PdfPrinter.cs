using Spire.Pdf;
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
                pdf.PrintSettings.PaperSize = CreateA4PaperSize(printerSettings);
            }
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
