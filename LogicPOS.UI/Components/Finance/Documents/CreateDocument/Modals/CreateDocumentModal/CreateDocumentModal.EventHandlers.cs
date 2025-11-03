using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents;
using LogicPOS.Api.Features.Finance.Customers.Customers.Common;
using LogicPOS.Api.Features.Finance.Documents.Documents.GetDocumentPreviewData;
using LogicPOS.UI.Components.Documents.Utilities;
using LogicPOS.UI.Components.Finance.Documents.CreateDocument.Modals.CreateDocumentModal.DocumentPreviewModal;
using LogicPOS.UI.Components.Finance.Documents.Services;
using LogicPOS.UI.Components.Finance.DocumentTypes;
using System;
using System.EnterpriseServices;
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
                DetailsTab.Page.OnTotalChanged += t => PaymentMethodsTab.PaymentMethodsBox.UpdateDocumentTotal(GetTotalFinal());
            }
            DetailsTab.Page.OnTotalChanged += t => UpdateTitle();
            CustomerTab.CustomerSelected += CustomerTab_CustomerSelected;
            CustomerTab.DiscountChanged += d => UpdateTitle();
        }

        private void AddEventHandlers()
        {
            BtnOk.Clicked += BtnOk_Clicked;
            BtnPreview.Clicked += BtnPreview_Clicked;
            BtnClear.Clicked += BtnClear_Clicked;
            BtnFillCustomerData.Clicked += BtnFillCustomerData_Clicked;
            Navigator.CurrentTabChanged += t => UpdateUI();
            DetailsTab.Page.OnTotalChanged += t => UpdateUI();
            CheckIsDraft.StateChanged += CheckIsDraft_StateChanged;
        }

        private void BtnFillCustomerData_Clicked(object sender, EventArgs e)
        {
            CustomerTab.FillWithAgtInfo();
            Run();
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

            var addCommand = CreateAddCommand();

            if (DraftMode == false)
            {
                var previewQuery = new GetDocumentPreviewDataQuery
                {
                    CurrencyId = addCommand.CurrencyId,
                    Type = addCommand.Type,
                    ShipFromAdress = addCommand.ShipFromAdress,
                    ShipToAdress = addCommand.ShipToAdress,
                    Discount = addCommand.Discount,
                    Notes = addCommand.Notes,
                    Details = addCommand.Details,
                    ExchangeRate = addCommand.ExchangeRate,
                };

                var confirmModal = new DocumentPreviewModal(this, previewQuery);
                var confirmation = (ResponseType)confirmModal.Run();
                confirmModal.Destroy();

                if (confirmation != ResponseType.Yes)
                {
                    Run();
                    return;
                }
            }

            Guid? id = DocumentsService.IssueDocument(addCommand);
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
            DetailsTab.ImportDataFromDocument(document.Id, document.Discount);
        }

        private void OnCopyDocumentSelected(Document document)
        {
            CustomerTab.ImportDataFromDocument(document);
            DetailsTab.ImportDataFromDocument(document.Id, document.Discount);

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
