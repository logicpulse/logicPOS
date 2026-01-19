using logicpos;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.Modals.Common;
using System.Drawing;

namespace LogicPOS.UI.Components.Finance.Agt
{
    public partial class AgtDocumentInfoModal : Modal
    {
        private IconButtonWithText BtnClose { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.Close);
        private IconButtonWithText BtnCorrectDocument { get; set; } = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "touchButtonClose_DialogActionArea",
                    Text = "Corrigir",
                    Font = ExpressionEvaluatorExtended.FontDocumentsSizeDefault,
                    FontColor = Color.White,
                    Icon = "Assets\\Images\\Icons\\icon_pos_agt_send.png",
                    IconSize = ExpressionEvaluatorExtended.SizePosToolbarButtonIconSizeDefault,
                    ButtonSize = new Size(
                        ExpressionEvaluatorExtended.SizePosToolbarButtonSizeDefault.Width,
                        ExpressionEvaluatorExtended.SizePosToolbarButtonSizeDefault.Height),
                });

        public TextBox TxtSubmissionDate { get; private set; }
        public TextBox TxtRequestId { get; private set; }
        public TextBox TxtDocumentNumber { get; private set; }
        public TextBox TxtSubmissionErrorCode { get; private set; }
        public TextBox TxtSubmissionErrorDescription { get; private set; }
        public TextBox TxtHttpStatusCode { get; private set; }
        public TextBox TxtValidationResultCode { get; private set; }
        public TextBox TxtValidationStatus { get; private set; }
        public TextBox TxtValidationErrors { get; private set; }
        public TextBox TxtRejectedDocumentNumber { get; private set; }
    }
}
