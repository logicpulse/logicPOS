using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ReceiptsModal
    {
        private ReceiptsPage Page { get; set; }
        private string WindowTitleBase => GeneralUtils.GetResourceByName("window_title_dialog_document_finance_payment");

        private IconButtonWithText BtnSendDocumentToAgt = ActionAreaButton.FactoryGetDialogButtonTypeDocuments("touchButton_Green",
                                                                                                   "Enviar",
                                                                                                   AppSettings.Paths.Images + @"Icons\icon_pos_agt_send.png");

        private IconButtonWithText BtnUpdateAgtValidationStatus = ActionAreaButton.FactoryGetDialogButtonTypeDocuments("touchButton_Green",
                                                                                                    "Atua. Estado",
                                                                                                    AppSettings.Paths.Images + @"Icons\icon_pos_agt_update.png");

        private IconButtonWithText BtnViewAgtDocument = ActionAreaButton.FactoryGetDialogButtonTypeDocuments("touchButton_Green",
                                                                                                    "Agt. Info",
                                                                                                    AppSettings.Paths.Images + @"Icons\icon_pos_agt_info.png");



        private IconButtonWithText BtnPrintDocument { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.Print, "touchButton_Green");
        private IconButtonWithText BtnOpenDocument { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.OpenDocument, "touchButton_Green");
        private IconButtonWithText BtnClose { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.Close);
        private IconButtonWithText BtnPrintDocumentAs { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.PrintAs, "touchButton_Green");
        private IconButtonWithText BtnSendDocumentEmail { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.SendEmailDocument, "touchButton_Green");
        private IconButtonWithText BtnCancelDocument { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments("touchButton_Green",
                                                                                                                          GeneralUtils.GetResourceByName("global_button_label_cancel_document"),
                                                                                                                          AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_cancel.png");
        private IconButtonWithText BtnPrevious { get; set; }
        private IconButtonWithText BtnNext { get; set; }
        private TextBox TxtSearch { get; set; }
        public IconButtonWithText BtnFilter { get; set; }
    }
}
