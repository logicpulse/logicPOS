using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Modals
{
    public partial class SystemAuditFilterModal
    {
        private IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        private TextBox TxtTerminal { get; set; }
        private TextBox TxtStartDate { get; set; }
        private TextBox TxtEndDate { get; set; }
        public HashSet<IValidatableField> ValidatableFields { get; private set; } = new HashSet<IValidatableField>();
    }
}
