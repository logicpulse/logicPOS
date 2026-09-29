using Gtk;
using LogicPOS.UI.Buttons;

namespace LogicPOS.UI.Components.Modals
{
    public partial class StockManagementModal
    {
        private IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        private Notebook Notebook { get; set; }
    }
}
