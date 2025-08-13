using LogicPOS.Api.Entities;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Modals
{
    public partial class CustomerCurrentAccountFilterModal
    {
        private IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        private TextBox TxtCustomer { get; set; }
        private TextBox TxtStartDate { get; set; }
        private TextBox TxtEndDate { get; set; }
        public HashSet<IValidatableField> ValidatableFields { get; private set; } = new HashSet<IValidatableField>();
        private List<Customer> _customersForCompletion;
        private List<Customer> CustomersForCompletion => _customersForCompletion ?? InitializeCustomersForCompletion();
    }
}
