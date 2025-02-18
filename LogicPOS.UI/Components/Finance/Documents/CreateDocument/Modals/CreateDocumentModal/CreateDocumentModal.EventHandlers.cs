using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.Documents.Utilities;
using LogicPOS.UI.Errors;
using System;

namespace LogicPOS.UI.Components.Modals
{
    public partial class CreateDocumentModal
    {
        private void BtnOk_Clicked(object sender, EventArgs e)
        {
            if (!AllTabsAreValid())
            {
                ShowValidationErrors();
                Run();
                return;
            }

            var command = CreateAddCommand();
            var result = _mediator.Send(command).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result, source: this);
                Run();
                return;
            }

            DocumentPdfUtils.ViewDocumentPdf(this, result.Value);
        }

        private void OnDocumentTypeSelected(DocumentType documentType)
        {
            ShowTabsForDocumentType(documentType);
            EnableTabsForDocumentType(documentType);
            Navigator.UpdateUI();
        }

        private void OnOriginDocumentSelected(Document document)
        {
            CustomerTab.ImportDataFromDocument(document);
            ArticlesTab.ImportDataFromDocument(document);
        }

        private void OnCopyDocumentSelected(Document document)
        {
            CustomerTab.ImportDataFromDocument(document);
            ArticlesTab.ImportDataFromDocument(document);

            if (document.TypeAnalyzer.IsGuide())
            {
                ShipFromTab.ImportDataFromDocument(document);
                ShipToTab.ImportDataFromDocument(document);
            }

            OnDocumentTypeSelected(document.Series.DocumentType);
        }

    }
}
