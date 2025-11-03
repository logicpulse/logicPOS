using LogicPOS.Api.Features.Common.Responses;
using LogicPOS.Api.Features.Documents.Documents.GetDocumentPreviewPdf;
using LogicPOS.Api.Features.Documents.Receipts.GetReceiptPdf;
using LogicPOS.Api.Features.Finance.Documents.Documents.GetDocumentPreviewData;
using LogicPOS.Api.Features.Finance.Documents.Documents.Prints.GetDocumentPdf;
using LogicPOS.UI.PDFViewer;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Documents.Utilities
{
    public static class DocumentPdfUtils
    {
        public static TempFile? GetDocumentPdfFileLocation(Guid documentId, IEnumerable<int> copies)
        {
            var mediator = DependencyInjection.Mediator;
            var command = new GetDocumentPdfQuery(documentId, copies);
            var result = mediator.Send(command).Result;

            if (result.IsError)
            {
                return null;
            }

            return result.Value;
        }

        public static void ViewDocumentPdf(Gtk.Window source, Guid documentId)
        {
            var tempFile = GetDocumentPdfFileLocation(documentId, new int[] { 1 });

            if (tempFile == null)
            {
                return;
            }

            LogicPOSPDFViewer.ShowPDF(tempFile.Value.Path, tempFile.Value.Name);
        }

        public static void ViewReceiptPdf(Gtk.Window source, Guid documentId)
        {
            var tempFile = GetReceiptPdfFileLocation(documentId, 1);

            if (tempFile == null)
            {
                return;
            }

            LogicPOSPDFViewer.ShowPDF(tempFile.Value.Path, tempFile.Value.Name);
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

        private static TempFile? GetDocumentPreviewPdfFileLocation(GetDocumentPreviewPdfQuery query)
        {
            var mediator = DependencyInjection.Mediator;
            var result = mediator.Send(query).Result;
            if (result.IsError)
            {
                return null;
            }
            return result.Value;
        }

        public static void PreviewDocument(Gtk.Window source, GetDocumentPreviewPdfQuery query)
        {
            var tempFile = GetDocumentPreviewPdfFileLocation(query);

            if (tempFile == null)
            {
                return;
            }

            LogicPOSPDFViewer.ShowPDF(tempFile.Value.Path, tempFile.Value.Name);
        }
    }
}
