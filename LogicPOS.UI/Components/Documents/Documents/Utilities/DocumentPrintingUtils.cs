using Spire.Pdf;
using System.Windows.Forms;

namespace LogicPOS.UI.Components.Documents
{
    public static class DocumentPrintingUtils
    {
        public static void PrintWithNativeDialog(string fileLocation)
        {
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            PdfDocument pdf = new PdfDocument();
            pdf.LoadFromFile(fileLocation);
            pdf.PrintSettings.PrinterName = printDialog.PrinterSettings.PrinterName;
            pdf.Print();
            pdf.Dispose();
        }
    }
}
