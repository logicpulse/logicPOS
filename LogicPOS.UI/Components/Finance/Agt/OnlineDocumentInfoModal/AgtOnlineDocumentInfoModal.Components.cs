using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;

namespace LogicPOS.UI.Components.Finance.Agt
{
    public partial class AgtOnlineDocumentInfoModal
    {
        private IconButtonWithText BtnClose { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.Close);
        public TextBox TxtDate { get; private set; }
        public TextBox TxtDocumentType { get; private set; }
        public TextBox TxtDocumentNumber { get; private set; }
        public TextBox TxtStatus { get; private set; }
        public TextBox TxtCustomerNif { get; private set; }
        public TextBox TxtTaxPayable { get; private set; }
        public TextBox TxtNetTotal { get; private set; }
        public TextBox TxtGrossTotal { get; private set; }
    }
}
