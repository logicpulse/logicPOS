using Gtk;
using logicpos;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System;

namespace LogicPOS.UI.Buttons
{
    public class ActionAreaButton
    {
        public CustomButton Button { get; set; }
        public ResponseType Response { get; set; }

        public event EventHandler Clicked;

        public ActionAreaButton(CustomButton button, ResponseType response)
        {
            Button = button;
            Response = response;

            Button.Clicked += ActionAreaButton_Clicked;
        }

        private void ActionAreaButton_Clicked(object sender, EventArgs e)
        {
            //Send this and Not sender, to catch base object
            Clicked?.Invoke(this, e);
        }

        public static IconButtonWithText FactoryGetDialogButtonType(DialogButtonType pButtonType)
        {
            return FactoryGetDialogButtonType(pButtonType, string.Empty, string.Empty, string.Empty);
        }

        public static IconButtonWithText FactoryGetDialogButtonType(DialogButtonType pButtonType, string pButtonName)
        {
            return FactoryGetDialogButtonType(pButtonType, pButtonName, string.Empty, string.Empty);
        }

        public static IconButtonWithText FactoryGetDialogButtonType(string pButtonName, string pButtonLabel, string pButtonImage)
        {
            return FactoryGetDialogButtonType(DialogButtonType.Default, pButtonName, pButtonLabel, pButtonImage);
        }

        public static IconButtonWithText FactoryGetDialogButtonType(DialogButtonType pButtonType, string pButtonName, string pButtonLabel, string pButtonImage)
        {
            System.Drawing.Size sizeBaseDialogDefaultButton = AppSettings.Instance.SizeBaseDialogDefaultButton;
            System.Drawing.Size sizeBaseDialogDefaultButtonIcon = AppSettings.Instance.SizeBaseDialogDefaultButtonIcon;
            System.Drawing.Size sizeBaseDialogActionAreaButton = AppSettings.Instance.SizeBaseDialogActionAreaButton;
            System.Drawing.Size sizeBaseDialogActionAreaButtonIcon = AppSettings.Instance.SizeBaseDialogActionAreaButtonIcon;
            System.Drawing.Color colorBaseDialogActionAreaButtonFont = AppSettings.Instance.ColorBaseDialogActionAreaButtonFont;

            string fontBaseDialogActionAreaButton = AppSettings.Instance.FontBaseDialogActionAreaButton;
            //Icons
            string fileActionDefault = AppSettings.Paths.Images + @"Icons\icon_pos_default.png";
            string fileActionOK = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_ok.png";
            string fileActionCancel = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_cancel.png";
            string fileActionYes = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_yes.png";
            string fileActionNo = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_no.png";
            string fileActionClose = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_close.png";
            string fileActionPrint = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_print.png";
            string fileActionPrintAs = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_print_as.png";
            string fileActionHelp = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_help.png";
            string fileActionCloneDocument = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_clone_document.png ";
            string fileActionOpenDocument = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_open_document.png";
            string fileActionSendEmailDocument = AppSettings.Paths.Images + @"Icons\icon_pos_dialog_send_email.png";
            string fileActionCleanFilter = AppSettings.Paths.Images + @"Icons\icon_pos_clean_filter.png";

            //Assign ButtonType to Name, Override After Switch
            string buttonName = pButtonName != string.Empty ? pButtonName : string.Format("touchButton{0}_DialogActionArea", pButtonType.ToString());
            string buttonLabel = string.Empty;
            string buttonImage = string.Empty;

            switch (pButtonType)
            {
                case DialogButtonType.Default:
                    //Required to be changed by Code
                    buttonLabel = "Default";
                    buttonImage = fileActionDefault;
                    break;
                case DialogButtonType.Ok:
                    buttonLabel = GeneralUtils.GetResourceByName("global_button_label_ok");
                    buttonImage = fileActionOK;
                    break;
                case DialogButtonType.Cancel:
                    buttonLabel = GeneralUtils.GetResourceByName("global_button_label_cancel");
                    buttonImage = fileActionCancel;
                    break;
                case DialogButtonType.Yes:
                    buttonLabel = GeneralUtils.GetResourceByName("global_button_label_yes");
                    buttonImage = fileActionYes;
                    break;
                case DialogButtonType.No:
                    buttonLabel = GeneralUtils.GetResourceByName("global_button_label_no");
                    buttonImage = fileActionNo;
                    break;
                case DialogButtonType.Close:
                    buttonLabel = GeneralUtils.GetResourceByName("global_button_label_close");
                    buttonImage = fileActionClose;
                    break;
                case DialogButtonType.Print:
                    buttonLabel = GeneralUtils.GetResourceByName("global_button_label_print");
                    buttonImage = fileActionPrint;
                    break;
                case DialogButtonType.PrintAs:
                    buttonLabel = GeneralUtils.GetResourceByName("global_button_label_print_as");
                    buttonImage = fileActionPrintAs;
                    break;
                case DialogButtonType.Help:
                    buttonLabel = GeneralUtils.GetResourceByName("global_button_label_help");
                    buttonImage = fileActionHelp;
                    break;
                case DialogButtonType.CloneDocument:
                    buttonLabel = GeneralUtils.GetResourceByName("global_button_label_clone_document");
                    buttonImage = fileActionCloneDocument;
                    break;
                case DialogButtonType.OpenDocument:
                    buttonLabel = GeneralUtils.GetResourceByName("global_button_label_open_document");
                    buttonImage = fileActionOpenDocument;
                    break;
                case DialogButtonType.SendEmailDocument:
                    buttonLabel = GeneralUtils.GetResourceByName("global_button_label_send_email_document");
                    buttonImage = fileActionSendEmailDocument;
                    break;
                case DialogButtonType.CleanFilter:
                    buttonLabel = GeneralUtils.GetResourceByName("global_button_label_filter_clear");
                    buttonImage = fileActionCleanFilter;
                    break;
                default:
                    break;
            }

            //Overrride buttonName, buttonLabel, buttonImage if Defined form Parameters
            if (pButtonLabel != string.Empty) buttonLabel = pButtonLabel;
            if (pButtonImage != string.Empty) buttonImage = pButtonImage;

            //Result Button
            return new IconButtonWithText(
                new ButtonSettings
                {
                    Name = buttonName,
                    Text = buttonLabel,
                    Font = fontBaseDialogActionAreaButton,
                    FontColor = colorBaseDialogActionAreaButtonFont,
                    Icon = buttonImage,
                    IconSize = sizeBaseDialogActionAreaButtonIcon,
                    ButtonSize = new System.Drawing.Size(sizeBaseDialogActionAreaButton.Width, sizeBaseDialogActionAreaButton.Height)
                });
        }


        //IN009257 Redimensionar botões para a resolução 1024 x 768.
        public static IconButtonWithText FactoryGetDialogButtonTypeDocuments(DialogButtonType pButtonType)
        {
            return FactoryGetDialogButtonTypeDocuments(pButtonType, string.Empty, string.Empty, string.Empty);
        }
        public static IconButtonWithText FactoryGetDialogButtonTypeDocuments(DialogButtonType pButtonType, string pButtonName)
        {
            return FactoryGetDialogButtonTypeDocuments(pButtonType, pButtonName, string.Empty, string.Empty);
        }
        public static IconButtonWithText FactoryGetDialogButtonTypeDocuments(string pButtonName, string pButtonLabel, string pButtonImage)
        {
            return FactoryGetDialogButtonTypeDocuments(DialogButtonType.Default, pButtonName, pButtonLabel, pButtonImage);
        }

        public static IconButtonWithText FactoryGetDialogButtonTypeDocuments(DialogButtonType pButtonType, string pButtonName, string pButtonLabel, string pButtonImage)
        {
            System.Drawing.Color colorBaseDialogActionAreaButtonFont = AppSettings.Instance.ColorBaseDialogActionAreaButtonFont;
            //Icons
            string fileActionDefault = AppSettings.Paths.Images + @"Icons\icon_pos_default.png";
            string fileActionOK = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_ok.png";
            string fileActionCancel = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_cancel.png";
            string fileActionYes = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_yes.png";
            string fileActionNo = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_no.png";
            string fileActionClose = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_close.png";
            string fileActionPrint = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_print.png";
            string fileActionPrintAs = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_print_as.png";
            string fileActionHelp = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_help.png";
            string fileActionCloneDocument = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_clone_document.png ";
            string fileActionOpenDocument = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_open_document.png";
            string fileActionSendEmailDocument = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_send_email.png";
            string fileActionCleanFilter = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_clean_filter.png";

            //Assign ButtonType to Name, Override After Switch
            string buttonName = pButtonName != string.Empty ? pButtonName : string.Format("touchButton{0}_DialogActionArea", pButtonType.ToString());
            string buttonLabel = string.Empty;
            string buttonImage = string.Empty;

            switch (pButtonType)
            {
                case DialogButtonType.Default:
                    //Required to be changed by Code
                    buttonLabel = "Default";
                    buttonImage = fileActionDefault;
                    break;
                case DialogButtonType.Ok:
                    buttonLabel = GeneralUtils.GetResourceByName("global_button_label_ok");
                    buttonImage = fileActionOK;
                    break;
                case DialogButtonType.Cancel:
                    buttonLabel = GeneralUtils.GetResourceByName("global_button_label_cancel");
                    buttonImage = fileActionCancel;
                    break;
                case DialogButtonType.Yes:
                    buttonLabel = GeneralUtils.GetResourceByName("global_button_label_yes");
                    buttonImage = fileActionYes;
                    break;
                case DialogButtonType.No:
                    buttonLabel = GeneralUtils.GetResourceByName("global_button_label_no");
                    buttonImage = fileActionNo;
                    break;
                case DialogButtonType.Close:
                    buttonLabel = GeneralUtils.GetResourceByName("global_button_label_close");
                    buttonImage = fileActionClose;
                    break;
                case DialogButtonType.Print:
                    buttonLabel = GeneralUtils.GetResourceByName("global_button_label_print");
                    buttonImage = fileActionPrint;
                    break;
                case DialogButtonType.PrintAs:
                    buttonLabel = GeneralUtils.GetResourceByName("global_button_label_print_as");
                    buttonImage = fileActionPrintAs;
                    break;
                case DialogButtonType.Help:
                    buttonLabel = GeneralUtils.GetResourceByName("global_button_label_help");
                    buttonImage = fileActionHelp;
                    break;
                case DialogButtonType.CloneDocument:
                    buttonLabel = GeneralUtils.GetResourceByName("global_button_label_clone_document");
                    buttonImage = fileActionCloneDocument;
                    break;
                case DialogButtonType.OpenDocument:
                    buttonLabel = GeneralUtils.GetResourceByName("global_button_label_open_document");
                    buttonImage = fileActionOpenDocument;
                    break;
                case DialogButtonType.SendEmailDocument:
                    buttonLabel = GeneralUtils.GetResourceByName("global_button_label_send_email_document");
                    buttonImage = fileActionSendEmailDocument;
                    break;
                case DialogButtonType.CleanFilter:
                    buttonLabel = GeneralUtils.GetResourceByName("global_button_label_filter_clear");
                    buttonImage = fileActionCleanFilter;
                    break;
                default:
                    break;
            }

            //Overrride buttonName, buttonLabel, buttonImage if Defined form Parameters
            if (pButtonLabel != string.Empty) buttonLabel = pButtonLabel;
            if (pButtonImage != string.Empty) buttonImage = pButtonImage;

            //Result Button
            return new IconButtonWithText(
                new ButtonSettings
                {
                    Name = buttonName,
                    Text = buttonLabel,
                    Font = ExpressionEvaluatorExtended.FontDocumentsSizeDefault,
                    FontColor = colorBaseDialogActionAreaButtonFont,
                    Icon = buttonImage,
                    IconSize = ExpressionEvaluatorExtended.SizePosToolbarButtonIconSizeDefault,
                    ButtonSize = new System.Drawing.Size(
                        ExpressionEvaluatorExtended.SizePosToolbarButtonSizeDefault.Width,
                        ExpressionEvaluatorExtended.SizePosToolbarButtonSizeDefault.Height)
                });
        }

    }
}