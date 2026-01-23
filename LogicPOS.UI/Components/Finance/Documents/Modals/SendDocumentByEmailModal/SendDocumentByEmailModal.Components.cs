using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Widgets.Entrys;
using LogicPOS.Globalization;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Services;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Modals
{
    public partial class SendDocumentByEmailModal
    {
        private IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        private TextBox TxtSubject { get; set; }
        private TextBox TxtTo { get; set; }
        private TextBox TxtCc { get; set; }
        private TextBox TxtBcc { get; set; }
        private EntryBoxValidationMultiLine TxtBody { get; set; }
        private List<IValidatableField> ValidatableFields { get; set; } = new List<IValidatableField>();

        private void InitializeTxtBody()
        {
            TxtBody = new EntryBoxValidationMultiLine(this,
                                                      LocalizedString.Instance["global_email_body"],
                                                      KeyboardMode.AlfaNumeric,
                                                      string.Empty,
                                                      false);
        }

        private void InitializeTxtSubject()
        {
            TxtSubject = new TextBox(this,
                                        LocalizedString.Instance["global_email_subject"],
                                        true,
                                        includeSelectButton: false,
                                        includeKeyBoardButton: true);

            TxtSubject.Text = PreferenceParametersService.GetPreferenceParameterValue("SEND_MAIL_FINANCE_DOCUMENTS_SUBJECT");
        }

        private void InitializeTxtTo()
        {
            TxtTo = new TextBox(this,
                                    LocalizedString.Instance["global_email_to"],
                                    true,
                                    true,
                                    RegularExpressions.Email,
                                    includeSelectButton: false,
                                    includeKeyBoardButton: true);
        }

        private void InitializeTxtCc()
        {
            TxtCc = new TextBox(sourceWindow: this,
                                    labelText: LocalizedString.Instance["global_email_cc"],
                                    isRequired: false,
                                    isValidatable: true,
                                    RegularExpressions.Email,
                                    includeSelectButton: false,
                                    includeKeyBoardButton: true);
        }

        private void InitializeTxtBcc()
        {
            TxtBcc = new TextBox(sourceWindow: this,
                                     labelText: LocalizedString.Instance["global_email_bcc"],
                                     isRequired: false,
                                     isValidatable: true,
                                     RegularExpressions.Email,
                                     includeSelectButton: false,
                                     includeKeyBoardButton: true);
        }

    }
}
