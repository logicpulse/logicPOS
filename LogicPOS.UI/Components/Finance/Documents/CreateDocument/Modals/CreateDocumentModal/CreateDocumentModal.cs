using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Countries.GetAllCountries;
using LogicPOS.Api.Features.Documents.AddDocument;
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
        private readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();
        private static IEnumerable<Country> _countries;

        public CreateDocumentModal(Window parent) : base(parent: parent,
                                                         title: GeneralUtils.GetResourceByName("window_title_dialog_new_finance_document"),
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

        private void AddEventHandlers()
        {
            BtnOk.Clicked += BtnOk_Clicked;
            BtnPreview.Clicked += BtnPreview_Clicked;
            BtnClear.Clicked += BtnClear_Clicked;
        }

        private void AddTabsEventHandlers()
        {
            DocumentTab.OriginDocumentSelected += OnOriginDocumentSelected;
            DocumentTab.DocumentTypeSelected += OnDocumentTypeSelected;
            DocumentTab.CopyDocumentSelected += OnCopyDocumentSelected;
            ArticlesTab.ItemsPage.OnTotalChanged += PaymentMethodsTab.PaymentMethodsBox.UpdateDocumentTotal;
        }

        private AddDocumentCommand CreateAddCommand()
        {
            var command = new AddDocumentCommand();

            var documentType = DocumentTab.GetDocumentType();

            var analyzer = documentType.Analyzer;

            if (analyzer.IsInvoiceReceipt() || analyzer.IsSimplifiedInvoice())
            {
                command.PaymentMethods = PaymentMethodsTab.PaymentMethodsBox.GetPaymentMethods();
            }

            command.Type = DocumentTab.GetDocumentType().Acronym;
            command.PaymentConditionId = DocumentTab.GetPaymentCondition()?.Id;
            command.CurrencyId = DocumentTab.GetCurrency().Id;
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
            command.Details = ArticlesTab.GetDocumentDetails(customer?.PriceType?.EnumValue);

            if (analyzer.IsGuide())
            {
                command.ShipToAdress = ShipToTab.GetAddress();
                command.ShipFromAdress = ShipFromTab.GetAddress();
            }

            return command;
        }

        public bool AllTabsAreValid() => GetValidatableTabs().All(tab => tab.IsValid());

        public IEnumerable<IValidatableField> GetValidatableTabs()
        {
            var validatableTabs = new List<IValidatableField>
            {
                DocumentTab,
                CustomerTab,
                ArticlesTab
            };

            var documentType = DocumentTab.GetDocumentType();

            if (documentType == null)
            {
                return validatableTabs;
            }

            var analyzer = documentType.Analyzer;

            if (analyzer.IsInvoiceReceipt() || analyzer.IsInvoiceReceipt())
            {
                validatableTabs.Add(PaymentMethodsTab);
            }

            if (analyzer.IsGuide())
            {
                validatableTabs.Add(ShipToTab);
                validatableTabs.Add(ShipFromTab);
            }

            return validatableTabs;
        }

        protected void ShowValidationErrors() => ValidationUtilities.ShowValidationErrors(GetValidatableTabs());

        public static IEnumerable<Country> GetCountries()
        {
            if (_countries == null)
            {
                var getResult = DependencyInjection.Services.GetRequiredService<IMediator>().Send(new GetAllCountriesQuery()).Result;

                if (getResult.IsError)
                {
                    return Enumerable.Empty<Country>();
                }

                _countries = getResult.Value;
            }

            return _countries;
        }

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
    }
}
