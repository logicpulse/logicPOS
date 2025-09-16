using Spire.Pdf;
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

            PdfDocument pdf = new PdfDocument();
            pdf.LoadFromFile(fileLocation);
            pdf.PrintSettings.PrinterName = printDialog.PrinterSettings.PrinterName;
            pdf.Print();
            pdf.Dispose();

            return dialogResult;
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
