using Gtk;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Widgets.Entrys;
using LogicPOS.Api.Features.Documents.SendDocumentsByEmail;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Errors;
using LogicPOS.UI.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public class SendDocumentByEmailModal : Modal
    {
        #region Components
        private IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        private TextBox TxtSubject { get; set; }
        private TextBox TxtTo { get; set; }
        private TextBox TxtCc { get; set; }
        private TextBox TxtBcc { get; set; }
        private EntryBoxValidationMultiLine TxtBody { get; set; }
        private List<IValidatableField> ValidatableFields { get; set; } = new List<IValidatableField>();
        #endregion

        private readonly IEnumerable<Guid> _documentsIds;
        private readonly bool _sendReceipts;

        public SendDocumentByEmailModal(IEnumerable<Guid> documentsIds,
                                        bool sendReceipts,
                                        Window parent) : base(parent,
                                                              LocalizedString.Instance["window_title_send_email"],
                                                              new Size(800, 640),
                                                              PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_send_email.png")
        {
            _documentsIds = documentsIds;
            _sendReceipts = sendReceipts;
        }

        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            return new ActionAreaButtons
            {
                new ActionAreaButton(BtnOk, ResponseType.Ok),
                new ActionAreaButton(BtnCancel, ResponseType.Cancel)
            };
        }

        protected override Widget CreateBody()
        {
            Initialize();
            VBox verticalLayout = new VBox(false, 0);
            verticalLayout.PackStart(TxtSubject.Component, false, false, 0);
            verticalLayout.PackStart(TxtTo.Component, false, false, 0);
            verticalLayout.PackStart(TxtCc.Component, false, false, 0);
            verticalLayout.PackStart(TxtBcc.Component, false, false, 0);
            verticalLayout.PackStart(TxtBody, true, true, 0);

            return verticalLayout;
        }

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

        protected override void OnResponse(ResponseType response)
        {
            if (response == ResponseType.Ok)
            {

                if (AllFieldsAreValid() == false)
                {
                    ShowValidationErrors();
                    Run();
                    return;
                }

                SendDocumentsByEmailCommand command = CreateSendEmailCommand();
                var result = DependencyInjection.Mediator.Send(command).Result;

                if (result.IsError)
                {
                    ErrorHandlingService.HandleApiError(result, source: this);
                    Run();
                    return;
                }

                CustomAlerts.Information(this)
                            .WithMessageResource("dialog_message_mail_sent_successfully")
                            .ShowAlert();
            }
        }

        private SendDocumentsByEmailCommand CreateSendEmailCommand()
        {
            return new SendDocumentsByEmailCommand
            {
                DocumentsIds = _documentsIds,
                SendReceipts = _sendReceipts,
                Subject = TxtSubject.Text,
                To = TxtTo.Text,
                Cc = string.IsNullOrWhiteSpace(TxtCc.Text) ? null : TxtCc.Text,
                Bcc = string.IsNullOrWhiteSpace(TxtBcc.Text) ? null : TxtBcc.Text,
                Body = TxtBody.EntryMultiline.Value.Text
            };
        }

        public bool AllFieldsAreValid() => ValidatableFields.All(field => field.IsValid());

        protected void ShowValidationErrors() => ValidationUtilities.ShowValidationErrors(ValidatableFields);
    }
}
