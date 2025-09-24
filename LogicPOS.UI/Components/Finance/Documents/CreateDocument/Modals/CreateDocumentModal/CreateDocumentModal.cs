using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Countries.GetAllCountries;
using LogicPOS.Api.Features.Documents.AddDocument;
using LogicPOS.Api.Features.Documents.Documents.GetDocumentPreviewPdf;
using LogicPOS.Globalization;
using LogicPOS.UI.Components.FiscalYears;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class CreateDocumentModal : Modal
    {
        
        public CreateDocumentModal(Window parent) : base(parent: parent,
                                                         title: LocalizedString.Instance["window_title_dialog_new_finance_document"],
                                                         size: new System.Drawing.Size(790, 546),
                                                         icon: AppSettings.Paths.Images + @"Icons\Windows\icon_window_document_new.png")
        {
            Initialize();
            Navigator.UpdateUI();
        }

        private void Initialize()
        {
            AddEventHandlers();
        }

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
        }

        private AddDocumentCommand CreateAddCommand()
        {
            var command = new AddDocumentCommand();

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
            command.ExchangeRate = DocumentTab.GetExchangeRate();
            command.IsDraft = CheckIsDraft.Active;

            var customer = CustomerTab.GetCustomer();

            if (customer != null)
            {
                command.CustomerId = customer.Id;
            }

            command.Customer = CustomerTab.GetDocumentCustomer();
            command.Discount = decimal.Parse(CustomerTab.TxtDiscount.Text);
            command.Details = DetailsTab.GetDocumentDetails(customer?.PriceType?.EnumValue);

            if (analyzer.IsGuide())
            {
                command.ShipToAdress = ShipToTab.GetAddress();
                command.ShipFromAdress = ShipFromTab.GetAddress();
            }

            return command;
        }

        private GetDocumentPreviewPdfQuery CreateDocumentPreviewQuery()
        {
            var query = new GetDocumentPreviewPdfQuery();

            query.CurrencyId = DocumentTab.GetCurrencyId();
            query.Notes = DocumentTab.TxtNotes.Text;
            query.ExchangeRate = DocumentTab.GetExchangeRate();
            query.Discount = decimal.Parse(CustomerTab.TxtDiscount.Text);
            query.Details = DetailsTab.GetDocumentDetails(null);

            return query;
        }

        public bool AllTabsAreValid() => GetValidatableTabs().All(tab => tab.IsValid());

        public bool TabsForPreviewAreValid()
        {
            return DetailsTab.IsValid();
        }

        public IEnumerable<IValidatableField> GetValidatableTabs()
        {
            var validatableTabs = new List<IValidatableField>
            {
                DocumentTab,
                CustomerTab,
                DetailsTab
            };

            var docAnalyzer = DocumentTab.DocumentTypeAnalyzer;

            if (docAnalyzer == null)
            {
                return validatableTabs;
            }

            if(SinglePaymentMethod == false)
            {
                if (docAnalyzer.Value.IsInvoiceReceipt() || docAnalyzer.Value.IsSimplifiedInvoice())
                {
                    validatableTabs.Add(PaymentMethodsTab);
                }
            }
         
            if (docAnalyzer.Value.IsGuide())
            {
                validatableTabs.Add(ShipToTab);
                validatableTabs.Add(ShipFromTab);
            }

            return validatableTabs;
        }

        protected void ShowValidationErrors() => ValidationUtilities.ShowValidationErrors(GetValidatableTabs());

        public static void ShowModal(Window parent)
        {
            if (FiscalYearService.HasFiscalYear() == false)
            {
                FiscalYearService.ShowOpenFiscalYearAlert();
                return;
            }

            var modal = new CreateDocumentModal(parent);
            modal.Run();
            modal.Destroy();
        }

        private void UpdateTitle()
        {
            decimal total = DetailsTab.Page.TotalFinal;
            WindowSettings.Title.Text = $"{LocalizedString.Instance["window_title_dialog_new_finance_document"]} :: {Navigator.CurrentTab.TabName} : {total:C}";
        }

    }
}
