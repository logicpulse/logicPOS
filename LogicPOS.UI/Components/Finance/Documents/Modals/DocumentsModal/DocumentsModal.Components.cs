using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.FiscalYears;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.Users;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Modals
{
    public partial class DocumentsModal
    {

        private IconButtonWithText BtnPayInvoice = ActionAreaButton.FactoryGetDialogButtonTypeDocuments("touchButton_Green",
                                                                                                      GeneralUtils.GetResourceByName("global_button_label_pay_invoice"),
                                                                                                      AppSettings.Paths.Images + @"Icons\icon_pos_payment_full.png");
        private IconButtonWithText BtnEditDraft = ActionAreaButton.FactoryGetDialogButtonTypeDocuments("touchButton_Green",
                                                                                                     "Ed. Rasc.",
                                                                                                     AppSettings.Paths.Images + @"Icons\icon_pos_nav_update.png");
       
        private IconButtonWithText BtnDeleteDraft = ActionAreaButton.FactoryGetDialogButtonTypeDocuments("touchButton_Red",
                                                                                                    "Eliminar",
                                                                                                    AppSettings.Paths.Images + @"Icons\icon_pos_nav_delete.png");

        private IconButtonWithText BtnSendDocumentToAgt = ActionAreaButton.FactoryGetDialogButtonTypeDocuments("touchButton_Green",
                                                                                                    "Enviar",
                                                                                                    AppSettings.Paths.Images + @"Icons\icon_pos_agt_send.png");

        private IconButtonWithText BtnSendDocumentToAt = ActionAreaButton.FactoryGetDialogButtonTypeDocuments("touchButton_Green",
                                                                                                   "Comunicar",
                                                                                                   AppSettings.Paths.Images + @"Icons\pos_at_icon.png");

        private IconButtonWithText BtnUpdateAgtValidationStatus = ActionAreaButton.FactoryGetDialogButtonTypeDocuments("touchButton_Green",
                                                                                                    "Atua. Estado",
                                                                                                    AppSettings.Paths.Images + @"Icons\icon_pos_agt_update.png");

        private IconButtonWithText BtnViewAgtDocument = ActionAreaButton.FactoryGetDialogButtonTypeDocuments("touchButton_Green",
                                                                                                    "Agt. Info",
                                                                                                    AppSettings.Paths.Images + @"Icons\icon_pos_agt_info.png");



        private IconButtonWithText BtnNewDocument { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments("touchButton_Green",
                                                                                                                       GeneralUtils.GetResourceByName("global_button_label_new_financial_document"),
                                                                                                                       AppSettings.Paths.Images + @"Icons\icon_pos_toolbar_finance_new_document.png");
        private IconButtonWithText BtnPrintDocument { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.Print, "touchButton_Green");
        private IconButtonWithText BtnOpenDocument { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.OpenDocument, "touchButton_Green");
        private IconButtonWithText BtnClose { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.Close);
        private IconButtonWithText BtnCloneDocument { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.CloneDocument, "touchButton_Green");
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


        private void UpdateUI()
        {
            bool hasFiscalYear = FiscalYearsService.HasActiveFiscalYear();
            var selectedDocument = Page.SelectedEntity;
            bool documentIsSelected = selectedDocument != null;

            BtnNewDocument.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_DOCUMENTFINANCETYPE_CREATE") && hasFiscalYear;
            BtnOpenDocument.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_DOCUMENTSSHOW_MENU") && hasFiscalYear && documentIsSelected;
            BtnPrintDocument.Sensitive = documentIsSelected;
            BtnPrintDocumentAs.Sensitive = documentIsSelected;
            BtnSendDocumentEmail.Sensitive = documentIsSelected;

            BtnPayInvoice.Sensitive = BtnPayInvoice.Visible = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_DOCUMENTSPAY_MENU") && hasFiscalYear && documentIsSelected && selectedDocument.IsPayable && IsInvoicePaymentMode;
            BtnCloneDocument.Visible = documentIsSelected && selectedDocument.IsActive && IsNotSelectionMode;
            BtnCancelDocument.Visible = Users.AuthenticationService.UserHasPermission("FINANCE_DOCUMENT_CANCEL_DOCUMENT") && hasFiscalYear && documentIsSelected && selectedDocument.IsCancellable && IsNotSelectionMode; ;
            BtnDeleteDraft.Visible = documentIsSelected && selectedDocument.IsDraft && IsNotSelectionMode;
            BtnEditDraft.Visible = documentIsSelected && selectedDocument.IsDraft && IsNotSelectionMode;
            BtnSendDocumentToAgt.Visible = documentIsSelected && selectedDocument.IsAgtDocument && IsNotSelectionMode;
            BtnSendDocumentToAt.Visible = documentIsSelected && selectedDocument.IsAtDocument && selectedDocument.AtResendDocument && IsNotSelectionMode;
            BtnUpdateAgtValidationStatus.Visible = documentIsSelected && selectedDocument.IsAgtDocument && IsNotSelectionMode;
            BtnViewAgtDocument.Visible = documentIsSelected && selectedDocument.IsAgtDocument && IsNotSelectionMode; 
        }

        private bool IsNotSelectionMode => _mode != Finance.Documents.Modals.DocumentsModal.DocumentsModalMode.Selection;
        private bool IsInvoicePaymentMode => _mode == Finance.Documents.Modals.DocumentsModal.DocumentsModalMode.UnpaidInvoices;

    }
}
