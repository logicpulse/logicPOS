using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.Modals.Common;

namespace LogicPOS.UI.Components.Finance.Agt
{
    public partial class SeriesInfoModal : Modal
    {
        private IconButtonWithText BtnClose { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.Close);
        public TextBox TxtCode { get; private set; }
        public TextBox TxtYear { get; private set; }
        public TextBox TxtDocumentType { get; private set; }
        public TextBox TxtStatus { get; private set; }
        public TextBox TxtCreationDate { get; private set; }
        public TextBox TxtFirstDocument { get; private set; }
        public TextBox TxtLastDocument { get; private set; }
        public TextBox TxtInvoicingMethod { get; private set; }
        public TextBox TxtNif { get; private set; }
        public TextBox TxtName{ get; private set; }
        public TextBox TxtJoiningDate { get; private set; }
        public TextBox TxtJoiningType { get; private set; }
        }
}
