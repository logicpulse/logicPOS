using Gtk;
using LogicPOS.Api.Features.Documents.SendDocumentsByEmail;
using LogicPOS.Globalization;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Errors;
using LogicPOS.UI.Settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class SendDocumentByEmailModal : Modal
    {
        public SendDocumentByEmailModal(IEnumerable<Guid> documentsIds,
                                        bool sendReceipts,
                                        Window parent) : base(parent,
                                                              LocalizedString.Instance["window_title_send_email"],
                                                              new Size(800, 640),
                                                              AppSettings.Paths.Images + @"Icons\Windows\icon_window_send_email.png")
        {
            _documentsIds = documentsIds;
            _sendReceipts = sendReceipts;
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
