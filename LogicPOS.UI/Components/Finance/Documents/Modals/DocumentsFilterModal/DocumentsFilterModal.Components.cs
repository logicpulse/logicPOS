using LogicPOS.Api.Entities;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Documents
{
    public partial class DocumentsFilterModal
    {
        private TextBox TxtStartDate { get; set; }
        private TextBox TxtEndDate { get; set; }
        private TextBox TxtDocumentType { get; set; }
        private TextBox TxtCustomer { get; set; }
        private TextBox TxtPaymentMethod { get; set; }
        private TextBox TxtPaymentCondition { get; set; }
        private PageComboBox<int> ComboPaymentStatus { get; set; }
        private IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        private IconButtonWithText BtnClear { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.CleanFilter);

        private List<Customer> _customersForCompletion;
        private List<Customer> CustomersForCompletion => _customersForCompletion ?? InitializeCustomersForCompletion();

        private List<PaymentCondition> _paymentConditionsForCompletion;
        private List<PaymentCondition> PaymentConditionsForCompletion => _paymentConditionsForCompletion ?? InitializePaymentConditionsForCompletion();

        private List<PaymentMethod> _paymentMethodsForCompletion;
        private List<PaymentMethod> PaymentMethodsForCompletion => _paymentMethodsForCompletion ?? InitializePaymentMethodsForCompletion();

        private List<DocumentType> _documentTypesForCompletion;
        private List<DocumentType> DocumentTypesForCompletion => _documentTypesForCompletion ?? InitializeDocumentTypesForCompletion();
    }
}
