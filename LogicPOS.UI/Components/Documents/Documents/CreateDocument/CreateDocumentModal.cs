using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents.AddDocument;
using LogicPOS.Api.Features.Documents.Documents.GetDocumentPdf;
using LogicPOS.Api.Features.Documents.Series.GetAllDocumentSeries;
using LogicPOS.PDFViewer.Winforms;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Documents;
using LogicPOS.UI.Components.Documents.CreateDocument;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public class CreateDocumentModal : Modal
    {
        private IEnumerable<DocumentSeries> _documentSeries;
        private readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();

        #region Buttons
        private IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        private IconButtonWithText BtnClearCustomer { get; set; } = ActionAreaButton.FactoryGetDialogButtonType("touchButtonClearCustomer_DialogActionArea",
                                                                                                                GeneralUtils.GetResourceByName("global_button_label_payment_dialog_clear_client"),
                                                                                                                PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_nav_delete.png");
        private IconButtonWithText BtnPreview { get; set; } = ActionAreaButton.FactoryGetDialogButtonType("touchButtonPreview_DialogActionArea",
                                                                                                          GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_preview"),
                                                                                                          PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_preview.png");

        string BtnPreviewIcon => PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_preview.png";
        string BtnClearCustomerIcon => PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_nav_delete.png";
        #endregion

        #region Tabs
        private CreateDocumentDocumentTab DocumentTab { get; set; }
        private CreateDocumentCustomerTab CustomerTab { get; set; }
        private CreateDocumentArticlesTab ArticlesTab { get; set; }
        private CreateDocumentShipToTab ShipToTab { get; set; }
        private CreateDocumentShipFromTab ShipFromTab { get; set; }
        #endregion

        private ModalTabsNavigator Navigator { get; set; }

        public CreateDocumentModal(Window parent) : base(parent: parent,
                                                         title: GeneralUtils.GetResourceByName("window_title_dialog_new_finance_document"),
                                                         size: new System.Drawing.Size(790, 546),
                                                         icon: PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_document_new.png")
        {
            Initialize();
        }

        private void Initialize()
        {
            _documentSeries = GetDocumentSeries();
            AddEventHandlers();
        }

        private void AddEventHandlers()
        {
            BtnOk.Clicked += BtnOk_Clicked;
        }

        private void BtnOk_Clicked(object sender, EventArgs e)
        {
            if (!AllTabsAreValid())
            {
                ShowValidationErrors();
                Run();
                return;
            }

            var command = CreateAddCommand();
            var result = _mediator.Send(command).Result;

            if (result.IsError)
            {
                SimpleAlerts.ShowApiErrorAlert(this,result.FirstError);
                return;
            }

            DocumentPrintingUtils.ShowPdf(this, result.Value);
        }

        private IEnumerable<DocumentSeries> GetDocumentSeries()
        {
            var getResult = _mediator.Send(new GetAllDocumentSeriesQuery()).Result;

            if (getResult.IsError)
            {
                SimpleAlerts.ShowApiErrorAlert(this, getResult.FirstError);
                return Enumerable.Empty<DocumentSeries>();
            }

            return getResult.Value;
        }     

        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
            {
                new ActionAreaButton(BtnClearCustomer,  (ResponseType)12),
                new ActionAreaButton(BtnPreview, (ResponseType)11),
                new ActionAreaButton(BtnOk, ResponseType.Ok),
                new ActionAreaButton(BtnCancel, ResponseType.Cancel)
            };

            return actionAreaButtons;
        }

        protected override Widget CreateBody()
        {
            InitializeNavigator();
            VBox box = new VBox();
            box.PackStart(Navigator, true, true, 0);
            return box;
        }

        private void InitializeNavigator()
        {
            InitializeTabs();

            Navigator = new ModalTabsNavigator(DocumentTab,
                                               CustomerTab,
                                               ArticlesTab,
                                               ShipToTab,
                                               ShipFromTab);
        }

        private void InitializeTabs()
        {
            DocumentTab = new CreateDocumentDocumentTab(this);
            DocumentTab.OriginDocumentSelected += OnOriginDocumentSelected;
            DocumentTab.DocumentTypeSelected += OnDocumentTypeSelected;
            DocumentTab.CopyDocumentSelected += OnCopyDocumentSelected;
            CustomerTab = new CreateDocumentCustomerTab(this);
            ArticlesTab = new CreateDocumentArticlesTab(this);
            ShipToTab = new CreateDocumentShipToTab(this);
            ShipFromTab = new CreateDocumentShipFromTab(this);
        }

        private void OnDocumentTypeSelected(DocumentType documentType)
        {
            EnableAllTabs();

            if (documentType.IsGuide())
            {
                CustomerTab.Disable();
            }
        }

        private void EnableAllTabs()
        {
            Navigator.Tabs.ForEach(t => t.Enable());
        }

        private void OnOriginDocumentSelected(Document document)
        {
            CustomerTab.ImportDataFromDocument(document);
            ArticlesTab.ImportDataFromDocument(document);
        }

        private void OnCopyDocumentSelected(Document document)
        {
            EnableAllTabs();

            CustomerTab.ImportDataFromDocument(document);
            ArticlesTab.ImportDataFromDocument(document);

            if(document.IsGuide())
            {
                ShipFromTab.ImportDataFromDocument(document);
                ShipToTab.ImportDataFromDocument(document);
            }
        }

        private AddDocumentCommand CreateAddCommand()
        {
            var command = new AddDocumentCommand();

            command.PaymentMethodId = DocumentTab.GetPaymentMethod()?.Id;
            command.SeriesId = GetSeriesFromDocumentType().Id;
            command.PaymentConditionId = DocumentTab.GetPaymentCondition()?.Id;
            command.CurrencyId = DocumentTab.GetCurrency().Id;
            command.ParentId = DocumentTab.GetOriginDocumentId();
            command.CustomerId = CustomerTab.CustomerId;

            var customer = CustomerTab.GetCustomer();

            if (customer != null)
            {
                command.CustomerId = customer.Id;
            }

            command.Customer = CustomerTab.GetDocumentCustomer();
            command.Discount = decimal.Parse(CustomerTab.TxtDiscount.Text);
            command.Details = ArticlesTab.GetDocumentDetails(customer?.PriceType?.EnumValue);

            if (DocumentTab.GetDocumentType().IsGuide())
            {
                command.ShipToAdress = ShipToTab.GetAddress();
                command.ShipFromAdress = ShipFromTab.GetAddress();
            }

            return command;
        }

        public DocumentSeries GetSeriesFromDocumentType()
        {
            var documentType = DocumentTab.GetDocumentType();
            return _documentSeries.First(series => series.DocumentType.Id == documentType.Id);
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



            if (documentType != null && documentType.IsGuide())
            {
                validatableTabs.Add(ShipToTab);
                validatableTabs.Add(ShipFromTab);
            }

            return validatableTabs;
        }

        protected void ShowValidationErrors() => Utilities.ShowValidationErrors(GetValidatableTabs());
    }
}
