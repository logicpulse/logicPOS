using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Company.GetCompanyCurreny;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Finance.DocumentTypes;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System;
using System.Linq;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public partial class CreateDocumentDocumentTab : ModalTab
    {
        public CreateDocumentDocumentTab(Window parent) : base(parent: parent,
                                                  name: GeneralUtils.GetResourceByName("window_title_dialog_document_finance_page1"),
                                                  icon: AppSettings.Paths.Images + @"Icons/Dialogs/DocumentFinanceDialog/icon_pos_dialog_toolbar_1_new_document.png")
        {
            Initialize();
            Design();
        }

        private void Initialize()
        {
            DocumentTypes = DocumentTypesService.GetAllDocumentTypes();
            InitializeTxtDocumentType();
            InitializeTxtPaymnentCondition();
            CompanyCurrency = GetCompanyCurreny();
            InitializeTxtCurrency();
            InitializeTxtExchangeRate();
            InitializeTxtOriginDocument();
            InitializeTxtCopyDocument();
            InitializeTxtNotes();
            UpdateValidatableFields();
        }

        private DocumentType GetDefaultDocumentType()
        {
            return DocumentTypes.FirstOrDefault(type => type.Acronym == "FT");
        }

        private DocumentType GetDocumentTypeFromDocument(Document document)
        {
            return DocumentTypes.FirstOrDefault(type => type.Acronym == document.Type);
        }
        
        private Currency GetCompanyCurreny()
        {
            var getCurrency = _mediator.Send(new GetCompanyCurrencyQuery()).Result;

            if (getCurrency.IsError)
            {
                CustomAlerts.ShowApiErrorAlert(this.SourceWindow, getCurrency.FirstError);
                return null;
            }

            return getCurrency.Value;
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
            var analyzer = documentType.Analyzer;

            if (analyzer.IsGuide())
            {
                TxtOriginDocument.Require(false);
                TxtPaymentCondition.Require(false, false);
                TxtNotes.Require(false);
            }
            else if (analyzer.IsInformative())
            {
                TxtOriginDocument.Require(false, false);
                TxtPaymentCondition.Require(true);
                TxtNotes.Require(false);

            }
            else if (analyzer.IsInvoice())
            {
                TxtOriginDocument.Require(false);
                TxtPaymentCondition.Require(true);
                TxtNotes.Require(false);

            }
            else if (analyzer.IsInvoiceReceipt())
            {
                TxtOriginDocument.Require(false, false);
                TxtPaymentCondition.Require(false, false);
                TxtNotes.Require(false);
            }

            else if (analyzer.IsCreditNote())
            {
                TxtOriginDocument.Require(true);
                TxtPaymentCondition.Require(false, false);
                TxtNotes.Require(true);
            }
        }
        
        public decimal GetExchangeRate()
        {
            var selectedCurrency = TxtCurrency.SelectedEntity as Currency;
            if (selectedCurrency.Id == CompanyCurrency.Id)
            {
                return 1;
            }

            return decimal.Parse(TxtExchangeRate.Text);
        }
    }
}
