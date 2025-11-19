using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Finance.Documents.Types.Common;
using LogicPOS.Globalization;
using LogicPOS.UI.Components.Finance.Currencies;
using LogicPOS.UI.Components.Finance.DocumentTypes;
using LogicPOS.UI.Components.Finance.PaymentConditions;
using LogicPOS.UI.Components.Finance.PaymentMethods;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Services;
using LogicPOS.Utility;
using System.Linq;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public partial class DocumentTab
    {
        private void Initialize()
        {
            InitializeTxtDocumentType();
            InitializeTxtPaymnentCondition();
            if (SinglePaymentMethod)
            {
                InitializeTxtPaymnentMethod();
            }
            InitializeTxtCurrency();
            InitializeTxtOriginDocument();
            InitializeTxtCopyDocument();
            InitializeTxtNotes();

            UpdateValidatableFields();
        }

        private void InitializeTxtPaymnentMethod()
        {
            TxtPaymentMethod = new TextBox(SourceWindow,
                                               GeneralUtils.GetResourceByName("global_payment_method"),
                                               isRequired: true,
                                               isValidatable: false,
                                               includeSelectButton: true,
                                               includeKeyBoardButton: false);

            TxtPaymentMethod.Entry.IsEditable = true;
            TxtPaymentMethod.WithAutoCompletion(PaymentMethodsService.AutocompleteLines, id => PaymentMethodsService.GetBydId(id));
            TxtPaymentMethod.Entry.Changed += TxtPaymentMethod_Changed;
            TxtPaymentMethod.SelectEntityClicked += BtnSelectPaymentMethod_Clicked;
        }

        private void InitializeTxtNotes()
        {
            TxtNotes = new TextBox(SourceWindow,
                                       LocalizedString.Instance["global_notes"],
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true);
        }

        private void InitializeTxtCopyDocument()
        {
            TxtCopyDocument = new TextBox(SourceWindow,
                                              GeneralUtils.GetResourceByName("global_copy_finance_document"),
                                              isRequired: false,
                                              isValidatable: false,
                                              includeSelectButton: true,
                                              includeKeyBoardButton: false);

            TxtCopyDocument.Entry.IsEditable = false;

            TxtCopyDocument.SelectEntityClicked += BtnSelectCopyDocument_Clicked;
        }

        private void InitializeTxtCurrency()
        {
            TxtCurrency = new TextBox(SourceWindow,
                                          GeneralUtils.GetResourceByName("global_currency"),
                                          isRequired: true,
                                          isValidatable: false,
                                          includeSelectButton: true,
                                          includeKeyBoardButton: false);

            TxtCurrency.Entry.IsEditable = false;

            TxtCurrency.Text = CurrenciesService.Default.Designation;
            TxtCurrency.SelectedEntity = CurrenciesService.Default;
            TxtCurrency.SelectEntityClicked += BtnSelectCurrency_Clicked;

            if (SystemInformationService.SystemInformation.IsAngola)
            {
                TxtCurrency.BtnSelect.Sensitive = TxtCurrency.Entry.Sensitive = false;
            }
        }

        private void InitializeTxtPaymnentCondition()
        {
            TxtPaymentCondition = new TextBox(SourceWindow,
                                                   GeneralUtils.GetResourceByName("global_payment_condition"),
                                                   isRequired: true,
                                                   isValidatable: false,
                                                   includeSelectButton: true,
                                                   includeKeyBoardButton: false);

            TxtPaymentCondition.Entry.IsEditable = true;
            TxtPaymentCondition.WithAutoCompletion(PaymentConditionsService.AutocompleteLines,id => PaymentConditionsService.GetById(id));
            TxtPaymentCondition.OnCompletionSelected += p => SelectPaymentCondition(p as PaymentCondition);
            TxtPaymentCondition.Entry.Changed += TxtPaymentCondition_Changed;
            TxtPaymentCondition.SelectEntityClicked += BtnSelectPaymentCondition_Clicked;
        }

        private void InitializeTxtDocumentType()
        {
            TxtDocumentType = new TextBox(SourceWindow,
                                              GeneralUtils.GetResourceByName("global_documentfinanceseries_documenttype"),
                                              isRequired: true,
                                              isValidatable: false,
                                              includeSelectButton: true,
                                              includeKeyBoardButton: false);

            TxtDocumentType.SelectedEntity = DocumentTypesService.Default;
            TxtDocumentType.Text = (TxtDocumentType.SelectedEntity as DocumentType).Designation;
            TxtDocumentType.Entry.IsEditable = true;
            TxtDocumentType.WithAutoCompletion(DocumentTypesService.AutocompleteLines, id => DocumentTypesService.GetById(id));
            TxtDocumentType.OnCompletionSelected += d => SelectDocumentType(d as DocumentType);
            TxtDocumentType.Entry.Changed += TxtDocumentType_Changed;
            TxtDocumentType.SelectEntityClicked += BtnSelectDocumentType_Clicked;
        }

        private void InitializeTxtOriginDocument()
        {
            TxtOriginDocument = new TextBox(SourceWindow,
                                                GeneralUtils.GetResourceByName("global_source_finance_document"),
                                                isRequired: false,
                                                isValidatable: false,
                                                includeSelectButton: true,
                                                includeKeyBoardButton: false);

            TxtOriginDocument.Entry.IsEditable = false;

            TxtOriginDocument.SelectEntityClicked += BtnSelectOriginDocument_Clicked;
        }


    }
}
