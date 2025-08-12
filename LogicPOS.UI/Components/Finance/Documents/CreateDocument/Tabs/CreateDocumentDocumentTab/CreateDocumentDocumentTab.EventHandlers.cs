using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.FiscalYears;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages;
using LogicPOS.Utility;
using System;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public partial class CreateDocumentDocumentTab
    {
        private void BtnSelectCopyDocument_Clicked(object sender, EventArgs e)
        {
            var modal = new DocumentsModal(SourceWindow, selectionMode: true);
            ResponseType response = (ResponseType)modal.Run();

            if (response == ResponseType.Ok && modal.Page.SelectedEntity != null)
            {
                var selectedDocument = modal.Page.SelectedEntity;
                var docType = GetDocumentTypeFromDocument(selectedDocument);

                if (docType == null)
                {
                    CustomAlerts.Warning(this.SourceWindow)
                     .WithTitle("Tipo de documento não encontrado")

                     .WithMessage($"O tipo de documento ({selectedDocument.Type}) não foi encontrado nas séries do ano fiscal actual ({FiscalYearService.CurrentFiscalYear.Acronym}).")
                     .ShowAlert();

                    docType = SelectDocumentType();

                    if (docType == null)
                    {
                        modal.Destroy();
                        return;
                    }
                }

                TxtDocumentType.SelectedEntity = docType;
                TxtDocumentType.Text = docType.Designation;

                UpdateValidatableFields();

                TxtCopyDocument.Text = modal.Page.SelectedEntity.Number;
                TxtCopyDocument.SelectedEntity = modal.Page.SelectedEntity;

                TxtPaymentCondition.SelectedEntity = modal.Page.SelectedEntity.PaymentCondition;
                TxtPaymentCondition.Text = modal.Page.SelectedEntity.PaymentCondition?.Designation;

                TxtCurrency.SelectedEntity = modal.Page.SelectedEntity.Currency;
                TxtCurrency.Text = modal.Page.SelectedEntity.Currency.Designation;

                CopyDocumentSelected?.Invoke(modal.Page.SelectedEntity);
            }

            modal.Destroy();
        }

        private void BtnSelectOriginDocument_Clicked(object sender, EventArgs e)
        {
            var modal = new DocumentsModal(SourceWindow, selectionMode: true);
            ResponseType response = (ResponseType)modal.Run();

            if (response == ResponseType.Ok && modal.Page.SelectedEntity != null)
            {
                TxtOriginDocument.Text = modal.Page.SelectedEntity.Number;
                TxtOriginDocument.SelectedEntity = modal.Page.SelectedEntity;
                OriginDocumentSelected?.Invoke(modal.Page.SelectedEntity);
            }

            modal.Destroy();
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
                TxtExchangeRate.Entry.Sensitive = selectedCurrency.Id != CompanyCurrency.Id;

                if (selectedCurrency.Id != CompanyCurrency.Id)
                {
                    TxtExchangeRate.Text = selectedCurrency.ExchangeRate.ToString();
                }
                else
                {
                    TxtExchangeRate.Text = "1";
                }
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

        private void BtnSelectDocumentType_Clicked(object sender, System.EventArgs e)
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

        private DocumentType SelectDocumentType()
        {
            var page = new DocumentTypesPage(this.SourceWindow, PageOptions.SelectionPageOptions);
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
