using LogicPOS.Api.Features.Documents;
using LogicPOS.Globalization;
using LogicPOS.UI.Components.Finance.Documents.Sdr;
using LogicPOS.UI.Components.Finance.PaymentConditions;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public partial class DocumentTab
    {
        public override bool IsValid()
        {
            if (TxtDocumentType.SelectedEntity == null)
            {
                return false;
            }

            if (SinglePaymentMethod && TxtPaymentMethod.IsValid() == false)
            {
                return false;
            }


            return TxtPaymentCondition.IsValid() &&
                   TxtCurrency.IsValid() &&
                   TxtOriginDocument.IsValid() &&
                   TxtCopyDocument.IsValid() &&
                   TxtNotes.IsValid();

        }

        public DocumentTypeAnalyzer? DocumentTypeAnalyzer => GetDocumentType()?.Analyzer;

        private void UpdateValidatableFields()
        {
            if (DocumentTypeAnalyzer == null)
            {
                return;
            }

            if (DocumentTypeAnalyzer.Value.IsWayBill())
            {
                TxtOriginDocument.Require(false);
                TxtPaymentCondition.Require(false, false);
                if (SinglePaymentMethod)
                {
                    TxtPaymentMethod.Require(false, false);
                }
                TxtNotes.Require(false);
                return;
            }

            if (DocumentTypeAnalyzer.Value.IsVoltaRefundReceipt())
            {
                TxtOriginDocument.Require(false, false);
                TxtPaymentCondition.Require(false, false);
                if (SinglePaymentMethod)
                {
                    TxtPaymentMethod.Require(true, true);
                    TxtPaymentMethod.Component.Sensitive = false;
                    SelectPaymentMethod(TrvDocumentUiRules.ResolvePaymentMethod());
                }
                TxtNotes.Require(false);
                return;
            }

            if (DocumentTypeAnalyzer.Value.IsInformative())
            {
                TxtOriginDocument.Require(false, false);
                TxtPaymentCondition.IsRequired = true;
                TxtPaymentCondition.Component.Sensitive = true;
                if (PaymentConditionsService.Default != null)
                {
                    SelectPaymentCondition(PaymentConditionsService.Default);
                }
                if (SinglePaymentMethod)
                {
                    TxtPaymentMethod.Require(false, false);
                    TxtPaymentMethod.Component.Sensitive = true;
                }
                TxtNotes.Require(false);
                return;
            }

            if (DocumentTypeAnalyzer.Value.IsInvoice())
            {
                TxtOriginDocument.Require(false);
                TxtPaymentCondition.IsRequired = true;
                TxtPaymentCondition.Component.Sensitive = true;
                if (PaymentConditionsService.Default != null)
                {
                    SelectPaymentCondition(PaymentConditionsService.Default);
                }
                if (SinglePaymentMethod)
                {
                    TxtPaymentMethod.Require(false, false);
                }
                TxtNotes.Require(false);
                return;
            }

            if (DocumentTypeAnalyzer.Value.IsInvoiceReceipt() || DocumentTypeAnalyzer.Value.IsSimplifiedInvoice())
            {
                TxtOriginDocument.Require(false, false);
                TxtPaymentCondition.Require(false, false);
                if (SinglePaymentMethod)
                {
                    TxtPaymentMethod.Require(true, true);
                }
                TxtNotes.Require(false);
                return;
            }

            if (DocumentTypeAnalyzer.Value.IsDebitNote())
            {
                TxtOriginDocument.Require(false);
                TxtPaymentCondition.Require(false, false);
                if (SinglePaymentMethod)
                {
                    TxtPaymentMethod.Require(false, false);
                }
                TxtNotes.Require(false);
            }

            if (DocumentTypeAnalyzer.Value.IsCreditNote())
            {
                TxtOriginDocument.Require(true);
                TxtPaymentCondition.Require(false, false);
                if (SinglePaymentMethod)
                {
                    TxtPaymentMethod.Require(false, false);
                }
                TxtNotes.Require(true);
                TxtNotes.Label.Text = "Motivo";
            }
            else
            {
                TxtNotes.Label.Text = LocalizedString.Instance["global_notes"];
            }
        }

    }
}
