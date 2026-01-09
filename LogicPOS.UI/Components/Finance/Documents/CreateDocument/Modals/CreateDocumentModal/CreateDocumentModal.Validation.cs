using Gtk;
using LogicPOS.Api.Features.Documents;
using LogicPOS.Globalization;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Finance.Customers;
using LogicPOS.UI.Components.Finance.Documents.Rules;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Services;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class CreateDocumentModal
    {
        private bool Validate()
        {
            if (AllTabsAreValid() == false)
            {
                ShowValidationErrors();
                return false;
            }

            if (SystemInformationService.SystemInformation.IsPortugal)
            {
                var docTypeAnalyzer = new DocumentTypeAnalyzer(DocumentTab.GetDocumentType().Acronym);

                if (docTypeAnalyzer.IsSimplifiedInvoice() && (DetailsTab.TotalFinal > DocumentRules.Portugal.SimplifiedInvoiceMaxTotal || DetailsTab.ServicesTotalFinal > DocumentRules.Portugal.SimplifiedInvoiceMaxTotal))
                {

                    string message = GetInvalidSimplifiedInvoiceMessage();
                    var response = CustomAlerts.Warning(this)
                        .WithSize(new System.Drawing.Size(550, 440))
                        .WithMessage(message)
                        .ShowAlert();

                    return false;
                }

                if (CustomerTab.FiscalNumber == CustomersService.Default.FiscalNumber && DetailsTab.TotalFinal > DocumentRules.Portugal.FinalConsumerMaxTotal)
                {

                    string message = GetInvalidTotalForFinalConsumerMessage();
                    var response = CustomAlerts.Warning(this)
                        .WithSize(new System.Drawing.Size(550, 480))
                        .WithMessage(message)
                        .ShowAlert();

                    return false;
                }

            }

            return true;
        }

        public bool AllTabsAreValid() => GetValidatableTabs().All(tab => tab.IsValid());

        public bool TabsForPreviewAreValid()
        {
            return DetailsTab.IsValid();
        }

        public IEnumerable<IValidatableField> GetValidatableTabs()
        {
            var validatableTabs = new List<IValidatableField>
            {
                DocumentTab,
                CustomerTab,
                DetailsTab
            };

            var docAnalyzer = DocumentTab.DocumentTypeAnalyzer;

            if (docAnalyzer == null)
            {
                return validatableTabs;
            }

            if (SinglePaymentMethod == false)
            {
                if (docAnalyzer.Value.IsInvoiceReceipt() || docAnalyzer.Value.IsSimplifiedInvoice())
                {
                    validatableTabs.Add(PaymentMethodsTab);
                }
            }

            if (docAnalyzer.Value.IsWayBill())
            {
                validatableTabs.Add(ShipToTab);
                validatableTabs.Add(ShipFromTab);
            }

            return validatableTabs;
        }

        protected void ShowValidationErrors() => ValidationUtilities.ShowValidationErrors(GetValidatableTabs());

        private string GetInvalidSimplifiedInvoiceMessage()
        {
            string messageFormat = LocalizedString.Instance["dialog_message_value_exceed_simplified_invoice_max_value"];
            string totalsMessage = $"{LocalizedString.Instance["global_total"]}: {DetailsTab.TotalFinal:C}\n{LocalizedString.Instance["global_maximum"]}: {DocumentRules.Portugal.SimplifiedInvoiceMaxTotal:C}";
            if (DetailsTab.ServicesTotalFinal > DocumentRules.Portugal.SimplifiedInvoiceServicesMaxTotal)
            {
                totalsMessage += $"\n\n{LocalizedString.Instance["global_services"]}: {DetailsTab.ServicesTotalFinal:C}\n{LocalizedString.Instance["global_maximum"]}: {DocumentRules.Portugal.SimplifiedInvoiceServicesMaxTotal:C}";
            }
            string message = string.Format(messageFormat, totalsMessage, LocalizedString.Instance["dialog_message_value_exceed_simplified_invoice_max_value_mode_paymentdialog_documentfinancedialog"]);

            return message;
        }

        private string GetInvalidTotalForFinalConsumerMessage()
        {
            string messageFormat = LocalizedString.Instance["dialog_message_value_exceed_simplified_invoice_for_final_or_annonymous_consumer"];
            string message = string.Format(messageFormat,
                $"{LocalizedString.Instance["global_total"]}: {DetailsTab.TotalFinal:C}",
                 $"{LocalizedString.Instance["global_maximum"]}: {DocumentRules.Portugal.FinalConsumerMaxTotal:C}");

            return message;
        }

    }
}
