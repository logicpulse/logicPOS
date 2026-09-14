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

        protected bool Validate()
        {
            var country = TxtCountry.SelectedEntity as Api.Entities.Country;
            bool isForeignCustomer = country != null && country.Code2 != "PT";

            // Se for estrangeiro e o NIF estiver em branco, removemos temporariamente 
            // o TxtFiscalNumber das validações obrigatórias da UI para evitar erros visuais.
            if (isForeignCustomer && string.IsNullOrWhiteSpace(TxtFiscalNumber.Text))
            {
                ValidatableFields.Remove(TxtFiscalNumber);
            }
            else
            {
                if (!ValidatableFields.Contains(TxtFiscalNumber))
                {
                    ValidatableFields.Add(TxtFiscalNumber);
                }
            }

            if (AllFieldsAreValid() == false)
            {
                ValidationUtilities.ShowValidationErrors(ValidatableFields, this);
                if (!ValidatableFields.Contains(TxtFiscalNumber))
                {
                    ValidatableFields.Add(TxtFiscalNumber);
                }
                return false;
            }

            if (!ValidatableFields.Contains(TxtFiscalNumber))
            {
                ValidatableFields.Add(TxtFiscalNumber);
            }

            if (_selectedPaymentMethod?.Token == "CUSTOMER_CARD")
            {
                // ... (restante código mantém-se igual)
                var customerCardCheck = GetSelectedCustomer();
                if (CustomersService.CanPayWithCustomerCard(customerCardCheck) == false)
                {
                    CustomAlerts.Warning(this)
                        .WithMessage("Cliente inválido!")
                        .ShowAlert();
                    return false;
                }

                if (CustomersService.HasSufficientCardBalance(customerCardCheck, TotalFinal) == false)
                {
                    CustomAlerts.Warning(this)
                        .WithMessage(string.Format(LocalizedString.Instance["dialog_message_value_exceed_customer_card_credit"],customerCardCheck.CardCredit.ToString("N2"),TotalFinal.ToString("N2")))
                        .ShowAlert();
                    return false;
                }
            }

            if (SystemInformationService.SystemInformation.IsPortugal)
            {
                string currentDocType = GetDocumentType();
                var customer = GetSelectedCustomer();
                
                // Determinar se é considerado Consumidor Final / Inválido para Fatura nominativa (FT/FR)
                bool isInvalidForInvoice = isForeignCustomer 
                    ? string.IsNullOrWhiteSpace(TxtCustomer.Text) 
                    : (customer == null || customer.IsFinalConsumer || GetDocumentCustomer().FiscalNumber == CustomersService.Default.FiscalNumber || string.IsNullOrWhiteSpace(TxtFiscalNumber.Text));

                // Bloqueio estrito para Fatura (FT) ou Fatura-Recibo (FR) a Consumidor Final / dados inválidos
                if ((currentDocType == "FT" || currentDocType == "FR") && isInvalidForInvoice)
                {
                    CustomAlerts.Error(this)
                        .WithMessageResource("dialog_message_cant_create_cc_document_with_default_entity")
                        .ShowAlert();
                    return false;
                }

                // Validação de limites para Fatura Simplificada (FS)
                if (DocTypeAnalyzer.IsSimplifiedInvoice() && (TotalFinal > DocumentRules.Portugal.SimplifiedInvoiceMaxTotal || ServicesTotalFinal > DocumentRules.Portugal.SimplifiedInvoiceServicesMaxTotal))
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
                    
                    // Revalidar se após a mudança para FR os dados continuam inválidos
                    if (isForeignCustomer ? string.IsNullOrWhiteSpace(TxtCustomer.Text) : (customer == null || customer.IsFinalConsumer || GetDocumentCustomer().FiscalNumber == CustomersService.Default.FiscalNumber))
                    {
                        CustomAlerts.Error(this)
                            .WithMessageResource("dialog_message_cant_create_cc_document_with_default_entity")
                            .ShowAlert();
                        return false;
                    }
                }

                // Validação do limite de Consumidor Final puro em FS
                bool isFinalConsumerCheck = customer == null || customer.IsFinalConsumer || GetDocumentCustomer().FiscalNumber == CustomersService.Default.FiscalNumber;
                if (isFinalConsumerCheck && TotalFinal > DocumentRules.Portugal.FinalConsumerMaxTotal)
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
