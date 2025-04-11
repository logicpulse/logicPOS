using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;

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
    }
}
