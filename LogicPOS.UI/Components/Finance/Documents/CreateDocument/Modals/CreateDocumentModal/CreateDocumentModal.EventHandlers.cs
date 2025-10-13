using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents;
using LogicPOS.UI.Components.Documents.Utilities;
using LogicPOS.UI.Components.Finance.Documents.Services;
using LogicPOS.UI.Components.Finance.DocumentTypes;
using System;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class CreateDocumentModal
    {
        private void AddTabsEventHandlers()
        {
            DocumentTab.OriginDocumentSelected += OnOriginDocumentSelected;
            DocumentTab.DocumentTypeSelected += OnDocumentTypeSelected;
            DocumentTab.CopyDocumentSelected += OnCopyDocumentSelected;
            if (SinglePaymentMethod == false)
            {
                DetailsTab.Page.OnTotalChanged += PaymentMethodsTab.PaymentMethodsBox.UpdateDocumentTotal;
            }
            DetailsTab.Page.OnTotalChanged += t => UpdateTitle();
            CustomerTab.CustomerSelected += CustomerTab_CustomerSelected;
        }

        private void AddEventHandlers()
        {
            BtnOk.Clicked += BtnOk_Clicked;
            BtnPreview.Clicked += BtnPreview_Clicked;
            BtnClear.Clicked += BtnClear_Clicked;
            Navigator.CurrentTabChanged += t => UpdateUI();
            DetailsTab.Page.OnTotalChanged += t => UpdateUI();
            CheckIsDraft.StateChanged += CheckIsDraft_StateChanged;
        }

        private void CheckIsDraft_StateChanged(object o, Gtk.StateChangedArgs args)
        {
            UpdateTitle();
        }

        private void BtnOk_Clicked(object sender, EventArgs e)
        {
            if (Validate() == false)
            {
                Run();
                return;
            }

            var command = CreateAddCommand();
            Guid? id = DocumentsService.IssueDocument(command);
            if (id == null)
            {
                Run();
                return;
            }

            if(_draftId != null)
            {
                DocumentsService.DeleteDraft(_draftId.Value);
            }

            DocumentPdfUtils.ViewDocumentPdf(this, id.Value);
        }

        private void BtnClear_Clicked(object sender, EventArgs e)
        {
            CustomerTab.Clear();
            Run();
        }

        private void BtnPreview_Clicked(object sender, EventArgs e)
        {
            if (!TabsForPreviewAreValid())
            {
                ShowValidationErrors();
            }
            else
            {
                var query = CreateDocumentPreviewQuery();
                DocumentPdfUtils.PreviewDocument(this, query);
            }

            Run();
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
            DetailsTab.ImportDataFromDocument(document.Id);
        }

        private void OnCopyDocumentSelected(Document document)
        {
            CustomerTab.ImportDataFromDocument(document);
            DetailsTab.ImportDataFromDocument(document.Id);

            if (document.TypeAnalyzer.IsGuide())
            {
                ShipFromTab.ImportDataFromDocument(document);
                ShipToTab.ImportDataFromDocument(document);
            }

            if(document.PaymentMethods != null && document.PaymentMethods.Any() && SinglePaymentMethod == false)
            {
                PaymentMethodsTab.ImportDataFromDocument(document);
            }

            var documentType = DocumentTypesService.DocumentTypes.Where(docType => docType.Acronym == document.Type).FirstOrDefault()
                ?? DocumentTypesService.Default;

            OnDocumentTypeSelected(documentType);
        }

        private void CustomerTab_CustomerSelected(Customer customer)
        {
            var docTypeAnalyzer = DocumentTab.DocumentTypeAnalyzer;

            if(docTypeAnalyzer == null || docTypeAnalyzer.Value.IsGuide() == false)
            {
                return;
            }

            ShipToTab.ImportCustomerShipAddress(customer);
        }

    }
}
