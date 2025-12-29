using Gtk;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Documents.CreateDocument;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Services;
using LogicPOS.UI.Settings;
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
                                                                                                                AppSettings.Paths.Images + @"Icons\icon_pos_nav_delete.png");
        private IconButtonWithText BtnPreview { get; set; } = ActionAreaButton.FactoryGetDialogButtonType("touchButtonPreview_DialogActionArea",
                                                                                                          GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_preview"),
                                                                                                          AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_preview.png");

        #endregion

        #region Tabs
        private DocumentTab DocumentTab { get; set; }
        private CustomerTab CustomerTab { get; set; }
        private DetailsTab DetailsTab { get; set; }
        private ShipToTab ShipToTab { get; set; }
        private ShipFromTab ShipFromTab { get; set; }
        private PaymentMethodsTab PaymentMethodsTab { get; set; }
        private bool SinglePaymentMethod => SystemInformationService.SystemInformation.IsPortugal;
        #endregion

        private ModalTabsNavigator Navigator { get; set; }
        public CheckButton CheckIsDraft { get; private set; } = new CheckButton("Guardar como Rascunho") { };


        IconButtonWithText BtnFillCustomerData = ActionAreaButton.FactoryGetDialogButtonTypeDocuments("touchButton_DialogActionArea",
                                                                                                      "Preencher",
                                                                                                      AppSettings.Paths.Images + @"Icons\icon_pos_agt_search_customer_data.png");


        public void UpdateUI()
        {
            UpdateTitle();

            BtnClear.Visible = this.Navigator.CurrentTab == this.CustomerTab;
            BtnFillCustomerData.Visible = this.Navigator.CurrentTab == this.CustomerTab;
            BtnPreview.Visible = this.Navigator.CurrentTab == this.DetailsTab && this.DetailsTab.Page.Items.Count > 0;
        }

    }
}
