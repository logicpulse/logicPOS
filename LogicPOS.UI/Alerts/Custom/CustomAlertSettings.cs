using Gtk;
using LogicPOS.Settings;
using System;
using System.Drawing;

namespace LogicPOS.UI.Alerts
{
    public class CustomAlertSettings
    {
        public Color colorBaseDialogActionAreaButtonBackground = AppSettings.Instance.colorBaseDialogActionAreaButtonBackground;
        public Color colorBaseDialogActionAreaButtonFont = AppSettings.Instance.colorBaseDialogActionAreaButtonFont;
        public Size sizeBaseDialogActionAreaButtonIcon = AppSettings.Instance.sizeBaseDialogActionAreaButtonIcon;
        public Size sizeBaseDialogActionAreaButton = AppSettings.Instance.sizeBaseDialogActionAreaButton;
        public string fontBaseDialogActionAreaButton = AppSettings.Instance.fontBaseDialogActionAreaButton;


        public string fileImageDialogBaseMessageTypeImage = AppSettings.Instance.fileImageDialogBaseMessageTypeImage;
        public string fileImageDialogBaseMessageTypeIcon = AppSettings.Instance.fileImageDialogBaseMessageTypeIcon;

        public string fileActionOK = PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_action_ok.png";
        public string fileActionCancel = PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_action_cancel.png";
        public string fileActionYes = PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_action_yes.png";
        public string fileActionNo = PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_action_no.png";
        public string fileActionClose = PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_action_close.png";

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
