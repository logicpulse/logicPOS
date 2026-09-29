using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;

namespace LogicPOS.UI.Components.Modals
{
    public partial class AgtDocumentsFilterModal
    {
        private TextBox TxtStartDate { get; set; }
        private TextBox TxtEndDate { get; set; }
        private IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        private IconButtonWithText BtnClear { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.CleanFilter);
    }
}
