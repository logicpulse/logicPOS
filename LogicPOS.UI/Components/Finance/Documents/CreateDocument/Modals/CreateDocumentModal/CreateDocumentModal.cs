using Gtk;
using LogicPOS.Api.Features.Documents.Documents.GetDocumentPreviewPdf;
using LogicPOS.Api.Features.Finance.Documents.Documents.Common;
using LogicPOS.Api.Features.Finance.Documents.Documents.IssueDocument;
using LogicPOS.Api.Features.Finance.Documents.Types.Common;
using LogicPOS.Globalization;
using LogicPOS.UI.Components.Documents.CreateDocument;
using LogicPOS.UI.Components.Finance.Documents.Services;
using LogicPOS.UI.Components.Finance.DocumentTypes;
using LogicPOS.UI.Components.FiscalYears;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using System;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class CreateDocumentModal : Modal
    {
        private Guid? _draftId = null;
        private bool DraftMode => CheckIsDraft.Active;

        private CreateDocumentModal(Window parent) : base(parent: parent,
                                                         title: LocalizedString.Instance["window_title_dialog_new_finance_document"],
                                                         size: new global::System.Drawing.Size(790, 546),
                                                         icon: AppSettings.Paths.Images + @"Icons\Windows\icon_window_document_new.png")
        {
            Initialize();
            Navigator.UpdateUI();
            UpdateUI();
        }

        public void ImportDraftData(DocumentViewModel document)
        {
            _draftId = document.Id;
            CheckIsDraft.Active = true;
            var fullDocument = DocumentsService.GetDocument(document.Id);
            CustomerTab.ImportDataFromDocument(fullDocument);
            DetailsTab.ImportDataFromDocument(document.Id, fullDocument.Discount);

            if (document.TypeAnalyzer.IsWayBill())
            {
                ShipFromTab.ImportDataFromDocument(fullDocument);
                ShipToTab.ImportDataFromDocument(fullDocument);
            }

            if (document.TypeAnalyzer.IsWayBill())
            {
                ShipFromTab.ImportDataFromDocument(fullDocument);
                ShipToTab.ImportDataFromDocument(fullDocument);
            }

            var documentType = DocumentTypesService.GetActive().Where(docType => docType.Acronym == document.Type).FirstOrDefault()
                ?? DocumentTypesService.Default;


            this.DocumentTab.SelectDocumentType(documentType);

            this.DocumentTab.TxtPaymentCondition.SelectedEntity = document.PaymentCondition;
            this.DocumentTab.TxtPaymentCondition.Text = document.PaymentCondition?.Designation;
            this.DocumentTab.TxtCurrency.SelectedEntity = document.Currency;
            this.DocumentTab.TxtCurrency.Text = document.Currency.Designation;
            this.DocumentTab.TxtNotes.Text = document.Notes;
        }

        private void Initialize()
        {
            AddEventHandlers();
        }

        private IssueDocumentCommand CreateAddCommand()
        {
            var command = new IssueDocumentCommand();

            var documentType = DocumentTab.GetDocumentType();

            var analyzer = documentType.Analyzer;

            if (analyzer.IsInvoiceReceipt() || analyzer.IsSimplifiedInvoice())
            {
                command.PaymentMethods = SinglePaymentMethod ? DocumentTab.GetPaymentMethods() : PaymentMethodsTab.PaymentMethodsBox.GetPaymentMethods();
            }

            command.Type = DocumentTab.GetDocumentType().Acronym;
            command.PaymentConditionId = DocumentTab.GetPaymentConditionId();
            command.CurrencyId = DocumentTab.GetCurrencyId();
            command.ParentId = DocumentTab.GetOriginDocumentId();
            command.CustomerId = CustomerTab.CustomerId;
            command.Notes = DocumentTab.TxtNotes.Text;
            command.IsDraft = CheckIsDraft.Active;

            var customer = CustomerTab.GetCustomer();

            if (customer != null)
            {
                command.CustomerId = customer.Id;
            }

            command.Customer = CustomerTab.GetDocumentCustomer();
            command.Discount = decimal.Parse(CustomerTab.TxtDiscount.Text);
            command.Details = DetailsTab.GetDocumentDetails();

            if (analyzer.IsWayBill())
            {
                command.ShipToAddress = ShipToTab.GetAddress();
                command.ShipFromAddress = ShipFromTab.GetAddress();
            }

            return command;
        }

        private GetDocumentPreviewPdfQuery CreateDocumentPreviewQuery()
        {
            var query = new GetDocumentPreviewPdfQuery();

            query.CurrencyId = DocumentTab.GetCurrencyId();
            query.Notes = DocumentTab.TxtNotes.Text;
            query.Discount = decimal.Parse(CustomerTab.TxtDiscount.Text);
            query.Details = DetailsTab.GetDocumentDetails();

            return query;
        }

        public static ResponseType ShowModal(Window parent, DocumentViewModel draft = null)
        {
            if (FiscalYearsService.HasActiveFiscalYear() == false)
            {
                FiscalYearsService.ShowOpenFiscalYearAlert(parent);
                return ResponseType.Cancel;
            }

            var modal = new CreateDocumentModal(parent);
            if (draft != null)
            {
                modal.ImportDraftData(draft);
            }

            ResponseType response = (ResponseType)modal.Run();
            modal.Destroy();
            return response;
        }

        private void UpdateTitle()
        {
            decimal total = GetTotalFinal();
            WindowSettings.Title.Text = $"{(DraftMode ? "Rascunho" : LocalizedString.Instance["window_title_dialog_new_finance_document"])} :: {Navigator.CurrentTab.TabName} : {total:C}";
        }

        private void InitializeNavigator()
        {
            InitializeTabs();


            if (SinglePaymentMethod)
            {
                Navigator = new ModalTabsNavigator(DocumentTab,
                                           CustomerTab,
                                           DetailsTab,
                                           ShipToTab,
                                           ShipFromTab);
                return;
            }

            Navigator = new ModalTabsNavigator(DocumentTab,
                                               CustomerTab,
                                               DetailsTab,
                                               PaymentMethodsTab,
                                               ShipToTab,
                                               ShipFromTab);
        }

        private void InitializeTabs()
        {
            DocumentTab = new DocumentTab(this);
            CustomerTab = new CustomerTab(this);
            DetailsTab = new DetailsTab(this);
            ShipToTab = new ShipToTab(this);
            ShipFromTab = new ShipFromTab(this);
            if (SinglePaymentMethod == false)
            {
                PaymentMethodsTab = new PaymentMethodsTab(this);
            }
            AddTabsEventHandlers();
        }

        private void ShowTabsForDocumentType(DocumentType documentType)
        {
            var analyzer = documentType.Analyzer;
            ShipToTab.ShowTab = ShipFromTab.ShowTab = analyzer.IsWayBill();
            if(analyzer.IsWayBill() && CustomerTab.CustomerId.HasValue && CustomerTab.CustomerId!=Guid.Empty)
            {
                ShipToTab.GetCustomerAddress((Guid)CustomerTab.CustomerId);
            }
            if (SinglePaymentMethod == false)
            {
                PaymentMethodsTab.ShowTab = analyzer.IsInvoiceReceipt() || analyzer.IsSimplifiedInvoice();
            }
        }

        private void EnableTabsForDocumentType(DocumentType documentType)
        {
            CustomerTab.Sensitive = documentType.Analyzer.IsCreditNote() == false;
        }

        private decimal GetTotalFinal()
        {
            decimal discount = 0;
            decimal.TryParse(CustomerTab.TxtDiscount.Text, out discount);
            decimal detailsTotal = DetailsTab.TotalFinal;
            decimal totalAfterDiscount = detailsTotal - (detailsTotal * discount / 100);
            return totalAfterDiscount;
        }
    }
}
