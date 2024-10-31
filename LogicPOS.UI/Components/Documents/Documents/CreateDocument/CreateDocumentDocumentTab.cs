using ExcelDataReader.Log.Logger;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Company.GetCompanyCurreny;
using LogicPOS.Api.Features.DocumentTypes.GetAllDocumentTypes;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.Pages;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Patagames.Pdf.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public class CreateDocumentDocumentTab : ModalTab
    {
        private readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<ISender>();
        public IEnumerable<DocumentType> DocumentTypes { get; private set; }
        public PageTextBox TxtDocumentType { get; set; }
        public PageTextBox TxtPaymentCondition { get; set; }
        public PageTextBox TxtPaymentMethod { get; set; }
        public PageTextBox TxtCurrency { get; set; }
        public Currency CompanyCurrency { get; private set; }
        public PageTextBox TxtExchangeRate { get; private set; }
        public PageTextBox TxtOriginDocument { get; set; }
        public PageTextBox TxtCopyDocument { get; set; }
        public PageTextBox TxtNotes { get; set; }

        public event Action<Document> OriginDocumentSelected;
        public event Action<DocumentType> DocumentTypeSelected;
        public event Action<Document> CopyDocumentSelected;

        public CreateDocumentDocumentTab(Window parent) : base(parent: parent,
                                                  name: GeneralUtils.GetResourceByName("window_title_dialog_document_finance_page1"),
                                                  icon: PathsSettings.ImagesFolderLocation + @"Icons/Dialogs/DocumentFinanceDialog/icon_pos_dialog_toolbar_1_new_document.png")
        {
            Initialize();
            Design();
        }

        private void Initialize()
        {
            DocumentTypes = GetDocumentTypes();
            InitializeTxtDocumentType();
            InitializeTxtPaymnentCondition();
            InitializeTxtPaymentMethod();
            CompanyCurrency = GetCompanyCurreny();
            InitializeTxtCurrency();
            InitializeTxtExchangeRate();
            InitializeTxtOriginDocument();
            InitializeTxtCopyDocument();
            InitializeTxtNotes();
            UpdateValidatableFields();
        }

        private IEnumerable<DocumentType> GetDocumentTypes()
        {
            var mediator = DependencyInjection.Services.GetRequiredService<ISender>();
            var documentTypes = mediator.Send(new GetAllDocumentTypesQuery()).Result;

            if (documentTypes.IsError)
            {
                SimpleAlerts.ShowApiErrorAlert(this.SourceWindow, documentTypes.FirstError);
                return Enumerable.Empty<DocumentType>();
            }

            return documentTypes.Value;
        }

        private void InitializeTxtNotes()
        {
            TxtNotes = new PageTextBox(SourceWindow,
                                       GeneralUtils.GetResourceByName("global_notes"),
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true);

            TxtNotes.Entry.IsEditable = true;
        }

        private void InitializeTxtCopyDocument()
        {
            TxtCopyDocument = new PageTextBox(SourceWindow,
                                              GeneralUtils.GetResourceByName("global_copy_finance_document"),
                                              isRequired: false,
                                              isValidatable: false,
                                              includeSelectButton: true,
                                              includeKeyBoardButton: false);

            TxtCopyDocument.Entry.IsEditable = false;

            TxtCopyDocument.SelectEntityClicked += BtnSelectCopyDocument_Clicked;
        }

        private DocumentType GetDefaultDocumentType()
        {
            return DocumentTypes.FirstOrDefault(type => type.Acronym == "FT");
        }

        private void BtnSelectCopyDocument_Clicked(object sender, EventArgs e)
        {
            var page = new DocumentsPage(null, PageOptions.SelectionPageOptions);
            var selectModal = new EntitySelectionModal<Document>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectModal.Run();
            selectModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                var docType = GetDocumentTypeFromDocument(page.SelectedEntity);
                TxtDocumentType.SelectedEntity = docType;
                TxtDocumentType.Text = docType.Designation;

                UpdateValidatableFields();

                TxtCopyDocument.Text = page.SelectedEntity.Number;
                TxtCopyDocument.SelectedEntity = page.SelectedEntity;

                TxtPaymentMethod.SelectedEntity = page.SelectedEntity.PaymentMethod;
                TxtPaymentMethod.Text = page.SelectedEntity.PaymentMethod?.Designation;

                TxtPaymentCondition.SelectedEntity = page.SelectedEntity.PaymentCondition;
                TxtPaymentCondition.Text = page.SelectedEntity.PaymentCondition?.Designation;

                TxtCurrency.SelectedEntity = page.SelectedEntity.Currency;
                TxtCurrency.Text = page.SelectedEntity.Currency.Designation;

                CopyDocumentSelected?.Invoke(page.SelectedEntity);
            }
        }

        private DocumentType GetDocumentTypeFromDocument(Document document)
        {
            return DocumentTypes.FirstOrDefault(type => type.Acronym == document.Type);
        }

        private void InitializeTxtOriginDocument()
        {
            TxtOriginDocument = new PageTextBox(SourceWindow,
                                                GeneralUtils.GetResourceByName("global_source_finance_document"),
                                                isRequired: false,
                                                isValidatable: false,
                                                includeSelectButton: true,
                                                includeKeyBoardButton: false);

            TxtOriginDocument.Entry.IsEditable = false;

            TxtOriginDocument.SelectEntityClicked += BtnSelectOriginDocument_Clicked;
        }

        private void BtnSelectOriginDocument_Clicked(object sender, EventArgs e)
        {
            var page = new DocumentsPage(null, PageOptions.SelectionPageOptions);
            var selectModal = new EntitySelectionModal<Document>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectModal.Run();
            selectModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtOriginDocument.Text = page.SelectedEntity.Number;
                TxtOriginDocument.SelectedEntity = page.SelectedEntity;
                OriginDocumentSelected?.Invoke(page.SelectedEntity);
            }
        }

        private void InitializeTxtCurrency()
        {
            TxtCurrency = new PageTextBox(SourceWindow,
                                          GeneralUtils.GetResourceByName("global_currency"),
                                          isRequired: true,
                                          isValidatable: false,
                                          includeSelectButton: true,
                                          includeKeyBoardButton: false);

            TxtCurrency.Entry.IsEditable = false;


            if (CompanyCurrency != null)
            {
                TxtCurrency.Text = CompanyCurrency.Designation;
                TxtCurrency.SelectedEntity = CompanyCurrency;
            }

            TxtCurrency.SelectEntityClicked += BtnSelectCurrency_Clicked;
        }

        private Currency GetCompanyCurreny()
        {
            var getCurrency = _mediator.Send(new GetCompanyCurrencyQuery()).Result;

            if (getCurrency.IsError)
            {
                SimpleAlerts.ShowApiErrorAlert(this.SourceWindow, getCurrency.FirstError);
                return null;
            }

            return getCurrency.Value;
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
                var selectedCurrency = page.SelectedEntity as Currency;
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

        private void InitializeTxtExchangeRate()
        {
            TxtExchangeRate = new PageTextBox(SourceWindow,
                                              GeneralUtils.GetResourceByName("global_exchangerate"),
                                              isRequired: true,
                                              isValidatable: true,
                                              regex: RegularExpressions.DecimalNumber,
                                              includeSelectButton: false,
                                              includeKeyBoardButton: true);
            TxtExchangeRate.Text = "1.00";
            TxtExchangeRate.Entry.Sensitive = false;
        }

        private void InitializeTxtPaymentMethod()
        {
            TxtPaymentMethod = new PageTextBox(SourceWindow,
                                               GeneralUtils.GetResourceByName("global_payment_method"),
                                               isRequired: true,
                                               isValidatable: false,
                                               includeSelectButton: true,
                                               includeKeyBoardButton: false);

            TxtPaymentMethod.Entry.IsEditable = false;

            TxtPaymentMethod.SelectEntityClicked += BtnSelectPaymentMethod_Clicked;
        }

        private void InitializeTxtTotalPaid()
        {
            TxtPaymentMethod = new PageTextBox(SourceWindow,
                                               GeneralUtils.GetResourceByName("global_payment_method"),
                                               isRequired: true,
                                               isValidatable: false,
                                               includeSelectButton: true,
                                               includeKeyBoardButton: false);

            TxtPaymentMethod.Entry.IsEditable = false;

            TxtPaymentMethod.SelectEntityClicked += BtnSelectPaymentMethod_Clicked;
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

        private void InitializeTxtPaymnentCondition()
        {
            TxtPaymentCondition = new PageTextBox(SourceWindow,
                                                   GeneralUtils.GetResourceByName("global_payment_condition"),
                                                   isRequired: true,
                                                   isValidatable: false,
                                                   includeSelectButton: true,
                                                   includeKeyBoardButton: false);

            TxtPaymentCondition.Entry.IsEditable = false;

            TxtPaymentCondition.SelectEntityClicked += BtnSelectPaymentCondition_Clicked;
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

        private void InitializeTxtDocumentType()
        {
            TxtDocumentType = new PageTextBox(SourceWindow,
                                              GeneralUtils.GetResourceByName("global_documentfinanceseries_documenttype"),
                                              isRequired: true,
                                              isValidatable: false,
                                              includeSelectButton: true,
                                              includeKeyBoardButton: false);

            TxtDocumentType.SelectedEntity = GetDefaultDocumentType();
            TxtDocumentType.Text = (TxtDocumentType.SelectedEntity as DocumentType).Designation;

            TxtDocumentType.Entry.IsEditable = false;

            TxtDocumentType.SelectEntityClicked += BtnSelectDocumentType_Clicked;
        }

        private void BtnSelectDocumentType_Clicked(object sender, System.EventArgs e)
        {
            var page = new DocumentTypesPage(null, PageOptions.SelectionPageOptions);
            var selectDocumentTypeModal = new EntitySelectionModal<DocumentType>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectDocumentTypeModal.Run();
            selectDocumentTypeModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtDocumentType.Text = page.SelectedEntity.Designation;
                TxtDocumentType.SelectedEntity = page.SelectedEntity;
                UpdateValidatableFields();
                DocumentTypeSelected?.Invoke(page.SelectedEntity);
            }
        }

        private void Design()
        {
            var verticalLayout = new VBox(false, 2);
            verticalLayout.PackStart(TxtDocumentType.Component, false, false, 0);
            verticalLayout.PackStart(TxtPaymentCondition.Component, false, false, 0);
            verticalLayout.PackStart(TxtPaymentMethod.Component, false, false, 0);
            verticalLayout.PackStart(PageTextBox.CreateHbox(TxtCurrency, TxtExchangeRate), false, false, 0);

            verticalLayout.PackStart(PageTextBox.CreateHbox(TxtOriginDocument, TxtCopyDocument), false, false, 0);

            verticalLayout.PackStart(TxtNotes.Component, false, false, 0);

            PackStart(verticalLayout);
        }

        public PaymentMethod GetPaymentMethod()
        {
            return TxtPaymentMethod.SelectedEntity as PaymentMethod;
        }

        public PaymentCondition GetPaymentCondition()
        {
            return TxtPaymentCondition.SelectedEntity as PaymentCondition;
        }

        public Currency GetCurrency()
        {
            return TxtCurrency.SelectedEntity as Currency;
        }

        public DocumentType GetDocumentType()
        {
            return TxtDocumentType.SelectedEntity as DocumentType;
        }

        public override bool IsValid()
        {
            if (TxtDocumentType.SelectedEntity == null)
            {
                return false;
            }

            return TxtPaymentCondition.IsValid() &&
                   TxtPaymentMethod.IsValid() &&
                   TxtCurrency.IsValid() &&
                   TxtExchangeRate.IsValid() &&
                   TxtOriginDocument.IsValid() &&
                   TxtCopyDocument.IsValid() &&
                   TxtNotes.IsValid();
        }

        public Guid? GetOriginDocumentId()
        {
            return (TxtOriginDocument.SelectedEntity as Document)?.Id;
        }

        private void UpdateValidatableFields()
        {
            var documentType = GetDocumentType();

            if (documentType.IsGuide())
            {
                TxtOriginDocument.Require(true);
                TxtPaymentCondition.Require(false, false);
                TxtPaymentMethod.Require(false, false);
                TxtNotes.Require(false);
            }
            else if (documentType.IsInformative())
            {
                TxtOriginDocument.Require(false, false);
                TxtPaymentCondition.Require(true);
                TxtPaymentMethod.Require(false, false);
                TxtNotes.Require(false);

            }
            else if (documentType.IsInvoice())
            {
                TxtOriginDocument.Require(false, false);
                TxtPaymentCondition.Require(true);
                TxtPaymentMethod.Require(false, false);
                TxtNotes.Require(false);

            }
            else if (documentType.IsInvoiceReceipt())
            {
                TxtOriginDocument.Require(false, false);
                TxtPaymentCondition.Require(false, false);
                TxtPaymentMethod.Require(true);
                TxtNotes.Require(false);
            }

            else if (documentType.IsCreditNote())
            {
                TxtOriginDocument.Require(true);
                TxtPaymentCondition.Require(false, false);
                TxtPaymentMethod.Require(false, false);
                TxtNotes.Require(true);
            }
        }

        public decimal GetExchangeRate()
        {
            var selectedCurrency = TxtCurrency.SelectedEntity as Currency;
            if(selectedCurrency.Id == CompanyCurrency.Id)
            {
                return 1;
            }

            return decimal.Parse(TxtExchangeRate.Text);
        }
    }
}
