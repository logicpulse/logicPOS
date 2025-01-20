using LogicPOS.Api.Features.Documents.Documents.GetDocumentPdf;
using LogicPOS.Api.Features.Documents.Receipts.GetReceiptPdf;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LogicPOS.UI.Components.Documents.Utilities
{
    public static class DocumentPdfUtils
    {
        public static void ViewPdf(Gtk.Window source, string fileLocation)
        {
            using (var pdfViewer = new LogicPOS.PDFViewer.Winforms.PDFViewer(fileLocation))
            {
                pdfViewer.ShowDialog();
            }
        }

        public static string GetDocumentPdfFileLocation(Guid documentId, uint copyNumber)
        {
            var mediator = DependencyInjection.Services.GetRequiredService<ISender>();
            var command = new GetDocumentPdfQuery (documentId, copyNumber);
            var result = mediator.Send(command).Result;

            if (result.IsError)
            {
                return null;
            }

            return result.Value;
        }

        public static void ViewDocumentPdf(Gtk.Window source, Guid documentId)
        {
            var fileLocation = GetDocumentPdfFileLocation(documentId,1);

            if (fileLocation == null)
            {
                return;
            }

            using (var pdfViewer = new LogicPOS.PDFViewer.Winforms.PDFViewer(fileLocation))
            {
                pdfViewer.ShowDialog();
            }
        }

        public static void ViewReceiptPdf(Gtk.Window source, Guid documentId)
        {
            var fileLocation = GetReceiptPdfFileLocation(documentId,1);

            if (fileLocation == null)
            {
                return;
            }

            using (var pdfViewer = new LogicPOS.PDFViewer.Winforms.PDFViewer(fileLocation))
            {
                pdfViewer.ShowDialog();
            }
        }

        public static string GetReceiptPdfFileLocation(Guid documentId, uint copyNumber)
        {
            var mediator = DependencyInjection.Services.GetRequiredService<ISender>();
            var command = new GetReceiptPdfQuery(documentId, copyNumber);
            var result = mediator.Send(command).Result;

            if (result.IsError)
            {
                return null;
            }

            return result.Value;
        }
    }
}
