using LogicPOS.Settings;
using System.Drawing;

namespace LogicPOS.UI.Dialogs
{
    public class BaseDialogSizeSettings
    {
        public Size PaymentButton = AppSettings.Instance.sizeBaseDialogDefaultButton;
        public Size PaymentButtonIcon = AppSettings.Instance.sizeBaseDialogDefaultButtonIcon;
        public Size DefaultButton = AppSettings.Instance.sizeBaseDialogDefaultButton;
        public Size DefaultButtonIcon = AppSettings.Instance.sizeBaseDialogDefaultButtonIcon;
        public Size ActionAreaButton = AppSettings.Instance.sizeBaseDialogActionAreaButton;
        public Size ActionAreaButtonIcon = AppSettings.Instance.sizeBaseDialogActionAreaButtonIcon;
    }
}
