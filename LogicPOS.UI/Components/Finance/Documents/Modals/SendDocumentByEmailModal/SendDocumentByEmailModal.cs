using Gtk;
using LogicPOS.Api.Features.Documents.SendDocumentsByEmail;
using LogicPOS.Api.Features.Finance.Customers.Customers.Common;
using LogicPOS.Globalization;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Finance.Customers;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Errors;
using LogicPOS.UI.Services;
using LogicPOS.UI.Settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class SendDocumentByEmailModal : Modal
    {
        private readonly Customer _customer;
        private readonly IEnumerable<(Guid Id, string Number)> _documents;
        private readonly bool _sendReceipts;


        public SendDocumentByEmailModal(IEnumerable<(Guid Id, string Number)> documents,
                                        string customerFiscalNumber,
                                        bool sendReceipts,
                                        Window parent) : base(parent,
                                                              LocalizedString.Instance["window_title_send_email"],
                                                              new Size(800, 640),
                                                              AppSettings.Paths.Images + @"Icons\Windows\icon_window_send_email.png")
        {
            _customer = CustomersService.GetByFiscalNumber(customerFiscalNumber);
            _documents = documents;
            _sendReceipts = sendReceipts;

            ShowData();
        }

        private void ShowData()
        {
            TxtTo.Text = _customer?.Email;
            string bodyTemplate = PreferenceParametersService.GetPreferenceParameterValue("SEND_MAIL_FINANCE_DOCUMENTS_BODY");
            bool useHtmlBody = PreferenceParametersService.GetPreferenceParameterValue("SEND_MAIL_FINANCE_DOCUMENTS_HTML_BODY").ToLower() == "true";
            var companyInformation = CompanyDetailsService.CompanyInformation;

            bodyTemplate = bodyTemplate.Replace("${DOCUMENT_LIST}", string.Join(useHtmlBody ? ";<br/>":";\n", _documents.Select(d => d.Number)));
            bodyTemplate = bodyTemplate.Replace("${COMPANY_NAME}", companyInformation.Name);
            bodyTemplate = bodyTemplate.Replace("${COMPANY_BUSINESS_NAME}", companyInformation.BusinessName);
            bodyTemplate = bodyTemplate.Replace("${COMPANY_WEBSITE}", companyInformation.Website);
            bodyTemplate = bodyTemplate.Replace("${COMPANY_EMAIL}", companyInformation.Email);
            bodyTemplate = bodyTemplate.Replace("${COMPANY_TELEPHONE}", companyInformation.Phone);
            bodyTemplate = bodyTemplate.Replace("${COMPANY_ADDRESS}", companyInformation.Address);
            bodyTemplate = bodyTemplate.Replace("${COMPANY_POSTALCODE}", companyInformation.PostalCode);
            bodyTemplate = bodyTemplate.Replace("${COMPANY_CITY}", companyInformation.City);
            bodyTemplate = bodyTemplate.Replace("${COMPANY_COUNTRY}", companyInformation.CountryCode2);

            TxtBody.EntryMultiline.Value.Text = bodyTemplate;
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
                DocumentsIds = _documents.Select(d => d.Id),
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
