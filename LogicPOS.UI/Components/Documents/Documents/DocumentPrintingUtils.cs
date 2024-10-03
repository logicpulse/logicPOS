using LogicPOS.Api.Features.Documents.Documents.GetDocumentPdf;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Spire.Pdf;
using System;
using System.Windows.Forms;

namespace LogicPOS.UI.Components.Documents
{
    public static class DocumentPrintingUtils
    {

        public static string GetPdfFile(Guid documentId)
        {
            var mediator = DependencyInjection.Services.GetRequiredService<ISender>();
            var command = new GetDocumentPdfQuery { Id = documentId };
            var result = mediator.Send(command).Result;

            if (result.IsError)
            {
                return null;
            }

            return result.Value;
        }


        public static void ShowPdf(Gtk.Window source, Guid documentId)
        {
            var fileLocation = GetPdfFile(documentId);

            if (fileLocation == null)
            {
                return;
            }

            using (var pdfViewer = new LogicPOS.PDFViewer.Winforms.PDFViewer(fileLocation))
            {
                pdfViewer.ShowDialog();
            }
        }

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
