using Gtk;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Entrys;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Dialogs;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal class PosSendEmailDialog : BaseDialog
    {
        private readonly VBox _vbox;
        private readonly IconButtonWithText _buttonOk;
        private readonly IconButtonWithText _buttonCancel;
        private readonly EntryBoxValidation _entryBoxValidationSubject;
        private readonly EntryBoxValidation _entryBoxValidationTo;
        private readonly EntryBoxValidation _entryBoxValidationCc;
        private readonly EntryBoxValidation _entryBoxValidationBcc;
        private readonly EntryBoxValidationMultiLine _entryBoxValidationMultiLine;

        public string Subject { get => _entryBoxValidationSubject.EntryValidation.Text; }
        public string To { get => _entryBoxValidationTo.EntryValidation.Text; }
        public string Cc { get => _entryBoxValidationCc.EntryValidation.Text; }
        public string Bcc { get => _entryBoxValidationBcc.EntryValidation.Text; }
        public string Body { get => _entryBoxValidationMultiLine.EntryMultiline.Value.Text; }
        public List<string> AttachmentFileNames { get; }

        public PosSendEmailDialog(Window parentWindow,
                                  DialogFlags pDialogFlags,
                                  Size pSize,
                                  string pWindowTitle,
                                  string pSubject,
                                  string pTo,
                                  string pBody,
                                  List<string> attachmentFileNames)
            : base(parentWindow,
                   pDialogFlags)
        {
            // Init Local Vars
            string windowTitle = pWindowTitle;
            Size windowSize = pSize;
            string windowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_send_email.png";

            AttachmentFileNames = attachmentFileNames;

            // EntryBoxValidationSubject
            _entryBoxValidationSubject = new EntryBoxValidation(this, GeneralUtils.GetResourceByName("global_email_subject"), KeyboardMode.AlfaNumeric, RegularExpressions.AlfaNumericEmail, false);
            if (!string.IsNullOrEmpty(pSubject))
            {
                _entryBoxValidationSubject.EntryValidation.Text = pSubject;
            }
            // EntryBoxValidationTo
            _entryBoxValidationTo = new EntryBoxValidation(this, GeneralUtils.GetResourceByName("global_email_to"), KeyboardMode.AlfaNumeric, RegularExpressions.Email, false);
            if (!string.IsNullOrEmpty(pTo))
            {
                _entryBoxValidationTo.EntryValidation.Text = pTo;
            }
            // EntryBoxValidationCc
            _entryBoxValidationCc = new EntryBoxValidation(this, GeneralUtils.GetResourceByName("global_email_cc"), KeyboardMode.AlfaNumeric, RegularExpressions.Email, false);
            // EntryBoxValidationBcc
            _entryBoxValidationBcc = new EntryBoxValidation(this, GeneralUtils.GetResourceByName("global_email_bcc"), KeyboardMode.AlfaNumeric, RegularExpressions.Email, false);

            _entryBoxValidationMultiLine = new EntryBoxValidationMultiLine(this, GeneralUtils.GetResourceByName("global_email_body"), KeyboardMode.AlfaNumeric, RegularExpressions.AlfaNumericEmail, true) { HeightRequest = 280 };
            if (!string.IsNullOrEmpty(pBody))
            {
                _entryBoxValidationMultiLine.EntryMultiline.Value.Text = pBody;
            }

            // VBox
            _vbox = new VBox(false, 0) { WidthRequest = windowSize.Width - 12 };
            _vbox.PackStart(_entryBoxValidationSubject, false, false, 0);
            _vbox.PackStart(_entryBoxValidationTo, false, false, 0);
            _vbox.PackStart(_entryBoxValidationCc, false, false, 0);
            _vbox.PackStart(_entryBoxValidationBcc, false, false, 0);
            _vbox.PackStart(_entryBoxValidationMultiLine, true, true, 0);

            //Init Content
            Fixed fixedContent = new Fixed();
            fixedContent.Put(_vbox, 0, 0);

            //ActionArea Buttons
            _buttonOk = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
            _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);

            // Start Validated
            Validate();

            //After Button Construction
            _entryBoxValidationTo.EntryValidation.Changed += delegate { Validate(); };
            _entryBoxValidationCc.EntryValidation.Changed += delegate { Validate(); };
            _entryBoxValidationBcc.EntryValidation.Changed += delegate { Validate(); };
            _entryBoxValidationMultiLine.EntryMultiline.Value.Changed += delegate { Validate(); };

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
            {
                new ActionAreaButton(_buttonOk, ResponseType.Ok),
                new ActionAreaButton(_buttonCancel, ResponseType.Cancel)
            };

            //Init Object
            this.Initialize(this, pDialogFlags, windowIcon, windowTitle, windowSize, fixedContent, actionAreaButtons);
        }

        private void Validate()
        {
            _buttonOk.Sensitive = (
                _entryBoxValidationSubject.EntryValidation.Validated &&
                _entryBoxValidationTo.EntryValidation.Validated &&
                _entryBoxValidationCc.EntryValidation.Validated &&
                _entryBoxValidationBcc.EntryValidation.Validated &&
                _entryBoxValidationMultiLine.Validated &&
                (
                    !string.IsNullOrEmpty(_entryBoxValidationTo.EntryValidation.Text) ||
                    !string.IsNullOrEmpty(_entryBoxValidationCc.EntryValidation.Text) ||
                    !string.IsNullOrEmpty(_entryBoxValidationBcc.EntryValidation.Text)
                )
            );
        }

        protected override void OnResponse(ResponseType pResponse)
        {
            if (pResponse == ResponseType.Ok)
            {
                try
                {


                    CustomAlerts.Information(this)
                                .WithSize(new Size(600, 400))
                                .WithTitleResource("global_information")
                                .WithMessage(GeneralUtils.GetResourceByName("dialog_message_mail_sent_successfully"))
                                .ShowAlert();

                }
                catch (Exception ex)
                {
                    CustomAlerts.Error(this)
                                .WithSize(new Size(500, 340))
                                .WithTitleResource("global_error")
                                .WithMessage(ex.Message)
                                .ShowAlert();

                    this.Run();
                }
            }
        }
    }
}
