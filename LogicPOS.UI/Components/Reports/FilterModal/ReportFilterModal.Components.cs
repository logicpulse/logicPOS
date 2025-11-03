using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ReportsFilterModal
    {
        public TextBox TxtStartDate { get; set; }
        public TextBox TxtEndDate { get; set; }
        public TextBox TxtDocumentType { get; set; }
        public TextBox TxtCustomer { get; set; }
        public TextBox TxtWarehouse { get; set; }
        public TextBox TxtArticle { get; set; }
        public TextBox TxtVatRate { get; set; }
        public TextBox TxtSerialNumber { get; set; }
        public TextBox TxtDocumentNumber { get; set; }
        private IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        private IconButtonWithText BtnClear { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.CleanFilter);

    }
}
