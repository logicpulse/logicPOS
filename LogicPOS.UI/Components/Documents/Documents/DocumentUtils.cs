using LogicPOS.Api.Features.Documents.Documents.GetDocumentPdf;
using LogicPOS.UI.Alerts;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LogicPOS.UI.Components.Documents
{
    public static class DocumentUtils
    {
        public static void ShowPdfFromApi(Gtk.Window source, Guid documentId)
        {
            var mediator = DependencyInjection.Services.GetRequiredService<ISender>();
            var command = new GetDocumentPdfQuery { Id = documentId };
            var result = mediator.Send(command).Result;
     
            if (result.IsError)
            {
                SimpleAlerts.ShowApiErrorAlert(source);
                return;
            }

            var fileLocation = result.Value;

            var pdfViewer = new PDFViewer.Winforms.PDFViewer(fileLocation);

            pdfViewer.ShowDialog();
        }
    }
}
