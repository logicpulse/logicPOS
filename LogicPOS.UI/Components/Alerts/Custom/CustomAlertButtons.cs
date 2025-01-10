using Gtk;
using LogicPOS.UI.Buttons;
using LogicPOS.Utility;
using System.Drawing;

namespace LogicPOS.UI.Alerts
{
    public class CustomAlertButtons
    {
        public IconButtonWithText Ok;
        public IconButtonWithText Cancel;
        public IconButtonWithText Yes;
        public IconButtonWithText No;
        public IconButtonWithText Close;

        public CustomAlertButtons(CustomAlertSettings settings)
        {
            Ok = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "touchButtonOk_DialogActionArea",
                    BackgroundColor = settings.colorBaseDialogActionAreaButtonBackground,
                    Text = GeneralUtils.GetResourceByName("global_button_label_ok"),
                    Font = settings.fontBaseDialogActionAreaButton,
                    FontColor = settings.colorBaseDialogActionAreaButtonFont,
                    Icon = settings.fileActionOK,
                    IconSize = settings.sizeBaseDialogActionAreaButtonIcon,
                    ButtonSize = new Size(settings.sizeBaseDialogActionAreaButton.Width, settings.sizeBaseDialogActionAreaButton.Height)
                });
         

            Cancel = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "touchButtonCancel_DialogActionArea",
                    BackgroundColor = settings.colorBaseDialogActionAreaButtonBackground,
                    Text = GeneralUtils.GetResourceByName("global_button_label_cancel"),
                    Font = settings.fontBaseDialogActionAreaButton,
                    FontColor = settings.colorBaseDialogActionAreaButtonFont,
                    Icon = settings.fileActionCancel,
                    IconSize = settings.sizeBaseDialogActionAreaButtonIcon,
                    ButtonSize = new System.Drawing.Size(settings.sizeBaseDialogActionAreaButton.Width, settings.sizeBaseDialogActionAreaButton.Height)
                });

            Yes = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "touchButtonYes_DialogActionArea",
                    BackgroundColor = settings.colorBaseDialogActionAreaButtonBackground,
                    Text = GeneralUtils.GetResourceByName("global_button_label_yes"),
                    Font = settings.fontBaseDialogActionAreaButton,
                    FontColor = settings.colorBaseDialogActionAreaButtonFont,
                    Icon = settings.fileActionYes,
                    IconSize = settings.sizeBaseDialogActionAreaButtonIcon,
                    ButtonSize = new System.Drawing.Size(settings.sizeBaseDialogActionAreaButton.Width, settings.sizeBaseDialogActionAreaButton.Height)
                });

            No = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "touchButtonNo_DialogActionArea",
                    BackgroundColor = settings.colorBaseDialogActionAreaButtonBackground,
                    Text = GeneralUtils.GetResourceByName("global_button_label_no"),
                    Font = settings.fontBaseDialogActionAreaButton,
                    FontColor = settings.colorBaseDialogActionAreaButtonFont,
                    Icon = settings.fileActionNo,
                    IconSize = settings.sizeBaseDialogActionAreaButtonIcon,
                    ButtonSize = new System.Drawing.Size(settings.sizeBaseDialogActionAreaButton.Width, settings.sizeBaseDialogActionAreaButton.Height)
                });

            Close = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "touchButtonClose_DialogActionArea",
                    BackgroundColor = settings.colorBaseDialogActionAreaButtonBackground,
                    Text = GeneralUtils.GetResourceByName("global_button_label_close"),
                    Font = settings.fontBaseDialogActionAreaButton,
                    FontColor = settings.colorBaseDialogActionAreaButtonFont,
                    Icon = settings.fileActionClose,
                    IconSize = settings.sizeBaseDialogActionAreaButtonIcon,
                    ButtonSize = new System.Drawing.Size(settings.sizeBaseDialogActionAreaButton.Width, settings.sizeBaseDialogActionAreaButton.Height)
                });

        }

        public ActionAreaButtons GetActionAreaButtons(ButtonsType buttonsType)
        {
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();
      
            switch (buttonsType)
            {
                case ButtonsType.Ok:
                    actionAreaButtons.Add(new ActionAreaButton(Ok, ResponseType.Ok));
                    break;
                case ButtonsType.Cancel:
                    actionAreaButtons.Add(new ActionAreaButton(Cancel, ResponseType.Cancel));
                    break;
                case ButtonsType.OkCancel:
                    actionAreaButtons.Add(new ActionAreaButton(Ok, ResponseType.Ok));
                    actionAreaButtons.Add(new ActionAreaButton(Cancel, ResponseType.Cancel));
                    break;
                case ButtonsType.YesNo:
                    actionAreaButtons.Add(new ActionAreaButton(Yes, ResponseType.Yes));
                    actionAreaButtons.Add(new ActionAreaButton(No, ResponseType.No));
                    break;
                case ButtonsType.Close:
                    actionAreaButtons.Add(new ActionAreaButton(Close, ResponseType.Close));
                    break;
                case ButtonsType.None:
                    break;
                default:
                    break;
            }

            return actionAreaButtons;
        }
    }
}
