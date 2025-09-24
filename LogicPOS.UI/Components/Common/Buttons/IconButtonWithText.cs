using Gtk;
using logicpos;
using LogicPOS.UI.Extensions;
using LogicPOS.UI.Settings;
using System.IO;

namespace LogicPOS.UI.Buttons
{
    public class IconButtonWithText : TextButton
    {
        public Widget Page { get; set; }

        public IconButtonWithText(ButtonSettings settings)
            : base(settings, false)
        {
            ButtonSettings.Widget = CreateWidget(settings);
            Initialize();
        }

        private Widget CreateWidget(ButtonSettings settings)
        {


            System.Drawing.Image buttonIcon;

            if (settings.LeftImage == false)
            {
                VBox verticalBox = new VBox(false, 0);
                verticalBox.BorderWidth = 2;

                ButtonLabel = new Label(settings.Text);
                ChangeFont(settings.Font, settings.FontColor);

                buttonIcon = System.Drawing.Image.FromFile(settings.Icon);
                buttonIcon = Utils.ResizeAndCrop(buttonIcon, settings.IconSize);
                Gdk.Pixbuf pixBuf = Utils.ImageToPixbuf(buttonIcon);
                Image gtkimageButton = new Image(pixBuf);
                verticalBox.PackStart(gtkimageButton);
                buttonIcon.Dispose();
                pixBuf.Dispose();

                verticalBox.PackStart(ButtonLabel);
                return verticalBox;
            }

            string fontPosBackOfficeParent = AppSettings.Instance.FontPosBackOfficeParent;
            string fontPosBackOfficeParentLowRes = AppSettings.Instance.FontPosBackOfficeParentLowRes;
            Pango.FontDescription fontDescription = Pango.FontDescription.FromString(fontPosBackOfficeParent);

            if (AppSettings.Instance.AppScreenSize.Height == 800)
            {
                fontDescription = Pango.FontDescription.FromString(fontPosBackOfficeParentLowRes);
            }

            HBox hbox = new HBox(false, 0);
            ButtonLabel = new Label(settings.Text);
            ChangeFont(settings.Font, settings.FontColor);

            if (settings.Icon != string.Empty && File.Exists(settings.Icon))
            {
                buttonIcon = System.Drawing.Image.FromFile(settings.Icon);
                buttonIcon = Utils.ResizeAndCrop(buttonIcon, settings.IconSize);
                Gdk.Pixbuf pixBuf = Utils.ImageToPixbuf(buttonIcon);
                Image gtkimageButton = new Image(pixBuf);
                if (AppSettings.Instance.AppScreenSize.Height == 800)
                {
                    hbox.PackStart(gtkimageButton, false, false, 4);
                }
                else
                {
                    hbox.PackStart(gtkimageButton, false, false, 5);
                }

                buttonIcon.Dispose();
                pixBuf.Dispose();
            }

            ButtonLabel.ModifyFont(fontDescription);
            ButtonLabel.ModifyFg(StateType.Active, "0, 0, 0".StringToColor().ToGdkColor());
            ButtonLabel.SetAlignment(0.0f, 0.5f);
            hbox.PackStart(ButtonLabel, true, true, 0);

            return hbox;
        }

        public static IconButtonWithText Create(string name,
                                                      string label,
                                                      string icon)
        {


            return new IconButtonWithText(
                new ButtonSettings
                {
                    Name = name,
                    Text = label,
                    Font = AppSettings.Instance.FontBaseDialogActionAreaButton,
                    FontColor = AppSettings.Instance.ColorBaseDialogActionAreaButtonFont,
                    Icon = AppSettings.Paths.Images + icon,
                    IconSize = ExpressionEvaluatorExtended.SizePosToolbarButtonIconSizeDefault,
                    ButtonSize = new System.Drawing.Size(
                        ExpressionEvaluatorExtended.SizePosToolbarButtonSizeDefault.Width,
                        ExpressionEvaluatorExtended.SizePosToolbarButtonSizeDefault.Height)
                    /*AppSettings.Instance.SizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon,
                    ButtonSize = AppSettings.Instance.SizeBaseDialogActionAreaBackOfficeNavigatorButton*/
                });
        }
    }
}
