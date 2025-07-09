using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Documents.CreateDocument;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.Modals
{
    public partial class CreateDocumentModal
    {
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
                new ActionAreaButton(BtnClear,  ResponseType.None),
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
            CheckIsDraft.Child.ModifyFont(Pango.FontDescription.FromString(AppSettings.Instance.FontEntryBoxLabel));
            hbox.PackStart(CheckIsDraft, false, false, 0);
            return hbox;
        }
    }
}
