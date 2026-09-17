using LogicPOS.Api.Features.Common.Responses;
using LogicPOS.Api.Features.Documents.Documents.GetDocumentPreviewPdf;
using LogicPOS.Api.Features.Documents.Receipts.GetReceiptPdf;
using LogicPOS.Api.Features.Finance.Documents.Documents.GetDocumentPreviewData;
using LogicPOS.Api.Features.Finance.Documents.Documents.Prints.GetDocumentPdf;
using LogicPOS.Api.Features.Finance.Documents.Types.Common;
using LogicPOS.UI.PDFViewer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Documents.Utilities
{
    public static class DocumentPdfUtils
    {
        public static List<int> GetPrintCopyNumbers(DocumentType documentType)
        {
            return GetPrintCopyNumbers(documentType == null ? (int?)null : documentType.PrintCopies);
        }

        public static List<int> GetPrintCopyNumbers(int? printCopies)
        {
            var count = printCopies ?? 1;
            if (count < 1)
            {
                count = 1;
            }
            else if (count > 4)
            {
                count = 4;
            }

            return Enumerable.Range(1, count).ToList();
        }

        public static TempFile? GetDocumentPdfFileLocation(Guid documentId, IEnumerable<int> copies, bool isSecondCopy)
        {
            var mediator = DependencyInjection.Mediator;
            var command = new GetDocumentPdfQuery(documentId, isSecondCopy, copies);
            var result = mediator.Send(command).Result;

            if (result.IsError)
            {
                return null;
            }

            return result.Value;
        }

        public static void ViewDocumentPdf(
            Gtk.Window source,
            Guid documentId,
            IEnumerable<int> copies = null,
            bool isSecondCopy = false)
        {
            var copyList = copies ?? new[] { 1 };
            var tempFile = GetDocumentPdfFileLocation(documentId, copyList, isSecondCopy);

            if (tempFile == null)
            {
                return;
            }

            LogicPOSPDFViewer.ShowPDF(tempFile.Value.Path, tempFile.Value.Name);
        }

        public static void ViewReceiptPdf(Gtk.Window source, Guid documentId)
        {
            var tempFile = GetReceiptPdfFileLocation(documentId,1, false);

            if (tempFile == null)
            {
                return;
            }

            LogicPOSPDFViewer.ShowPDF(tempFile.Value.Path, tempFile.Value.Name);
        }

        public static TempFile? GetReceiptPdfFileLocation(Guid documentId, uint copyNumber, bool isSecondCopy)
        {
            var mediator = DependencyInjection.Mediator;
            var command = new GetReceiptPdfQuery(documentId, isSecondCopy, copyNumber);
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
