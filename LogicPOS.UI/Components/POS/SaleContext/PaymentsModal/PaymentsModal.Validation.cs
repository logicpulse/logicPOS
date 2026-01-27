using Gtk;
using LogicPOS.Globalization;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Finance.Customers;
using LogicPOS.UI.Components.Finance.Documents.Rules;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Services;
using System;
using System.Linq;

namespace LogicPOS.UI.Components.POS
{
    public partial class PaymentsModal
    {
        private string GetInvalidSimplifiedInvoiceMessage()
        {
            string messageFormat = LocalizedString.Instance["dialog_message_value_exceed_simplified_invoice_max_value"];
            string totalsMessage = $"{LocalizedString.Instance["global_total"]}: {TotalFinal:C}\n{LocalizedString.Instance["global_maximum"]}: {DocumentRules.Portugal.SimplifiedInvoiceMaxTotal:C}";
            if (ServicesTotalFinal > DocumentRules.Portugal.SimplifiedInvoiceServicesMaxTotal)
            {
                totalsMessage += $"\n\n{LocalizedString.Instance["global_services"]}: {ServicesTotalFinal:C}\n{LocalizedString.Instance["global_maximum"]}: {DocumentRules.Portugal.SimplifiedInvoiceServicesMaxTotal:C}";
            }
            string message = string.Format(messageFormat, totalsMessage, LocalizedString.Instance["dialog_message_value_exceed_simplified_invoice_max_value_mode_paymentdialog"]);

            return message;
        }

        private string GetInvalidTotalForFinalConsumerMessage()
        {
            string messageFormat = LocalizedString.Instance["dialog_message_value_exceed_simplified_invoice_for_final_or_annonymous_consumer"];
            string message = string.Format(messageFormat,
                $"{LocalizedString.Instance["global_total"]}: {TotalFinal:C}",
                 $"{LocalizedString.Instance["global_maximum"]}: {DocumentRules.Portugal.FinalConsumerMaxTotal:C}");

            return message;
        }

        protected bool 
            Validate()
        {
            if (AllFieldsAreValid() == false)
            {
                ValidationUtilities.ShowValidationErrors(ValidatableFields);
                return false;
            }

            if (SystemInformationService.SystemInformation.IsPortugal)
            {

                if (DocTypeAnalyzer.IsSimplifiedInvoice() && (TotalFinal > DocumentRules.Portugal.SimplifiedInvoiceMaxTotal || ServicesTotalFinal > DocumentRules.Portugal.SimplifiedInvoiceMaxTotal))
                {
                   
                    string message = GetInvalidSimplifiedInvoiceMessage();
                    var response = CustomAlerts.Warning(this)
                        .WithSize(new global::System.Drawing.Size(550,440))
                        .WithButtonsType(ButtonsType.YesNo)
                        .WithMessage(message)
                        .ShowAlert();

                    if (response != ResponseType.Yes)
                    {
                        return false;
                    }

                    _documentType = "FR";
                }

                if (GetDocumentCustomer().FiscalNumber == CustomersService.Default.FiscalNumber && TotalFinal > DocumentRules.Portugal.FinalConsumerMaxTotal)
                {

                    string message = GetInvalidTotalForFinalConsumerMessage();
                    var response = CustomAlerts.Warning(this)
                        .WithSize(new global::System.Drawing.Size(550, 480))
                        .WithMessage(message)
                        .ShowAlert();

                    return false;
                }

            }

            return true;
        }

        protected bool AllFieldsAreValid()
        {
            return ValidatableFields.All(txt => txt.IsValid());
        }

    }
}
