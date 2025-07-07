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
        private IconButtonWithText BtnClear { get; set; } = ActionAreaButton.FactoryGetDialogButtonType("touchButtonClearCustomer_DialogActionArea",
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
        public CheckButton CheckIsDraft { get; private set; } = new CheckButton("Guardar como Rascunho") { };
    }
}
