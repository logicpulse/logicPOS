using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Finance.Documents.Types.Common;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Finance.Currencies;
using LogicPOS.UI.Components.Finance.Documents.Services;
using LogicPOS.UI.Components.FiscalYears;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Services;
using LogicPOS.Utility;
using System;
using System.Linq;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public partial class DocumentTab
    {
        private void BtnSelectCopyDocument_Clicked(object sender, EventArgs e)
        {
            var selectCopyDocumentModal = new DocumentsModal(SourceWindow, Finance.Documents.Modals.DocumentsModal.DocumentsModalMode.Selection);
            ResponseType response = (ResponseType)selectCopyDocumentModal.Run();

            if (response != ResponseType.Ok || selectCopyDocumentModal.Page.SelectedEntity == null)
            {
                selectCopyDocumentModal.Destroy();
                return;
            }

            var copyDocument = selectCopyDocumentModal.Page.SelectedEntity;

            if (copyDocument.IsDraft)
            {
                CustomAlerts.Warning(this.SourceWindow)
                 .WithTitle("Documento inválido")
                 .WithMessage("Não é possível selecionar um rascunho.")
                 .ShowAlert();

                selectCopyDocumentModal.Destroy();
                return;
            }

            var docType = GetDocumentTypeFromDocument(copyDocument);

            if (docType == null)
            {
                CustomAlerts.Warning(this.SourceWindow)
                 .WithTitle("Tipo de documento não encontrado")
                 .WithMessage($"O tipo de documento ({copyDocument.Type}) não foi encontrado nas séries do ano fiscal actual ({FiscalYearsService.CurrentFiscalYear.Acronym}).")
                 .ShowAlert();

                docType = SelectDocumentType();

                if (docType == null)
                {
                    selectCopyDocumentModal.Destroy();
                    return;
                }
            }

            ImportCopyDocument(copyDocument, docType);

            var fullDocument = DocumentsService.GetDocument(copyDocument.Id);

            ImportPaymentMethods(fullDocument);

            CopyDocumentSelected?.Invoke(fullDocument);

            selectCopyDocumentModal.Destroy();

        }

        private void ImportCopyDocument(Api.Features.Finance.Documents.Documents.Common.DocumentViewModel copyDocument, DocumentType docType)
        {
            TxtDocumentType.SelectedEntity = docType;
            TxtDocumentType.Text = docType.Designation;

            UpdateValidatableFields();

            TxtCopyDocument.Text = copyDocument.Number;
            TxtCopyDocument.SelectedEntity = copyDocument;

            TxtPaymentCondition.SelectedEntity = copyDocument.PaymentCondition;
            TxtPaymentCondition.Text = copyDocument.PaymentCondition?.Designation;

            TxtCurrency.SelectedEntity = copyDocument.Currency;
            TxtCurrency.Text = copyDocument.Currency.Designation;
            TxtNotes.Text = copyDocument.Notes;
        }

        private void BtnSelectOriginDocument_Clicked(object sender, EventArgs e)
        {
            var modal = new DocumentsModal(SourceWindow, Finance.Documents.Modals.DocumentsModal.DocumentsModalMode.Selection);
            ResponseType response = (ResponseType)modal.Run();

            if (response == ResponseType.Ok && modal.Page.SelectedEntity != null)
            {
                if (modal.Page.SelectedEntity.IsActive == false)
                {
                    CustomAlerts.Warning(this.SourceWindow)
                     .WithTitle("Documento inválido")
                     .WithMessage("Não é possível selecionar um documento que se encontra cancelado/anulado ou em rascunho.")
                     .ShowAlert();

                    modal.Destroy();

                    return;
                }

                TxtOriginDocument.Text = modal.Page.SelectedEntity.Number;
                TxtOriginDocument.SelectedEntity = modal.Page.SelectedEntity;


                var fullDocument = DocumentsService.GetDocument(modal.Page.SelectedEntity.Id);
                OriginDocumentSelected?.Invoke(fullDocument);
            }

            modal.Destroy();
        }

        private void ImportPaymentMethods(Document document)
        {
            if (document.PaymentMethods != null && document.PaymentMethods.Any())
            {
                if (SystemInformationService.SystemInformation.IsPortugal)
                {
                    var payment = document.PaymentMethods.First();
                    TxtPaymentMethod.SelectedEntity = payment.PaymentMethod;
                    TxtPaymentMethod.Text = payment.PaymentMethod.Designation;
                }
            }
        }

        private void BtnSelectCurrency_Clicked(object sender, EventArgs e)
        {
            var page = new CurrenciesPage(null, PageOptions.SelectionPageOptions);
            var selectCurrencyModal = new EntitySelectionModal<Currency>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectCurrencyModal.Run();
            selectCurrencyModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtCurrency.Text = page.SelectedEntity.Designation;
                TxtCurrency.SelectedEntity = page.SelectedEntity;
                var selectedCurrency = page.SelectedEntity;
            }
        }

        private void BtnSelectPaymentCondition_Clicked(object sender, EventArgs e)
        {
            var page = new PaymentConditionsPage(null, PageOptions.SelectionPageOptions);
            var selectPaymentConditionModal = new EntitySelectionModal<PaymentCondition>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectPaymentConditionModal.Run();
            selectPaymentConditionModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtPaymentCondition.Text = page.SelectedEntity.Designation;
                TxtPaymentCondition.SelectedEntity = page.SelectedEntity;
            }
        }

        private void BtnSelectDocumentType_Clicked(object sender, EventArgs e)
        {
            var docType = SelectDocumentType();

            if (docType == null)
            {
                return;
            }

            TxtDocumentType.Text = docType.Designation;
            TxtDocumentType.SelectedEntity = docType;
            UpdateValidatableFields();
            DocumentTypeSelected?.Invoke(docType);
        }

        private void TxtDocumentType_Changed(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtDocumentType.Text))
            {
                TxtDocumentType.Clear();
            }
        }

        private void TxtPaymentCondition_Changed(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtPaymentCondition.Text))
            {
                TxtPaymentCondition.Clear();
            }
        }

        private void BtnSelectPaymentMethod_Clicked(object sender, EventArgs e)
        {
            var page = new PaymentMethodsPage(null, PageOptions.SelectionPageOptions);
            var selectPaymentMethodModal = new EntitySelectionModal<PaymentMethod>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectPaymentMethodModal.Run();
            selectPaymentMethodModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtPaymentMethod.Text = page.SelectedEntity.Designation;
                TxtPaymentMethod.SelectedEntity = page.SelectedEntity;
            }
        }

        private void TxtPaymentMethod_Changed(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(TxtPaymentMethod.Text))
            {
                TxtPaymentMethod.Clear();
            }
        }

        private DocumentType SelectDocumentType()
        {
            var page = new DocumentTypesPage(this.SourceWindow, DocumentTypesPage.ActiveTypesOnlyOptions);
            var selectDocumentTypeModal = new EntitySelectionModal<DocumentType>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectDocumentTypeModal.Run();
            selectDocumentTypeModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                return page.SelectedEntity;
            }

            return null;
        }
    }
}
