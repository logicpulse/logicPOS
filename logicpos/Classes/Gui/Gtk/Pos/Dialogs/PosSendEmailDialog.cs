using Gtk;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Gui.Gtk.Widgets.Entrys;
using logicpos.datalayer.App;
using logicpos.shared.App;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Mail;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal class PosSendEmailDialog : PosBaseDialog
    {
        private readonly VBox _vbox;
        private readonly TouchButtonIconWithText _buttonOk;
        private readonly TouchButtonIconWithText _buttonCancel;
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

        public PosSendEmailDialog(Window pSourceWindow, DialogFlags pDialogFlags, Size pSize, string pWindowTitle, string pSubject, string pTo, string pBody, List<string> attachmentFileNames)
            : base(pSourceWindow, pDialogFlags)
        {
            // Init Local Vars
            string windowTitle = pWindowTitle;
            Size windowSize = pSize;
            string windowIcon = DataLayerFramework.Path["images"] + @"Icons\Windows\icon_window_send_email.png";

            AttachmentFileNames = attachmentFileNames;

            // EntryBoxValidationSubject
            _entryBoxValidationSubject = new EntryBoxValidation(this, resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_email_subject"), KeyboardMode.AlfaNumeric, SharedSettings.RegexAlfaNumericEmail, false);
            if (!string.IsNullOrEmpty(pSubject))
            {
                _entryBoxValidationSubject.EntryValidation.Text = pSubject;
            }
            // EntryBoxValidationTo
            _entryBoxValidationTo = new EntryBoxValidation(this, resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_email_to"), KeyboardMode.AlfaNumeric, SharedSettings.RegexEmail, false);
            if (!string.IsNullOrEmpty(pTo))
            {
                _entryBoxValidationTo.EntryValidation.Text = pTo;
            }
            // EntryBoxValidationCc
            _entryBoxValidationCc = new EntryBoxValidation(this, resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_email_cc"), KeyboardMode.AlfaNumeric, SharedSettings.RegexEmail, false);
            // EntryBoxValidationBcc
            _entryBoxValidationBcc = new EntryBoxValidation(this, resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_email_bcc"), KeyboardMode.AlfaNumeric, SharedSettings.RegexEmail, false);

            _entryBoxValidationMultiLine = new EntryBoxValidationMultiLine(this, resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_email_body"), KeyboardMode.AlfaNumeric, SharedSettings.RegexAlfaNumericEmail, true) { HeightRequest = 280 };
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
            _buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
            _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);

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
            this.InitObject(this, pDialogFlags, windowIcon, windowTitle, windowSize, fixedContent, actionAreaButtons);
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
                    // One of them is Required
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
                    LogicPOS.Utility.EmailUtils.SendMail(
                        LogicPOS.Settings.AppSettings.PreferenceParameters,
                        SmtpDeliveryMethod.Network,
                        To,
                        Cc,
                        Bcc,
                        Subject,
                        Body,
                        AttachmentFileNames
                        );
                    logicpos.Utils.ShowMessageTouch(this, DialogFlags.Modal, new Size(650, 380), MessageType.Info, ButtonsType.Ok, resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_information"), resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "dialog_message_mail_sent_successfully"));
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                    logicpos.Utils.ShowMessageTouch(this, DialogFlags.Modal, new Size(650, 380), MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_error"), ex.Message);
                    // Keep Running
                    this.Run();
                }
            }
        }
    }
}
