using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Documents.Documents.GetDocumentPdf;
using LogicPOS.Api.Features.Documents.Receipts.GetReceiptPdf;
using LogicPOS.UI.PDFViewer;
using System;

namespace LogicPOS.UI.Components.Documents.Utilities
{
    public static class DocumentPdfUtils
    {
        public static TempFile? GetDocumentPdfFileLocation(Guid documentId, uint copyNumber)
        {
            var mediator = DependencyInjection.Mediator;
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
            var tempFile = GetDocumentPdfFileLocation(documentId,1);

            if (tempFile == null)
            {
                return;
            }

            LogicPOSPDFViewer.ShowPDF(tempFile.Value.Path,tempFile.Value.Name);
        }

        public static void ViewReceiptPdf(Gtk.Window source, Guid documentId)
        {
            var tempFile = GetReceiptPdfFileLocation(documentId,1);

            if (tempFile == null)
            {
                return;
            }

            LogicPOSPDFViewer.ShowPDF(tempFile.Value.Path,tempFile.Value.Name);
        }

        public static TempFile? GetReceiptPdfFileLocation(Guid documentId, uint copyNumber)
        {
            var mediator = DependencyInjection.Mediator;
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
