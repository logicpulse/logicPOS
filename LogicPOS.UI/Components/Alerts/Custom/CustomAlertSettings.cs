using Gtk;
using LogicPOS.UI.Settings;
using System;
using System.Drawing;

namespace LogicPOS.UI.Alerts
{
    public class CustomAlertSettings
    {
        public Color colorBaseDialogActionAreaButtonBackground = AppSettings.Instance.ColorBaseDialogActionAreaButtonBackground;
        public Color colorBaseDialogActionAreaButtonFont = AppSettings.Instance.ColorBaseDialogActionAreaButtonFont;
        public Size sizeBaseDialogActionAreaButtonIcon = AppSettings.Instance.SizeBaseDialogActionAreaButtonIcon;
        public Size sizeBaseDialogActionAreaButton = AppSettings.Instance.SizeBaseDialogActionAreaButton;
        public string fontBaseDialogActionAreaButton = AppSettings.Instance.FontBaseDialogActionAreaButton;


        public string fileImageDialogBaseMessageTypeImage = AppSettings.Instance.FileImageDialogBaseMessageTypeImage;
        public string fileImageDialogBaseMessageTypeIcon = AppSettings.Instance.FileImageDialogBaseMessageTypeIcon;

        public string fileActionOK = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_ok.png";
        public string fileActionCancel = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_cancel.png";
        public string fileActionYes = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_yes.png";
        public string fileActionNo = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_no.png";
        public string fileActionClose = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_close.png";

        public string GetDialogImage(MessageType messageType)
        {
            string msgType = Enum.GetName(typeof(MessageType), messageType).ToLower();

            if (msgType == string.Empty)
            {
                return msgType;
            }

            return string.Format(fileImageDialogBaseMessageTypeImage, msgType);
        }

        public string GetDialogIcon(MessageType messageType)
        {
            string msgType = Enum.GetName(typeof(MessageType), messageType).ToLower();

            if (msgType == string.Empty)
            {
                return msgType;
            }

            return string.Format(fileImageDialogBaseMessageTypeIcon, msgType);
        }

    }
}
