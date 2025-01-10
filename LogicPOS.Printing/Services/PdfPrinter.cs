using Spire.Pdf;
using System.Windows.Forms;

namespace LogicPOS.Printing.Services
{
    public static class PdfPrinter
    {
        public static void PrintWithNativeDialog(string fileLocation)
        {
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var printerName = printDialog.PrinterSettings.PrinterName;

            printDialog.Dispose();

            PdfDocument pdf = new PdfDocument();
            pdf.LoadFromFile(fileLocation);
            pdf.PrintSettings.PrinterName = printDialog.PrinterSettings.PrinterName;
            pdf.Print();
            pdf.Dispose();
        }

        public static void Print(string fileLocation, 
                                 string printerName)
        {
            PdfDocument pdf = new PdfDocument();
            pdf.LoadFromFile(fileLocation);
            pdf.PrintSettings.PrinterName = printerName;
            pdf.Print();
            pdf.Dispose();
        }
    }
}
