using Gtk;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Widgets.Entrys;
using LogicPOS.Globalization;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.Modals
{
    public partial class SendDocumentByEmailModal
    {
        private void Initialize()
        {
            InitializeTxtSubject();
            InitializeTxtTo();
            InitializeTxtCc();
            InitializeTxtBcc();
            InitializeTxtBody();

            ValidatableFields.Add(TxtSubject);
            ValidatableFields.Add(TxtTo);
            ValidatableFields.Add(TxtCc);
            ValidatableFields.Add(TxtBcc);
        }
        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            return new ActionAreaButtons
            {
                new ActionAreaButton(BtnOk, ResponseType.Ok),
                new ActionAreaButton(BtnCancel, ResponseType.Cancel)
            };
        }

        private void InitializeTxtBody()
        {
            TxtBody = new EntryBoxValidationMultiLine(this,
                                                      LocalizedString.Instance["global_email_body"],
                                                      KeyboardMode.AlfaNumeric,
                                                      string.Empty,
                                                      false);

            TxtBody.EntryMultiline.Value.Text = PreferenceParametersService.GetPreferenceParameterValue("SEND_MAIL_FINANCE_DOCUMENTS_BODY");
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
