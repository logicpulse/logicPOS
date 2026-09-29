using LogicPOS.UI.Settings;
using System.Drawing;

namespace LogicPOS.UI.Components.Modals.Common
{
    public class ModalSizesSettings
    {
        public Size PaymentButton = AppSettings.Instance.SizeBaseDialogDefaultButton;
        public Size PaymentButtonIcon = AppSettings.Instance.SizeBaseDialogDefaultButtonIcon;
        public Size DefaultButton = AppSettings.Instance.SizeBaseDialogDefaultButton;
        public Size DefaultButtonIcon = AppSettings.Instance.SizeBaseDialogDefaultButtonIcon;
        public Size ActionAreaButton = AppSettings.Instance.SizeBaseDialogActionAreaButton;
        public Size ActionAreaButtonIcon = AppSettings.Instance.SizeBaseDialogActionAreaButtonIcon;
    }
}
