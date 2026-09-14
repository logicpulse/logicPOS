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
            string customerName = TxtCustomer.Text?.Trim() ?? string.Empty;
            string fiscalNumber = TxtFiscalNumber.Text?.Trim() ?? string.Empty;

            // ✅ SOLUÇÃO COMPLETA: Ajusta o NIF para estrangeiros SEM exigir preenchimento
            // Para estrangeiros SEM NIF: torna opcional
            // Para portugueses OU estrangeiros COM NIF: mantém obrigatório
            bool isForeignWithoutNif = isForeignCustomer && string.IsNullOrWhiteSpace(fiscalNumber);
            
            if (isForeignWithoutNif)
            {
                // Estrangeiro SEM NIF: Remove das validações
                ValidatableFields.Remove(TxtFiscalNumber);
                TxtFiscalNumber.IsRequired = false;
                TxtFiscalNumber.IsValidatable = false;
                TxtFiscalNumber.UpdateValidationColors();
            }
            else
            {
                // Português OU estrangeiro COM NIF: Mantém obrigatório
                if (!ValidatableFields.Contains(TxtFiscalNumber))
                {
                    ValidatableFields.Add(TxtFiscalNumber);
                }
                TxtFiscalNumber.IsRequired = true;
                TxtFiscalNumber.IsValidatable = true;
                TxtFiscalNumber.UpdateValidationColors();
            }

            // Valida com a configuração correta
            if (AllFieldsAreValid() == false)
            {
                ValidationUtilities.ShowValidationErrors(ValidatableFields, this);
                return false;
            }

            if (_selectedPaymentMethod?.Token == "CUSTOMER_CARD")
            {
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
                
                // ✅ Para estrangeiros, apenas verifica NOME (sem NIF)
                // Para portugueses, verifica NIF válido
                bool isInvalidForInvoice = isForeignCustomer 
                    ? string.IsNullOrWhiteSpace(customerName)
                    : (customer == null || customer.IsFinalConsumer || GetDocumentCustomer().FiscalNumber == CustomersService.Default.FiscalNumber || string.IsNullOrWhiteSpace(fiscalNumber));

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
                    bool isInvalidForFr = isForeignCustomer
                        ? string.IsNullOrWhiteSpace(customerName)
                        : (customer == null || customer.IsFinalConsumer || GetDocumentCustomer().FiscalNumber == CustomersService.Default.FiscalNumber);

                    if (isInvalidForFr)
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
