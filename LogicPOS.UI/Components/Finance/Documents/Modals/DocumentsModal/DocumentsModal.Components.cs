using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Modals
{
    public partial class DocumentsModal
    {
        
        private IconButtonWithText BtnPayInvoice = ActionAreaButton.FactoryGetDialogButtonTypeDocuments("touchButton_Green",
                                                                                                      GeneralUtils.GetResourceByName("global_button_label_pay_invoice"),
                                                                                                      AppSettings.Paths.Images + @"Icons\icon_pos_payment_full.png");
        private IconButtonWithText BtnNewDocument { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments("touchButton_Green",
                                                                                                                       GeneralUtils.GetResourceByName("global_button_label_new_financial_document"),
                                                                                                                       AppSettings.Paths.Images + @"Icons\icon_pos_toolbar_finance_new_document.png");
        private IconButtonWithText BtnPrintDocument { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.Print, "touchButton_Green");
        private IconButtonWithText BtnOpenDocument { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.OpenDocument, "touchButton_Green");
        private IconButtonWithText BtnClose { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.Close);
        private IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.Cancel);
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
