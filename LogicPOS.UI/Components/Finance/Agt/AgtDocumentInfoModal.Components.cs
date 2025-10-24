using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.Modals.Common;

namespace LogicPOS.UI.Components.Finance.Agt
{
    public partial class AgtDocumentInfoModal : Modal
    {
        private IconButtonWithText BtnClose { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.Close);
        public TextBox TxtSubmissionDate { get; private set; }
        public TextBox TxtRequestId { get; private set; }
        public TextBox TxtDocumentNumber { get; private set; }
        public TextBox TxtSubmissionErrorCode { get; private set; }
        public TextBox TxtSubmissionErrorDescription { get; private set; }
        public TextBox TxtHttpStatusCode { get; private set; }
        public TextBox TxtValidationResultCode { get; private set; }
        public TextBox TxtValidationStatus { get; private set; }
        public TextBox TxtValidationErrors { get; private set; }
    }
}
