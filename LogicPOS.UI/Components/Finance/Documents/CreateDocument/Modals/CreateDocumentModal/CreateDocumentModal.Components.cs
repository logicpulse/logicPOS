using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Documents.CreateDocument;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Modals
{
    public partial class CreateDocumentModal
    {
        #region Buttons
        private IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        private IconButtonWithText BtnClearCustomer { get; set; } = ActionAreaButton.FactoryGetDialogButtonType("touchButtonClearCustomer_DialogActionArea",
                                                                                                                GeneralUtils.GetResourceByName("global_button_label_payment_dialog_clear_client"),
                                                                                                                PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_nav_delete.png");
        private IconButtonWithText BtnPreview { get; set; } = ActionAreaButton.FactoryGetDialogButtonType("touchButtonPreview_DialogActionArea",
                                                                                                          GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_preview"),
                                                                                                          PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_preview.png");

        #endregion

        #region Tabs
        private CreateDocumentDocumentTab DocumentTab { get; set; }
        private CreateDocumentCustomerTab CustomerTab { get; set; }
        private CreateDocumentArticlesTab ArticlesTab { get; set; }
        private CreateDocumentShipToTab ShipToTab { get; set; }
        private CreateDocumentShipFromTab ShipFromTab { get; set; }
        private CreateDocumentPaymentMethodsTab PaymentMethodsTab { get; set; }
        #endregion

        private ModalTabsNavigator Navigator { get; set; }
        public CheckButton CheckIsDraft { get; private set; } = new CheckButton("Gudardar como Rascunho") { };

        private void InitializeNavigator()
        {
            InitializeTabs();

            Navigator = new ModalTabsNavigator(DocumentTab,
                                               CustomerTab,
                                               ArticlesTab,
                                               PaymentMethodsTab,
                                               ShipToTab,
                                               ShipFromTab);
        }

        private void InitializeTabs()
        {
            DocumentTab = new CreateDocumentDocumentTab(this);
            CustomerTab = new CreateDocumentCustomerTab(this);
            ArticlesTab = new CreateDocumentArticlesTab(this);
            ShipToTab = new CreateDocumentShipToTab(this);
            ShipFromTab = new CreateDocumentShipFromTab(this);
            PaymentMethodsTab = new CreateDocumentPaymentMethodsTab(this);
            AddTabsEventHandlers();
        }

        private void ShowTabsForDocumentType(DocumentType documentType)
        {
            var analyzer = documentType.Analyzer;
            ShipToTab.ShowTab = ShipFromTab.ShowTab = analyzer.IsGuide();
            PaymentMethodsTab.ShowTab = analyzer.IsInvoiceReceipt() || analyzer.IsSimplifiedInvoice();
        }

        private void EnableTabsForDocumentType(DocumentType documentType)
        {
            CustomerTab.Sensitive = documentType.Analyzer.IsCreditNote() == false;
        }

        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
            {
                new ActionAreaButton(BtnClearCustomer,  ResponseType.None),
                new ActionAreaButton(BtnPreview, ResponseType.None),
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

        protected override Widget CreateLeftContent()
        {
            var hbox = new HBox(true, 0);
            CheckIsDraft.Child.ModifyFont(Pango.FontDescription.FromString(AppSettings.Instance.fontEntryBoxLabel));
            hbox.PackStart(CheckIsDraft, false, false, 0);
            return hbox;
        }
    }
}
