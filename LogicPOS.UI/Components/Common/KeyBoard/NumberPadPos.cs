using Gtk;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.Utility;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    internal class NumberPadPos : NumberPad
    {
        public NumberPadPos(string name, Color colorButton, Color colorRightButton, string fontButton, string fontRightButton, byte buttonWidth, byte rightButtonWidth, byte buttonHeight, byte padding = 0)
            : base(name, colorButton, fontButton, buttonWidth, buttonHeight, padding)
        {
            //Init Local Vars
            Size sizeIcon = new Size(28, 28);
            string icon1 = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_numberpad_1_splitaccount.png";
            string icon2 = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_numberpad_2_messages.png";
            string icon3 = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_numberpad_3_gifts.png";
            string icon4 = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_numberpad_4_weight.png";

            Color colorFont = Color.Black;

            Table.Name = name;
            Table.Homogeneous = false;
            Table.NColumns = 4;

            IconButtonWithText buttonKeySplitAccount = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "buttonKeySplitAccount",
                    BackgroundColor = colorRightButton,
                    Text = GeneralUtils.GetResourceByName("global_split_account"),
                    Font = fontRightButton,
                    FontColor = colorFont,
                    Icon = icon1,
                    IconSize = sizeIcon,
                    ButtonSize = new Size(rightButtonWidth, buttonHeight)
                });

            IconButtonWithText buttonKeyMessages = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "buttonKeyMessages",
                    BackgroundColor = colorRightButton,
                    Text = GeneralUtils.GetResourceByName("global_messages"),
                    Font = fontRightButton,
                    FontColor = colorFont,
                    Icon = icon2,
                    IconSize = sizeIcon,
                    ButtonSize = new Size(rightButtonWidth, buttonHeight)
                });

            IconButtonWithText buttonKeyGifts = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "buttonKeyGifts",
                    BackgroundColor = colorRightButton,
                    Text = GeneralUtils.GetResourceByName("global_gifts"),
                    Font = fontRightButton,
                    FontColor = colorFont,
                    Icon = icon3,
                    IconSize = sizeIcon,
                    ButtonSize = new Size(rightButtonWidth, buttonHeight)
                });

            IconButtonWithText buttonKeyWeight = new IconButtonWithText(new ButtonSettings { Name = "buttonKeyWeight", BackgroundColor = colorRightButton, Text = GeneralUtils.GetResourceByName("global_weight"), Font = fontRightButton, FontColor = colorFont, Icon = icon4, IconSize = sizeIcon, ButtonSize = new Size(rightButtonWidth, buttonHeight) });
            
            buttonKeyMessages.Sensitive = false;
            buttonKeyGifts.Sensitive = false;

            //add column4 to base NumberPad
            Table.Attach(buttonKeySplitAccount, 3, 4, 0, 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            Table.Attach(buttonKeyMessages, 3, 4, 1, 2, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            Table.Attach(buttonKeyGifts, 3, 4, 2, 3, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            Table.Attach(buttonKeyWeight, 3, 4, 3, 4, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            //space between NumberPad and POS Keys
            Table.SetColSpacing(2, 6);
        }
    }
}
